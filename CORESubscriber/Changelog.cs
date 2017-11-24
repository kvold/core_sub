using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

namespace CORESubscriber
{
    internal class Changelog
    {
        internal static string ZipFile { get; set; }

        internal static string Uuid { get; set; }

        internal static string DownloadUrl { get; set; }

        internal static string DataFolder { get; set; }

        internal static string WfsClient { get; set; }

        internal static void Get()
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(Provider.User + ":" + Provider.Password);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var result = client.GetAsync(DownloadUrl).Result;

                if (!result.IsSuccessStatusCode)
                    throw new FileNotFoundException("Statuscode when trying to download from " +
                                                    DownloadUrl + " was " + result.StatusCode);

                Uuid = DownloadUrl.Split('/')[DownloadUrl.Split('/').Length - 1];

                ZipFile = Config.DownloadFolder + Uuid;

                Uuid = Uuid.Replace(".zip", "");

                using (var fs = new FileStream(ZipFile, FileMode.Create))
                {
                    result.Content.CopyToAsync(fs);
                }
            }

            Unzip();
        }

        internal static void Unzip()
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(ZipFile, Config.DownloadFolder);

            DataFolder = Config.DownloadFolder + Uuid;
        }

        internal static void Execute()
        {
            WfsClient = Provider.ConfigFileXml.Descendants().First(d => d.Attribute("datasetId")?.Value == Provider.DatasetId).Descendants("wfsClient").First().Value;

            if(WfsClient == "") throw new Exception("No wfsClient given for dataset " + Provider.DatasetId);

            var directoryInfo = new DirectoryInfo(DataFolder);

            foreach (var directory in directoryInfo.GetDirectories()) ReadFiles(directory.GetFiles());

            ReadFiles(directoryInfo.GetFiles());
        }
        
        private static void ReadFiles(IEnumerable<FileInfo> files)
        {
            foreach (var fileInfo in files)
            {
                var changelogXml = XDocument.Parse(fileInfo.OpenText().ReadToEnd());

                foreach (var transaction in changelogXml.Descendants(Config.ChangelogNs + "transactions"))
                {
                    transaction.Name = Config.WfsXNamespace + "Transaction";

                    Send(new XDocument(transaction));
                }
            }
            UpdateProviderSettings();
        }

        private static void Send(XNode transactionDocument)
        {
            using (var client = new HttpClient())
            {
                var httpContent = new StringContent(transactionDocument.ToString(), Encoding.UTF8, Config.XmlMediaType);

                var response = client.PostAsync(WfsClient, httpContent);

                if (!response.Result.IsSuccessStatusCode)
                {
                    var errorMessage = response.Result.Content.ReadAsStringAsync().Result;

                    Console.WriteLine(errorMessage);

                    throw new TransactionAbortedException("Transaction failed. Message from WFS-server: \r\n" + errorMessage);
                }

                Console.WriteLine(XDocument.Parse(response.Result.Content.ReadAsStringAsync().Result).Descendants(Config.WfsXNamespace + "TransactionSummary").First().ToString());
            }
        }

        private static void UpdateProviderSettings()
        {
            Provider.OrderedChangelogId = -1;

            // ReSharper disable once PossibleNullReferenceException
            Provider.ConfigFileXml.Descendants()
                .First(d => d.Attribute("datasetId")?.Value == Provider.DatasetId)
                .Attribute("subscriberLastindex").Value = Provider.ProviderLastIndex.ToString();

            Provider.Save();
        }
    }
}