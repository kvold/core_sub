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
            new XAttribute(Config.Attributes.Namespace, ""),

            new XAttribute(Config.Attributes.SubscriberLastIndex, -1),

            new XElement(Config.Elements.AbortedChangelog,

                new XAttribute(Config.Attributes.EndIndex, ""),

                new XAttribute(Config.Attributes.Transaction, ""),

                new XAttribute(Config.Attributes.ChangelogPath, ""),

                new XAttribute(Config.Attributes.ChangelogId, -1)),

            new XElement(Config.Elements.WfsClient, ""),

            new XElement(Config.Elements.Subscribed, bool.FalseString)
        };

        internal static string Id { get; set; }

        internal static long OrderedChangelogId { get; set; }

        internal static long ProviderLastIndex { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        internal static void UpdateSettings()
        {
            OrderedChangelogId = -1;

            Provider.ConfigFileXml.Descendants()
                .First(d => d.Attribute(Config.Attributes.DatasetId)?.Value == Id)
                .Attribute(Config.Attributes.SubscriberLastIndex).Value = ProviderLastIndex.ToString();

            Provider.Save();
        }

        internal static bool ReadVariables(XObject subscribed)
        {
            Id = subscribed.Parent?.Attribute(Config.Attributes.DatasetId)?.Value;

            SubscriberLastIndex = Convert.ToInt64(subscribed.Parent?.Attribute(Config.Attributes.SubscriberLastIndex)?.Value);

            OrderedChangelogId = Convert.ToInt64(Provider.ConfigFileXml.Descendants(Config.Elements.Dataset)
                .First(d => d.Attribute(Config.Attributes.DatasetId)?.Value == Id)
                .Descendants(Config.Elements.AbortedChangelog).First().Attribute(Config.Attributes.ChangelogId).Value);

            return OrderedChangelogId == -1;
        }
    }
}