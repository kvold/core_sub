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
                (Dataset.SubscriberLastIndex + 1).ToString();

            orderChangelog.Descendants(Config.GeosynchronizationNs + "datasetId").First().Value = Dataset.Id;

            // ReSharper disable once PossibleNullReferenceException
            orderChangelog.Descendants(Config.GeosynchronizationNs + "order").First().Attribute("count").Value =
                "1000";

            var responseContent = SoapRequest.Send(action, orderChangelog);

            Dataset.OrderedChangelogId =
                Convert.ToInt64(responseContent.Descendants(Config.GeosynchronizationNs + "changelogId").First().Value);

            // ReSharper disable once PossibleNullReferenceException
            Provider.ConfigFileXml.Descendants("dataset")
                    .First(d => d.Attribute("datasetId")?.Value == Dataset.Id)
                    .Descendants("abortedChangelog").First().Attribute("changelogId").Value =
                Dataset.OrderedChangelogId.ToString();

            Provider.Save();
        }
    }
}