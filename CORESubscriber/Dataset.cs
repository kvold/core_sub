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
            new XAttribute(XmlAttributes.Namespace, ""),

            new XAttribute(XmlAttributes.SubscriberLastIndex, -1),

            new XElement(XmlElements.AbortedChangelog,
                new XAttribute(XmlAttributes.EndIndex, ""),
                new XAttribute(XmlAttributes.Transaction, ""),
                new XAttribute(XmlAttributes.ChangelogPath, ""),
                new XAttribute(XmlAttributes.ChangelogId, -1)),

            new XElement(XmlElements.WfsClient, ""),

            new XElement(XmlElements.Subscribed, bool.FalseString),

            new XElement(XmlElements.Precision, 
                new XAttribute(XmlAttributes.EpsgCode, ""),
                new XAttribute(XmlAttributes.Decimals, ""),
                new XAttribute(XmlAttributes.Tolerance, ""))

            //new XAttribute(XmlAttributes.Version, "")
        };

        internal static string Id { get; set; }

        internal static long OrderedChangelogId { get; set; }

        internal static long ProviderLastIndex { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        public static string Version { get; set; }

        internal static void UpdateSettings()
        {
            OrderedChangelogId = -1;

            Provider.ConfigFileXml.Descendants()
                .First(d => d.Attribute(XmlAttributes.DatasetId)?.Value == Id)
                .Attribute(XmlAttributes.SubscriberLastIndex).Value = ProviderLastIndex.ToString();

            Provider.Save();
        }

        internal static bool ReadVariables(XObject subscribed)
        {
            Id = subscribed.Parent?.Attribute(XmlAttributes.DatasetId)?.Value;

            SubscriberLastIndex =
                Convert.ToInt64(subscribed.Parent?.Attribute(XmlAttributes.SubscriberLastIndex)?.Value);

            OrderedChangelogId = Convert.ToInt64(Provider.ConfigFileXml.Descendants(XmlElements.Dataset)
                .First(d => d.Attribute(XmlAttributes.DatasetId)?.Value == Id)
                .Descendants(XmlElements.AbortedChangelog).First().Attribute(XmlAttributes.ChangelogId).Value);

            return OrderedChangelogId == -1;
        }
    }
}