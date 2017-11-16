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
        private const string ConfigFileProvider = "Config/Providers.xml";

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

        public static string Password { get; set; }

        public static string User { get; set; }

        public static string ApiUrl { get; set; }

        internal static string DatasetId { get; set; }
        public static long SubscriberLastIndex { get; set; }

        public static ISoapAction ReadArgs(string[] args)
        {
            var action = args[0].ToLower();

            switch (action)
            {
                case "getcapabilities":
                    ApiUrl = args[1];
                    User = args[2];
                    Password = args[3];
                    return new GetCapabilities();
                case "getlastindex":
                    ApiUrl = args[1];
                    DatasetId = args[2];
                    return new GetLastIndex();
                default:
                    throw new NotImplementedException("Action " + action + "not implemented");
            }
        }

        public static void UpdateConfig(IEnumerable<XElement> datasetsList)
        {
            var datasetsDocument = File.Exists(ConfigFileProvider)
                ? ReadConfigFile()
                : new XDocument(new XElement("providers"));

            CreateProviderIfNotExists(datasetsDocument);

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

        private static void CreateProviderIfNotExists(XContainer datasetsDocument)
        {
            if (datasetsDocument.Descendants("provider").Any(d => d.Attribute("uri")?.Value == ApiUrl)) return;

            var providerElement = new XElement("provider");

            providerElement.Add(ProviderDefaults);

            datasetsDocument.Descendants("providers").First().Add(providerElement);
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