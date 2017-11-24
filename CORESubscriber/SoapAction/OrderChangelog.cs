using System;
using System.Linq;

namespace CORESubscriber.SoapAction
{
    internal class OrderChangelog
    {
        public static void Run()
        {
            const string action = "OrderChangelog";

            var orderChangelog = SoapRequest.GetSoapContentByAction(action);

            // ReSharper disable once PossibleNullReferenceException
            orderChangelog.Descendants(Config.GeosynchronizationNs + "order").First().Attribute("startIndex").Value =
                (Provider.SubscriberLastIndex + 1).ToString();

            orderChangelog.Descendants(Config.GeosynchronizationNs + "datasetId").First().Value = Provider.DatasetId;

            // ReSharper disable once PossibleNullReferenceException
            orderChangelog.Descendants(Config.GeosynchronizationNs + "order").First().Attribute("count").Value =
                "1000";

            var responseContent = SoapRequest.Send(action, orderChangelog);

            Provider.OrderedChangelogId =
                Convert.ToInt64(responseContent.Descendants(Config.GeosynchronizationNs + "changelogId").First().Value);

            // ReSharper disable once PossibleNullReferenceException
            Provider.ConfigFileXml.Descendants("dataset").First(d => d.Attribute("datasetId")?.Value == Provider.DatasetId)
                .Descendants("abortedChangelog").First().Attribute("changelogId").Value = Provider.OrderedChangelogId.ToString();

            Provider.Save();
        }
    }
}