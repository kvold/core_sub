using System;
using System.Linq;
using System.Xml.Linq;
using CORESubscriber.Xml;

// ReSharper disable PossibleNullReferenceException

namespace CORESubscriber.SoapAction
{
    internal class OrderChangelog
    {
        public static void Run()
        {
            if (Dataset.OrderedChangelogId != -1) return;

            const string action = "OrderChangelog";

            var responseContent =
                SoapRequest.Send(action, SetOrderVariables(SoapRequest.GetSoapContentByAction(action)));

            Dataset.OrderedChangelogId =
                Convert.ToInt64(responseContent
                    .Descendants(XmlNamespaces.Geosynchronization + XmlNames.Attributes.ChangelogId.LocalName).First().Value);

            Provider.ConfigFileXml.Descendants(XmlNames.Elements.Dataset)
                    .First(d => d.Attribute(XmlNames.Attributes.DatasetId)?.Value == Dataset.Id)
                    .Descendants(XmlNames.Elements.AbortedChangelog).First().Attribute(XmlNames.Attributes.ChangelogId)
                    .Value =
                Dataset.OrderedChangelogId.ToString();

            Provider.Save();
        }

        private static XDocument SetOrderVariables(XDocument orderChangelog)
        {
            orderChangelog.Descendants(XmlNamespaces.Geosynchronization + XmlNames.Elements.Order.LocalName).First()
                    .Attribute(XmlNames.Attributes.StartIndex).Value =
                (Dataset.SubscriberLastIndex + 1).ToString();

            orderChangelog.Descendants(XmlNamespaces.Geosynchronization + XmlNames.Attributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            orderChangelog.Descendants(XmlNamespaces.Geosynchronization + XmlNames.Elements.Order.LocalName).First()
                    .Attribute(XmlNames.Attributes.Count).Value =
                Config.OrderedChangeCount;

            return orderChangelog;
        }
    }
}