using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            new XElement("abortedChangelog", new XAttribute("endIndex", ""), new XAttribute("transaction", ""),
                new XAttribute("changelogPath", ""), new XAttribute("changelogId", "")),
            new XElement("wfsClient", ""),
            new XElement("subscribed", bool.FalseString)
        };

        internal static readonly List<string> DatasetFields =
            new List<string> {"datasetId", "name", "version", "applicationSchema"};

        internal static XDocument ConfigFileXml { get; set; }

        internal static string Password { get; set; }

        internal static string User { get; set; }

        internal static string ApiUrl { get; set; }

        internal static string DatasetId { get; set; }

        internal static string ApplicationSchema { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        internal static long OrderedChangelogId { get; set; }

        internal static long ProviderLastIndex { get; set; }

        internal static void Save(IEnumerable<XElement> datasetsList)
        {
            ConfigFileXml = File.Exists(ConfigFile)
                ? ReadConfigFile()
                : new XDocument(
                    new XComment(
                        "Settings for Provider. Don't edit attributes unless you know what you're doing! SubscriberLastIndex is -1 to indicate first synchronization. In normal circumstances only the text-value of the elements wfsClient and subscribed should be manually edited."),
                    CreateDefaultProvider());

            AddDatasetsToDocument(datasetsList, ConfigFileXml);

            Save();
        }

        internal static void Save()
        {
            using (var fileStream = new FileStream(ConfigFile, FileMode.OpenOrCreate))
            {
                ConfigFileXml.Save(fileStream);
            }
        }

        internal static void ReadProviderSettings()
        {
            ConfigFileXml = ReadConfigFile();

            var provider = ConfigFileXml.Descendants("provider").First();

            Password = provider.Attribute("password")?.Value;

            User = provider.Attribute("user")?.Value;

            ApiUrl = provider.Attribute("uri")?.Value;

            var dataset = provider.Descendants().First(d => d.Attribute("datasetId")?.Value == DatasetId);

            SubscriberLastIndex = Convert.ToInt64(dataset.Attribute("subscriberLastindex")?.Value);
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