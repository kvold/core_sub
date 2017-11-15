using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace CORESubscriber
{
    internal class Program
    {
        private const string ConfigFileProvider = "Config\\Providers.xml";
        private const string XmlMediaType = "text/xml";
        private static string _apiUrl;

        private static readonly List<string> DatasetFields =
            new List<string> {"datasetId", "name", "applicationSchema", "version"};

        private static readonly XNamespace GeosynchronizationNs =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        private static readonly List<XAttribute> DefaultAttributes = new List<XAttribute>
        {
            new XAttribute("subscribed", false),
            new XAttribute("lastindex", 0),
            new XAttribute("wfsClient", "")
        };

        private static string _password;
        private static string _user;

        private static void Main(string[] args)
        {
            _apiUrl = args.Length > 0 ? args[0] : "http://localhost:43397/WebFeatureServiceReplication.svc";
            _user = args.Length > 1 ? args[1] : "https_user";
            _password = args.Length > 2 ? args[2] : "https_user";

            GetCapabilities();
        }

        private static HttpRequestMessage CreateRequest(XDocument soapXml, string action)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_apiUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(soapXml.ToString(), Encoding.UTF8, XmlMediaType)
            };

            request.Headers.Clear();

            request.Content.Headers.ContentType = new MediaTypeHeaderValue(XmlMediaType);

            request.Headers.Add("SOAPAction", GeosynchronizationNs.NamespaceName + "/#" + action);

            return request;
        }

        private static HttpClient GetClient()
        {
            var byteArray = Encoding.ASCII.GetBytes(_user + ":" + _password);

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(XmlMediaType));

            return client;
        }

        private static void GetCapabilities()
        {
            const string action = "GetCapabilities";

            var getCapabilities = GetSoapContentByAction(action);

            var request = CreateRequest(getCapabilities, action);

            var client = GetClient();

            var response = client.SendAsync(request);

            var content = response.Result.Content.ReadAsStringAsync();

            var datasetsList = GetDatasets(content.Result);

            UpdateDatasetsDocument(datasetsList);
        }

        private static IEnumerable<XElement> GetDatasets(string result)
        {
            var datasetsList = new List<XElement>();

            foreach (var dataset in XDocument.Parse(result).Descendants(GeosynchronizationNs + "datasets")
                .Descendants())
            {
                var datasetElement = new XElement("dataset");

                datasetElement.Add(DefaultAttributes);

                foreach (var field in dataset.Descendants().Where(d => DatasetFields.Contains(d.Name.LocalName)))
                {
                    datasetElement.Add(new XAttribute(field.Name.LocalName.Trim(), field.Value.Trim()));

                    Console.WriteLine(field.Name.LocalName + ": " + field.Value.Trim());
                }

                if (datasetElement.Attributes().Count() == DefaultAttributes.Count) continue;

                datasetsList.Add(datasetElement);
            }

            return datasetsList;
        }

        private static void UpdateDatasetsDocument(IEnumerable<XElement> datasetsList)
        {
            var datasetsDocument = XDocument.Parse(File.ReadAllText(ConfigFileProvider));

            CreateProviderIfNotExists(datasetsDocument);

            AddDatasetsToDocument(datasetsList, datasetsDocument);

            datasetsDocument.Save(new FileStream(ConfigFileProvider, FileMode.Open));
        }

        private static void AddDatasetsToDocument(IEnumerable<XElement> datasetsList, XContainer datasetsDocument)
        {
            foreach (var xElement in datasetsList)
            {
                if (datasetsDocument.Descendants("datasets").Descendants().Any(d =>
                    DatasetFields.All(f =>
                        d.Attribute(f)?.Value == xElement.Attribute(f)?.Value)
                ))
                    continue;

                datasetsDocument.Descendants().Where(d => d.Attribute("uri")?.Value == _apiUrl).Descendants("datasets")
                    .First()?.Add(xElement);
            }
        }

        private static void CreateProviderIfNotExists(XContainer datasetsDocument)
        {
            if (datasetsDocument.Descendants("provider").Any(d => d.Attribute("uri")?.Value == _apiUrl)) return;

            var providerElement = new XElement("provider");

            providerElement.Add(new XElement("datasets"), new XAttribute("uri", _apiUrl), new XAttribute("user", _user),
                new XAttribute("password", _password));

            datasetsDocument.Descendants("providers").First().Add(providerElement);
        }

        private static XDocument GetSoapContentByAction(string action)
        {
            return XDocument.Parse(File.ReadAllText("Queries\\" + action + ".xml"));
        }
    }
}