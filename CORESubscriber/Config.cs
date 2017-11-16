using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CORESubscriber.SoapAction;

namespace CORESubscriber
{
    internal class Config
    {
        internal static string ConfigFileProvider;

        internal const string XmlMediaType = "text/xml";

        public static readonly List<object> ProviderDefaults = new List<object> {new XElement("datasets")};

        public static readonly List<object> DatasetDefaults = new List<object>
        {
            new XAttribute("subscribed", false),
            new XAttribute("lastindex", 0),
            new XAttribute("wfsClient", "")
        };

        internal static readonly XNamespace OwsNs = "http://www.opengis.net/ows/1.1";

        internal static readonly XNamespace GeosynchronizationNs =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        internal static readonly List<string> DatasetFields =
            new List<string> {"datasetId", "name", "version"};

        internal static string Password { get; set; }

        internal static string User { get; set; }

        internal static string ApiUrl { get; set; }

        internal static string DatasetId { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        internal static void UpdateConfig(IEnumerable<XElement> datasetsList)
        {
            var datasetsDocument = File.Exists(ConfigFileProvider) ? ReadConfigFile() : new XDocument(CreateDefaultProvider());

            AddDatasetsToDocument(datasetsList, datasetsDocument);

            datasetsDocument.Save(new FileStream(ConfigFileProvider, FileMode.OpenOrCreate));
        }

        internal static XDocument ReadConfigFile()
        {
            return XDocument.Parse(File.ReadAllText(ConfigFileProvider));
        }

        private static void AddDatasetsToDocument(IEnumerable<XElement> datasetsList, XContainer datasetsDocument)
        {
            foreach (var xElement in datasetsList)
            {
                if (datasetsDocument.Descendants("datasets").Descendants().Any(d =>
                    DatasetFields.All(f =>
                        d.Attribute(f)?.Value == xElement.Attribute(f)?.Value)
                ))
                    continue;

                datasetsDocument.Descendants().Where(d => d.Attribute("uri")?.Value == ApiUrl).Descendants("datasets")
                    .First()?.Add(xElement);
            }
        }

        private static XElement CreateDefaultProvider()
        {
            var providerElement = new XElement("provider");

            providerElement.Add(ProviderDefaults);

            return providerElement;
        }

        internal static void SetProviderDefaults(string capabilitiesFileName)
        {
            var providerDefaults = new List<object>
            {
                new XAttribute("uri", ApiUrl),
                new XAttribute("user", User),
                new XAttribute("password", Password),
                new XAttribute("capabilities", capabilitiesFileName)
            };

            ProviderDefaults.Add(providerDefaults);
        }
    }
}