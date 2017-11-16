using System;
using System.Linq;

namespace CORESubscriber.SoapAction
{
    internal class GetLastIndex : ISoapAction
    {
        public bool Run()
        {
            ReadConfig();

            const string action = "GetLastIndex";

            var getLastIndex = SoapRequest.GetSoapContentByAction(action);

            getLastIndex.Descendants(Config.GeosynchronizationNs + "datasetId").First().Value = Config.DatasetId;

            var providerLastIndex = Convert.ToInt64(SoapRequest.Send(action, getLastIndex).Descendants(Config.GeosynchronizationNs + "return").First().Value);

            return providerLastIndex > Config.SubscriberLastIndex;
        }

        private static void ReadConfig()
        {
            var configFile = Config.ReadConfigFile();

            var provider = configFile.Descendants("provider").First(p => p.Attribute("uri")?.Value == Config.ApiUrl);

            Config.Password = provider.Attribute("password")?.Value;

            Config.User = provider.Attribute("user")?.Value;

            var dataset = provider.Descendants().First(d => d.Attribute("datasetId")?.Value == Config.DatasetId);

            Config.SubscriberLastIndex = Convert.ToInt64(dataset.Attribute("lastindex")?.Value);
        }
    }
}