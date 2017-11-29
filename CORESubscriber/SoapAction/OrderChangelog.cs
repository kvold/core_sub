using System;
using System.Linq;
using System.Xml.Linq;

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
                    .Descendants(Config.GeosynchronizationNs + Config.Attributes.ChangelogId.LocalName).First().Value);

            Provider.ConfigFileXml.Descendants(Config.Elements.Dataset)
                    .First(d => d.Attribute(Config.Attributes.DatasetId)?.Value == Dataset.Id)
                    .Descendants(Config.Elements.AbortedChangelog).First().Attribute(Config.Attributes.ChangelogId)
                    .Value =
                Dataset.OrderedChangelogId.ToString();

            Provider.Save();
        }

        private static XDocument SetOrderVariables(XDocument orderChangelog)
        {
            orderChangelog.Descendants(Config.GeosynchronizationNs + Config.Elements.Order.LocalName).First()
                    .Attribute(Config.Attributes.StartIndex).Value =
                (Dataset.SubscriberLastIndex + 1).ToString();

            orderChangelog.Descendants(Config.GeosynchronizationNs + Config.Attributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            orderChangelog.Descendants(Config.GeosynchronizationNs + Config.Elements.Order.LocalName).First()
                    .Attribute(Config.Attributes.Count).Value =
                Config.OrderedChangeCount;

            return orderChangelog;
        }
    }
}