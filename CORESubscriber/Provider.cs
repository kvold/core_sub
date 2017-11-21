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
    internal class Provider
    {
        internal static string ConfigFile;

        internal static readonly List<object> DatasetDefaults = new List<object>
        {
            new XAttribute("nameSpace", ""),
            new XAttribute("subscriberLastindex", -1),
            new XElement("abortedChangelog", new XAttribute("endIndex", ""), new XAttribute("transaction", ""), new XAttribute("changelogPath", ""), new XAttribute("changelogId", "")),
            new XElement("wfsClient", ""),
            new XElement("subscribed", bool.FalseString)
        };

        internal static readonly List<string> DatasetFields =
            new List<string> {"datasetId", "name", "version", "applicationSchema"};

        internal static string Password { get; set; }

        internal static string User { get; set; }

        internal static string ApiUrl { get; set; }

        internal static string DatasetId { get; set; }

        internal static string ApplicationSchema { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        internal static long OrderedChangelogId { get; set; }

        internal static string OrderedChangelogDownloadUrl { get; set; }

        internal static void Save(IEnumerable<XElement> datasetsList)
        {
            var datasetsDocument = File.Exists(ConfigFile) ? ReadConfigFile() : new XDocument(new XComment("Settings for Provider. Don't edit attributes unless you know what you're doing! SubscriberLastIndex is -1 to indicate first synchronization. In normal circumstances only the text-value of the elements wfsClient and subscribed should be manually edited."), CreateDefaultProvider());

            AddDatasetsToDocument(datasetsList, datasetsDocument);

            datasetsDocument.Save(new FileStream(ConfigFile, FileMode.OpenOrCreate));
        }

        internal static void Save()
        {
            var datasetsDocument = ReadConfigFile();

            // ReSharper disable once PossibleNullReferenceException
            datasetsDocument.Descendants("dataset").First(d => d.Attribute("datasetId")?.Value == DatasetId)
                .Descendants("abortedChangelog").First().Attribute("changelogId").Value = OrderedChangelogId.ToString();

            datasetsDocument.Save(new FileStream(ConfigFile, FileMode.OpenOrCreate));
        }

        internal static void ReadProviderSettings()
        {
            var configFile = ReadConfigFile();

            var provider = configFile.Descendants("provider").First();

            Password = provider.Attribute("password")?.Value;

            User = provider.Attribute("user")?.Value;

            ApiUrl = provider.Attribute("uri")?.Value;

            var dataset = provider.Descendants().First(d => d.Attribute("datasetId")?.Value == DatasetId);

            SubscriberLastIndex = Convert.ToInt64(dataset.Attribute("subscriberLastindex")?.Value);
        }

        internal static void GetChangeLogZipFile()
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(User + ":" + Password);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var result = client.GetAsync(OrderedChangelogDownloadUrl).Result;

                if (!result.IsSuccessStatusCode)
                    throw new FileNotFoundException("Statuscode when trying to download from " +
                                                    OrderedChangelogDownloadUrl + " was " + result.StatusCode);

                var fileName = Config.DownloadFolder + "/" +
                               OrderedChangelogDownloadUrl.Split('/')[
                                   OrderedChangelogDownloadUrl.Split('/').Length - 1];

                using (var fs = new FileStream(fileName, FileMode.Create)) result.Content.ReadAsStreamAsync().Result.CopyToAsync(fs);

            }
        }

        private static XDocument ReadConfigFile()
        {
            return XDocument.Parse(File.ReadAllText(ConfigFile));
        }

        private static void AddDatasetsToDocument(IEnumerable<XElement> datasetsList, XContainer datasetsDocument)
        {
            foreach (var xElement in datasetsList)
            {
                if (datasetsDocument.Descendants("provider").Descendants().Any(d =>
                    DatasetFields.All(f =>
                        d.Attribute(f)?.Value == xElement.Attribute(f)?.Value)
                ))
                    continue;

                // ReSharper disable once PossibleNullReferenceException
                xElement.Attribute("nameSpace").Value =
                    GetNamespaceFromApplicationSchema(xElement.Attribute("applicationSchema")?.Value);

                datasetsDocument.Descendants("provider")
                    .First()?.Add(xElement);
            }
        }

        private static string GetNamespaceFromApplicationSchema(string applicationSchema)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var result = client.GetAsync(applicationSchema).Result;

                    var xsd = XDocument.Parse(result.Content.ReadAsStringAsync().Result);

                    return xsd.Root?.Attribute("targetNamespace")?.Value;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        private static XElement CreateDefaultProvider()
        {
            var providerElement = new XElement("provider");

            providerElement.Add(new List<object>
            {
                new XAttribute("uri", ApiUrl),
                new XAttribute("user", User),
                new XAttribute("password", Password)
            });

            return providerElement;
        }
    }
}