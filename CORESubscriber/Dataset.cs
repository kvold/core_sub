using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CORESubscriber.Xml;

// ReSharper disable PossibleNullReferenceException

namespace CORESubscriber
{
    internal class Dataset
    {
        internal static readonly List<object> DefaultElements = new List<object>
        {
            new XAttribute(XmlNames.Attributes.Namespace, ""),

            new XAttribute(XmlNames.Attributes.SubscriberLastIndex, -1),

            new XElement(XmlNames.Elements.AbortedChangelog,
                new XAttribute(XmlNames.Attributes.EndIndex, ""),
                new XAttribute(XmlNames.Attributes.Transaction, ""),
                new XAttribute(XmlNames.Attributes.ChangelogPath, ""),
                new XAttribute(XmlNames.Attributes.ChangelogId, -1)),

            new XElement(XmlNames.Elements.WfsClient, ""),

            new XElement(XmlNames.Elements.Subscribed, bool.FalseString)
        };

        internal static string Id { get; set; }

        internal static long OrderedChangelogId { get; set; }

        internal static long ProviderLastIndex { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        internal static void UpdateSettings()
        {
            OrderedChangelogId = -1;

            Provider.ConfigFileXml.Descendants()
                .First(d => d.Attribute(XmlNames.Attributes.DatasetId)?.Value == Id)
                .Attribute(XmlNames.Attributes.SubscriberLastIndex).Value = ProviderLastIndex.ToString();

            Provider.Save();
        }

        internal static bool ReadVariables(XObject subscribed)
        {
            Id = subscribed.Parent?.Attribute(XmlNames.Attributes.DatasetId)?.Value;

            SubscriberLastIndex =
                Convert.ToInt64(subscribed.Parent?.Attribute(XmlNames.Attributes.SubscriberLastIndex)?.Value);

            OrderedChangelogId = Convert.ToInt64(Provider.ConfigFileXml.Descendants(XmlNames.Elements.Dataset)
                .First(d => d.Attribute(XmlNames.Attributes.DatasetId)?.Value == Id)
                .Descendants(XmlNames.Elements.AbortedChangelog).First().Attribute(XmlNames.Attributes.ChangelogId).Value);

            return OrderedChangelogId == -1;
        }
    }
}