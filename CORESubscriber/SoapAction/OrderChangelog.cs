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

            var responseContent = SoapRequest.Send(action, SetOrderVariables(SoapRequest.GetSoapContentByAction(action)));

            Dataset.OrderedChangelogId =
                Convert.ToInt64(responseContent.Descendants(Config.GeosynchronizationNs + "changelogId").First().Value);

            Provider.ConfigFileXml.Descendants("dataset")
                    .First(d => d.Attribute("datasetId")?.Value == Dataset.Id)
                    .Descendants("abortedChangelog").First().Attribute("changelogId").Value =
                Dataset.OrderedChangelogId.ToString();

            Provider.Save();
        }

        private static XDocument SetOrderVariables(XDocument orderChangelog)
        {
            orderChangelog.Descendants(Config.GeosynchronizationNs + "order").First().Attribute("startIndex").Value =
                (Dataset.SubscriberLastIndex + 1).ToString();

            orderChangelog.Descendants(Config.GeosynchronizationNs + "datasetId").First().Value = Dataset.Id;

            orderChangelog.Descendants(Config.GeosynchronizationNs + "order").First().Attribute("count").Value =
                "1000";

            return orderChangelog;
        }
    }
}