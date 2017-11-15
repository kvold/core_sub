using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CORESubscriber
{
    class Program
    {
        private const string ApiUrl = "http://localhost:43397/WebFeatureServiceReplication.svc";
        private static string myns = "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        static void Main(string[] args)
        {
            var getCapabilities = File.OpenText("Queries\\GetCapabilities.xml").ReadToEnd();
            TestServiceCall(getCapabilities);
            //TestServiceCall2(getCapabilities);
        }

        private static void TestServiceCall2(string getCapabilities)
        {
            try
            {
                using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }) { Timeout = new TimeSpan(6000) })
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                    //AddAuthorization(client);

                    var response = client.SendAsync(CreateRequest(getCapabilities)).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }

                    Task<Stream> streamTask = response.Content.ReadAsStreamAsync();
                    Stream stream = streamTask.Result;
                    var sr = new StreamReader(stream);
                    var soapResponse = XDocument.Load(sr);
                    Console.WriteLine(soapResponse);

                    //var xml = soapResponse.Descendants(myns + "GetCapabilities").FirstOrDefault().ToString();
                    //var purchaseOrderResult = StaticMethods.Deserialize<GetStuffResult>(xml);
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is TaskCanceledException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static HttpRequestMessage CreateRequest(string getCapabilities)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(ApiUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(getCapabilities, Encoding.UTF8, "text/xml")
            };

            request.Headers.Clear();
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            request.Headers.Add("SOAPAction", "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt/#GetCapabilities");
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

        //public static T Deserialize<T>(string xmlStr)
        //{
        //    var serializer = new XmlSerializer(typeof(T));
        //    T result;
        //    using (TextReader reader = new StringReader(xmlStr))
        //    {
        //        result = (T)serializer.Deserialize(reader);
        //    }
        //    return result;
        //}

        private static void TestServiceCall(string getCapabilities)
        {
            var client = new HttpClient();
            //AddAuthorization(client);
            
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            var request = CreateRequest(getCapabilities);

            var response = client.SendAsync(request);

            while (!response.IsCompleted && !response.IsCanceled && !response.IsFaulted)
            {

            }

            var result = response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(result.Result);

        }
    }
}
