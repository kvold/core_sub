using System;
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
        private const string ApiUrl = "http://localhost:43397/WebFeatureServiceReplication.svc";
        private static XNamespace Myns = "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        private static void Main(string[] args)
        {
            TestServiceCall();
        }

        private static HttpRequestMessage CreateRequest(string soapXml, string action)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(ApiUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(soapXml, Encoding.UTF8, "text/xml")
            };

            request.Headers.Clear();
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            request.Headers.Add("SOAPAction", Myns.NamespaceName + "/#" + action);
            return request;
        }

        private static void AddAuthorization(HttpClient client)
        {
            var password = "https_user";
            var user = "https_user";

            var byteArray = Encoding.ASCII.GetBytes(user + ":" + password);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        }

        private static void TestServiceCall()
        {
            var client = new HttpClient();

            //AddAuthorization(client);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            var getCapabilities = File.OpenText("Queries\\GetCapabilities.xml").ReadToEnd();

            var request = CreateRequest(getCapabilities, "GetCapabilities");

            var response = client.SendAsync(request);

            var result = response.Result.Content.ReadAsStringAsync();

            var soap = XDocument.Parse(result.Result);

            foreach (var dataset in soap.Descendants(Myns + "datasets").First().Descendants())
            {
                foreach (var field in dataset.Descendants())
                {
                    Console.WriteLine(field.Name.LocalName + ":" + field.Value);
                }
            }
        }
    }
}