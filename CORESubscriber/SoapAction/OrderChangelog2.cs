using System;
using System.Linq;
using System.Xml.Linq;
using CORESubscriber.Xml;

// ReSharper disable PossibleNullReferenceException

namespace CORESubscriber.SoapAction
{
    internal class OrderChangelog2
    {
        public static void Run()
        {
            if (!Dataset.ChangelogIdIsDefault()) return;

            Dataset.SetOrderedChangelogId(GetOrderedChangelogIdFromResponseContent());

            if (Dataset.ChangelogIdIsDefault())
                throw new Exception("Provider datasetVersion differs from subscriber.");

            Dataset.SetAbortedChangelog(Dataset.OrderedChangelogId);
        }


        private static string GetOrderedChangelogIdFromResponseContent()
        {
            return GetResponseContent()
                .Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.ChangelogId.LocalName).First().Value;
        }

        private static XDocument GetResponseContent()
        {
            return SoapRequest.Send(SoapActions.OrderChangelog2,
                SetOrderVariables(SoapRequest.GetSoapContentByAction(SoapActions.OrderChangelog2)));
        }

        private static XDocument SetOrderVariables(XDocument orderChangelog)
        {
            SetSubscriberLastIndex(orderChangelog);

            SetDatasetId(orderChangelog);

            SetOrderedChangeCount(orderChangelog);

            SetDatasetVersion(orderChangelog);

            return orderChangelog;
        }

        private static void SetDatasetVersion(XContainer orderChangelog)
        {
            orderChangelog.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.DatasetVersion.LocalName)
                .First()
                .Value = Dataset.Version;
        }

        private static void SetOrderedChangeCount(XContainer orderChangelog)
        {
            GetOrderLocalname(orderChangelog)
                    .Attribute(XmlAttributes.Count).Value =
                Config.OrderedChangeCount;
        }

        private static void SetDatasetId(XContainer orderChangelog)
        {
            orderChangelog.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;
        }

        private static void SetSubscriberLastIndex(XContainer orderChangelog)
        {
            GetOrderLocalname(orderChangelog)
                    .Attribute(XmlAttributes.StartIndex).Value =
                (Dataset.SubscriberLastIndex + 1).ToString();
        }

        private static XElement GetOrderLocalname(XContainer orderChangelog)
        {
            return orderChangelog.Descendants(Provider.GeosynchronizationNamespace + XmlElements.Order.LocalName)
                .First();
        }
    }
}