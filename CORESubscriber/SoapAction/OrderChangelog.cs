using System;
using System.Linq;
// ReSharper disable PossibleNullReferenceException

namespace CORESubscriber.SoapAction
{
    internal class OrderChangelog
    {
        public static void Run()
        {
            Dataset.OrderedChangelogId = Convert.ToInt64(Provider.ConfigFileXml.Descendants("dataset")
                .First(d => d.Attribute("datasetId")?.Value == Dataset.Id)
                .Descendants("abortedChangelog").First().Attribute("changelogId").Value);

            if (Dataset.OrderedChangelogId != -1) return;

            const string action = "OrderChangelog";

            var orderChangelog = SoapRequest.GetSoapContentByAction(action);

            orderChangelog.Descendants(Config.GeosynchronizationNs + "order").First().Attribute("startIndex").Value =
                (Dataset.SubscriberLastIndex + 1).ToString();

            orderChangelog.Descendants(Config.GeosynchronizationNs + "datasetId").First().Value = Dataset.Id;

            orderChangelog.Descendants(Config.GeosynchronizationNs + "order").First().Attribute("count").Value =
                "1000";

            var responseContent = SoapRequest.Send(action, orderChangelog);

            Dataset.OrderedChangelogId =
                Convert.ToInt64(responseContent.Descendants(Config.GeosynchronizationNs + "changelogId").First().Value);

            Provider.ConfigFileXml.Descendants("dataset")
                    .First(d => d.Attribute("datasetId")?.Value == Dataset.Id)
                    .Descendants("abortedChangelog").First().Attribute("changelogId").Value =
                Dataset.OrderedChangelogId.ToString();

            Provider.Save();
        }
    }
}