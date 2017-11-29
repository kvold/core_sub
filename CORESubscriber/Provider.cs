using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using CORESubscriber.SoapAction;

namespace CORESubscriber
{
    internal class Provider
    {
        internal static string ConfigFile;

        internal static XDocument ConfigFileXml { get; set; }

        internal static string Password { get; set; }

        internal static string User { get; set; }

        internal static string ApiUrl { get; set; }


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

        internal static void ReadSettings()
        {
            ConfigFileXml = ReadConfigFile();

            var provider = ConfigFileXml.Descendants(Config.Elements.Provider).First();

            Password = provider.Attribute(Config.Attributes.Password)?.Value;

            User = provider.Attribute(Config.Attributes.User)?.Value;

            ApiUrl = provider.Attribute(Config.Attributes.Uri)?.Value;
        }

        private static XDocument ReadConfigFile()
        {
            return XDocument.Parse(File.ReadAllText(ConfigFile));
        }

        private static void AddDatasetsToDocument(IEnumerable<XElement> datasetsList, XContainer datasetsDocument)
        {
            foreach (var xElement in datasetsList)
            {
                if (datasetsDocument.Descendants(Config.Elements.Provider).Descendants().Any(d =>
                    Capabilities.Fields.All(f =>
                        d.Attribute(f)?.Value == xElement.Attribute(f)?.Value)
                ))
                    continue;

                // ReSharper disable once PossibleNullReferenceException
                xElement.Attribute(Config.Attributes.Namespace).Value =
                    GetNamespaceFromApplicationSchema(xElement.Attribute(Config.Attributes.ApplicationSchema)?.Value);

                datasetsDocument.Descendants(Config.Elements.Provider)
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

                    return xsd.Root?.Attribute(Config.Attributes.TargetNamespace)?.Value;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        private static XElement CreateDefaultProvider()
        {
            var providerElement = new XElement(Config.Elements.Provider);

            providerElement.Add(new List<object>
            {
                new XAttribute(Config.Attributes.Uri, ApiUrl),
                new XAttribute(Config.Attributes.User, User),
                new XAttribute(Config.Attributes.Password, Password)
            });

            return providerElement;
        }
    }
}