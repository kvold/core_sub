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

        public static readonly List<object> DatasetDefaults = new List<object>
        {
            new XAttribute("nameSpace", ""),
            new XAttribute("subscriberLastindex", 0),
            new XAttribute("wfsClient", "")
        };

        internal static readonly List<string> DatasetFields =
            new List<string> {"datasetId", "name", "version", "applicationSchema"};

        internal static string Password { get; set; }

        internal static string User { get; set; }

        internal static string ApiUrl { get; set; }

        internal static string DatasetId { get; set; }

        public static string ApplicationSchema { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        internal static void Save(IEnumerable<XElement> datasetsList)
        {
            var datasetsDocument = File.Exists(ConfigFile) ? ReadConfigFile() : new XDocument(CreateDefaultProvider());

            AddDatasetsToDocument(datasetsList, datasetsDocument);

            datasetsDocument.Save(new FileStream(ConfigFile, FileMode.OpenOrCreate));
        }

        internal static XDocument ReadConfigFile()
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
                xElement.Attribute("nameSpace").Value = GetNamespaceFromApplicationSchema(xElement.Attribute("applicationSchema")?.Value);

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