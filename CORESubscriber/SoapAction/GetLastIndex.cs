using System;
using System.Linq;

namespace CORESubscriber.SoapAction
{
    internal class GetLastIndex
    {
        public static bool Run()
        {
            ReadConfig();

            const string action = "GetLastIndex";

            var getLastIndex = SoapRequest.GetSoapContentByAction(action);

            getLastIndex.Descendants(Config.GeosynchronizationNs + "datasetId").First().Value = Provider.DatasetId;

            var providerLastIndex = Convert.ToInt64(SoapRequest.Send(action, getLastIndex)
                .Descendants(Config.GeosynchronizationNs + "return").First().Value);

            return providerLastIndex > Provider.SubscriberLastIndex;
        }

        private static void ReadConfig()
        {
            var configFile = Provider.ReadConfigFile();

            var provider = configFile.Descendants("provider").First(p => p.Attribute("uri")?.Value == Provider.ApiUrl);

            Provider.Password = provider.Attribute("password")?.Value;

            Provider.User = provider.Attribute("user")?.Value;

            var dataset = provider.Descendants().First(d => d.Attribute("datasetId")?.Value == Provider.DatasetId);

            Provider.SubscriberLastIndex = Convert.ToInt64(dataset.Attribute("subscriberLastindex")?.Value);
        }
    }
}