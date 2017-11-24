using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        private static string DataFolder { get; set; }

        private static string WfsClient { get; set; }

        internal static void Get(string downloadUrl)
        {
            string uuid;

            string zipFile;

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(Provider.User + ":" + Provider.Password);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var result = client.GetAsync(downloadUrl).Result;

                if (!result.IsSuccessStatusCode)
                    throw new FileNotFoundException("Statuscode when trying to download from " +
                                                    downloadUrl + " was " + result.StatusCode);

                uuid = downloadUrl.Split('/')[downloadUrl.Split('/').Length - 1];

                zipFile = Config.DownloadFolder + "/" + uuid;

                uuid = uuid.Split(".")[0];

                DataFolder = Config.DownloadFolder + "/" + uuid;

                using (var fs = new FileStream(zipFile, FileMode.Create))
                {
                    result.Content.CopyToAsync(fs);
                }
            }

            Unzip(uuid, zipFile);
        }

        internal static void Unzip(string uuid, string zipFile)
        {
            ZipFile.ExtractToDirectory(zipFile, Config.DownloadFolder);
        }

        internal static void Execute()
        {
            WfsClient = Provider.ConfigFileXml.Descendants()
                .First(d => d.Attribute("datasetId")?.Value == Provider.DatasetId).Descendants("wfsClient").First()
                .Value;

            if (WfsClient == "") throw new Exception("No wfsClient given for dataset " + Provider.DatasetId);

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
                    transaction.Name = Config.WfsNs + "Transaction";

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

                    throw new TransactionAbortedException("Transaction failed. Message from WFS-server: \r\n" +
                                                          errorMessage);
                }

                Console.WriteLine(XDocument.Parse(response.Result.Content.ReadAsStringAsync().Result)
                    .Descendants(Config.WfsNs + "TransactionSummary").First().ToString());
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