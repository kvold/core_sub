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
        };

        internal static string Id { get; set; }

        internal static string ChangelogIdDefaultValue = "-1";

        internal static string OrderedChangelogId { get; set; }

        internal static long ProviderLastIndex { get; set; }

        internal static long SubscriberLastIndex { get; set; }

        public static string Version { get; set; }

        internal static void UpdateSettings()
        {
            OrderedChangelogId = ChangelogIdDefaultValue;

            SetSubscriberLastIndexToProviderLastIndex();

            SetAbortedchangelogToChangelogId();

            Provider.Save();
        }

        private static void SetAbortedchangelogToChangelogId()
        {
            GetDatasetConfig().Descendants(XmlElements.AbortedChangelog).First().SetAttributeValue(XmlAttributes.ChangelogId, ChangelogIdDefaultValue);

            Provider.Save();
        }

        private static void SetSubscriberLastIndexToProviderLastIndex()
        {
            GetDatasetConfig().Attribute(XmlAttributes.SubscriberLastIndex).Value = ProviderLastIndex.ToString();

            Provider.Save();
        }

        internal static bool ReadVariables(XObject subscribed)
        {
            Id = subscribed.Parent?.Attribute(XmlAttributes.DatasetId)?.Value;

            SubscriberLastIndex = GetSubscriberLastIndex(subscribed);
              
            OrderedChangelogId = GetAbortedChangelogId();

            Version = GetVersion(subscribed);

            return OrderedChangelogId == ChangelogIdDefaultValue;
        }

        private static string GetVersion(XObject subscribed)
        {
            return subscribed.Parent?.Attribute(XmlAttributes.Version)?.Value;
        }

        private static long GetSubscriberLastIndex(XObject subscribed)
        {
            return Convert.ToInt64(subscribed.Parent?.Attribute(XmlAttributes.SubscriberLastIndex)?.Value);
        }

        private static string GetAbortedChangelogId()
        {
            return GetDatasetConfig().Descendants(XmlElements.AbortedChangelog).First().Attribute(XmlAttributes.ChangelogId).Value;
        }

        public static void SetChangelogPath(string dataFolder)
        {
            GetDatasetConfig().Descendants(XmlElements.AbortedChangelog).First().SetAttributeValue(XmlAttributes.ChangelogPath, dataFolder);

            Provider.Save();
        }

        public static void SetEndindex(string endIndex)
        {
            GetDatasetConfig().Descendants(XmlElements.AbortedChangelog).First().SetAttributeValue(XmlAttributes.EndIndex, endIndex);

            Provider.Save();
        }

        public static void SetTransaction(string transaction)
        {
            GetDatasetConfig().Descendants(XmlElements.AbortedChangelog).First().SetAttributeValue(XmlAttributes.Transaction, transaction);

            Provider.Save();
        }

        private static XElement GetDatasetConfig()
        {
            return Provider.ConfigFileXml.Descendants(XmlElements.Dataset).First(d => d.Attribute(XmlAttributes.DatasetId)?.Value == Id);
        }

        public static void ResetAbortedChangelog()
        {
            SetChangelogPath("");

            SetEndindex("");

            SetTransaction("");
        }

        public static void SetOrderedChangelogId(string orderedChangelogId)
        {
            OrderedChangelogId = orderedChangelogId;

            GetDatasetConfig().Descendants(XmlElements.AbortedChangelog).First().SetAttributeValue(XmlAttributes.ChangelogId, OrderedChangelogId);

            Provider.Save();


        }

        public static bool ChangelogIdIsDefault()
        {
            return OrderedChangelogId == ChangelogIdDefaultValue;
        }
    }
}