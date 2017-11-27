using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
// ReSharper disable PossibleNullReferenceException

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

            Provider.ConfigFileXml.Descendants()
                .First(d => d.Attribute("datasetId")?.Value == Id)
                .Attribute("subscriberLastindex").Value = ProviderLastIndex.ToString();

            Provider.Save();
        }

        internal static bool ReadVariables(XObject subscribed)
        {
            Id = subscribed.Parent?.Attribute("datasetId")?.Value;

            SubscriberLastIndex = Convert.ToInt64(subscribed.Parent?.Attribute("subscriberLastindex")?.Value);

            OrderedChangelogId = Convert.ToInt64(Provider.ConfigFileXml.Descendants("dataset")
                .First(d => d.Attribute("datasetId")?.Value == Id)
                .Descendants("abortedChangelog").First().Attribute("changelogId").Value);

            return OrderedChangelogId == -1;
        }
    }
}