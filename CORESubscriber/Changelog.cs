using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace CORESubscriber
{
    internal class Changelog
    {
        private static string DataFolder { get; set; }

        private static string WfsClient { get; set; }

        internal static async Task Get(string downloadUrl)
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
                    await result.Content.CopyToAsync(fs);
                }
            }

            ZipFile.ExtractToDirectory(uuid, zipFile);
        }

        internal static void Execute()
        {
            WfsClient = Provider.ConfigFileXml.Descendants()
                .First(d => d.Attribute("datasetId")?.Value == Dataset.Id).Descendants("wfsClient").First()
                .Value;

            if (WfsClient == "") throw new Exception("No wfsClient given for dataset " + Dataset.Id);

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

                    throw new TransactionAbortedException("Transaction failed. Message from WFS-server: \r\n" +
                                                          errorMessage);
                }

                Console.WriteLine(XDocument.Parse(response.Result.Content.ReadAsStringAsync().Result)
                    .Descendants(Config.WfsNs + "TransactionSummary").First().ToString());
            }
        }
    }
}