using System;
using System.Linq;

namespace CORESubscriber.SoapAction
{
    internal class GetLastIndex
    {
        public static bool Run()
        {
            const string action = "GetLastIndex";

            var getLastIndex = SoapRequest.GetSoapContentByAction(action);

            getLastIndex.Descendants(Config.GeosynchronizationNs + "datasetId").First().Value = Dataset.Id;

            Dataset.ProviderLastIndex = Convert.ToInt64(SoapRequest.Send(action, getLastIndex)
                .Descendants(Config.GeosynchronizationNs + "return").First().Value);

            Console.WriteLine("Provider LastIndex: " + Dataset.ProviderLastIndex + ", Subscriber Lastindex: " + Dataset.SubscriberLastIndex);

            return Dataset.ProviderLastIndex > Dataset.SubscriberLastIndex;
        }
    }
}