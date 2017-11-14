using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace CORESubscriber
{
    class Program
    {
        static void Main(string[] args)
        {

            var getCapabilities = File.OpenText("Queries\\GetCapabilities.xml").ReadToEnd();
            TestServiceCall(getCapabilities, args);
        }

        private static void TestServiceCall(string getCapabilities, string[] args)
        {
            var password = args[0];
            var user = args[1];
            var byteArray = Encoding.ASCII.GetBytes(user + ":" + password);
            const string uri = "https://geosync.kystverket.no/kystverket/";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var response = client.PostAsync(uri, new StringContent(getCapabilities, Encoding.UTF8, "text/xml"));

            while (!response.IsCompleted && !response.IsCanceled && !response.IsFaulted)
            {

            }
            Console.WriteLine(response.Result);

        }
    }
}
