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
                    .Descendants(XmlNamespaces.Geosynchronization + XmlAttributes.ChangelogId.LocalName).First().Value);

            Provider.ConfigFileXml.Descendants(XmlElements.Dataset)
                    .First(d => d.Attribute(XmlAttributes.DatasetId)?.Value == Dataset.Id)
                    .Descendants(XmlElements.AbortedChangelog).First().Attribute(XmlAttributes.ChangelogId)
                    .Value =
                Dataset.OrderedChangelogId.ToString();

            Provider.Save();
        }

        private static XDocument SetOrderVariables(XDocument orderChangelog)
        {
            orderChangelog.Descendants(XmlNamespaces.Geosynchronization + XmlElements.Order.LocalName).First()
                    .Attribute(XmlAttributes.StartIndex).Value =
                (Dataset.SubscriberLastIndex + 1).ToString();

            orderChangelog.Descendants(XmlNamespaces.Geosynchronization + XmlAttributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            orderChangelog.Descendants(XmlNamespaces.Geosynchronization + XmlElements.Order.LocalName).First()
                    .Attribute(XmlAttributes.Count).Value =
                Config.OrderedChangeCount;

            return orderChangelog;
        }
    }
}