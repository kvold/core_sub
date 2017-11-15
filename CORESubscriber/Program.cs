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

        private static HttpRequestMessage CreateRequest(string soapXml, string action, string apiUrl)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(apiUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(soapXml, Encoding.UTF8, "text/xml")
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

            foreach (var field in XDocument.Parse(result.Result).Descendants()
                .Where(d => datasetFields.Contains(d.Name.LocalName)))
            {
                Console.WriteLine(field.Name.LocalName + ": " + field.Value.Trim());
            }
        }

        private static string GetSoapContentByAction(string action)
        {
            return File.OpenText("Queries\\" + action + ".xml").ReadToEnd();
        }
    }
}