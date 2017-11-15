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
        private const string ConfigFileProvider = "Config/Providers.xml";
        private const string XmlMediaType = "text/xml";
        
        private static readonly List<string> DatasetFields =
            new List<string> {"datasetId", "name", "version"};

        private static readonly XNamespace OwsNs = "http://www.opengis.net/ows/1.1";
        private static readonly XNamespace GeosynchronizationNs =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";


        private static readonly List<object> ProviderDefaults = new List<object> { new XElement("datasets") };
        private static readonly List<object> DatasetDefaults = new List<object>
        {
            new XAttribute("subscribed", false),
            new XAttribute("lastindex", 0),
            new XAttribute("wfsClient", "")
        };
        
        private static string _apiUrl;
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

            var getCapabilities = GetSoapRequestContentByAction(action);

            var responseContent = SendRequest(action, getCapabilities);

            var capabilitiesFileName = "Capabilities/" +
                                       responseContent.Descendants(OwsNs + "Title").First().Value
                                           .Replace(" ", "_") + ".xml";

            responseContent.Save(new FileStream(capabilitiesFileName, FileMode.OpenOrCreate));

            SetProviderDefaults(capabilitiesFileName);

            var datasetsList = GetDatasets(responseContent);

            UpdateConfig(datasetsList);
        }

        private static XDocument SendRequest(string action, XDocument requestContent)
        {
            var request = CreateRequest(requestContent, action);

            var client = GetClient();

            var response = client.SendAsync(request);

            return XDocument.Parse(response.Result.Content.ReadAsStringAsync().Result);
        }

        private static void SetProviderDefaults(string capabilitiesFileName)
        {
            var providerDefaults = new List<object>
            {
                new XAttribute("uri", _apiUrl),
                new XAttribute("user", _user),
                new XAttribute("password", _password),
                new XAttribute("capabilities", capabilitiesFileName)
            };

            ProviderDefaults.Add(providerDefaults);
        }


        private static IEnumerable<XElement> GetDatasets(XContainer result)
        {
            var datasetsList = new List<XElement>();

            foreach (var dataset in result.Descendants(GeosynchronizationNs + "datasets")
                .Descendants())
            {
                var datasetElement = new XElement("dataset");

                datasetElement.Add(DatasetDefaults);

                foreach (var field in dataset.Descendants().Where(d => DatasetFields.Contains(d.Name.LocalName)))
                {
                    datasetElement.Add(new XAttribute(field.Name.LocalName.Trim(), field.Value.Trim()));

                    Console.WriteLine(field.Name.LocalName + ": " + field.Value.Trim());
                }

                if (datasetElement.Attributes().Count() == DatasetDefaults.Count) continue;

                datasetsList.Add(datasetElement);
            }

            return datasetsList;
        }

        private static void UpdateConfig(IEnumerable<XElement> datasetsList)
        {
            var datasetsDocument = File.Exists(ConfigFileProvider)
                ? XDocument.Parse(File.ReadAllText(ConfigFileProvider))
                : new XDocument(new XElement("providers"));

            CreateProviderIfNotExists(datasetsDocument);

            AddDatasetsToDocument(datasetsList, datasetsDocument);

            datasetsDocument.Save(new FileStream(ConfigFileProvider, FileMode.OpenOrCreate));
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

            providerElement.Add(ProviderDefaults);

            datasetsDocument.Descendants("providers").First().Add(providerElement);
        }

        private static XDocument GetSoapRequestContentByAction(string action)
        {
            return XDocument.Parse(File.ReadAllText("Queries/" + action + ".xml"));
        }
    }
}