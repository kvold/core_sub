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
        private static readonly XNamespace GeosynchronizationNs =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        private static void Main(string[] args)
        {
            var apiUrl = args.Length > 0 ? args[0] : "http://localhost:43397/WebFeatureServiceReplication.svc";
            GetCapabilities(apiUrl);
        }

        private static HttpRequestMessage CreateRequest(XDocument soapXml, string action, string apiUrl)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(apiUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(soapXml.ToString(), Encoding.UTF8, "text/xml")
            };

            request.Headers.Clear();
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            request.Headers.Add("SOAPAction", GeosynchronizationNs.NamespaceName + "/#" + action);
            return request;
        }

        private static HttpClient GetClient()
        {
            var password = "https_user";
            var user = "https_user";

            var byteArray = Encoding.ASCII.GetBytes(user + ":" + password);

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            return client;
        }

        private static void GetCapabilities(string apiUrl)
        {
            const string action = "GetCapabilities";

            var getCapabilities = GetSoapContentByAction(action);

            var request = CreateRequest(getCapabilities, action, apiUrl);

            var client = GetClient();

            var response = client.SendAsync(request);

            var result = response.Result.Content.ReadAsStringAsync();

            var datasetFields = new List<string> {"datasetId", "name", "version", "applicationSchema"};

            var datasetsList = new List<XElement>();

            foreach (var dataset in XDocument.Parse(result.Result).Descendants(GeosynchronizationNs + "datasets")
                .Descendants())
            {
                var datasetElement = new XElement("dataset");
                foreach (var field in dataset.Descendants().Where(d => datasetFields.Contains(d.Name.LocalName)))
                {
                    var attribute = new XAttribute(field.Name.LocalName.Trim(), field.Value.Trim());
                    datasetElement.Add(attribute);
                    Console.WriteLine(field.Name.LocalName + ": " + field.Value.Trim());
                }

                if (!datasetElement.Attributes().Any()) continue;

                datasetElement.Add(new XAttribute("subscribed", false));
                datasetsList.Add(datasetElement);
            }

            var datasetsDocument = XDocument.Parse(File.ReadAllText("Config\\Datasets.xml"));

            foreach (var xElement in datasetsList)
            {
                if (datasetsDocument.Descendants("datasets").Descendants().Any(d =>
                    datasetFields.All(f =>
                        d.Attribute(f)?.Value == xElement.Attribute(f)?.Value)
                ))
                    continue;
                datasetsDocument.Root?.Add(xElement);
            }

            datasetsDocument.Save(new FileStream("Config\\Datasets.xml", FileMode.Open));
        }

        private static XDocument GetSoapContentByAction(string action)
        {
            return XDocument.Parse(File.ReadAllText("Queries\\" + action + ".xml"));
        }
    }
}