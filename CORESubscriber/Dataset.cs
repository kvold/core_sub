using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CORESubscriber
{
    internal class Dataset
    {
        internal static readonly List<object> DefaultElements = new List<object>
        {
            new XAttribute("nameSpace", ""),

            new XAttribute("subscriberLastindex", -1),

            new XElement("abortedChangelog",

                new XAttribute("endIndex", ""),

                new XAttribute("transaction", ""),

                new XAttribute("changelogPath", ""),

                new XAttribute("changelogId", -1)),

            new XElement("wfsClient", ""),

            new XElement("subscribed", bool.FalseString)
        };

        internal static readonly List<string> Fields =
            new List<string>
            {
                "datasetId",

                "name",

                "version",

                "applicationSchema"
            };

        internal static string Id { get; set; }

        internal static long OrderedChangelogId { get; set; }

        internal static long ProviderLastIndex { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        internal static void UpdateSettings()
        {
            OrderedChangelogId = -1;

            // ReSharper disable once PossibleNullReferenceException
            Provider.ConfigFileXml.Descendants()
                .First(d => d.Attribute("datasetId")?.Value == Dataset.Id)
                .Attribute("subscriberLastindex").Value = Dataset.ProviderLastIndex.ToString();

            Provider.Save();
        }
    }
}