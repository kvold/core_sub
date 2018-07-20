using System;
using System.Linq;
using CORESubscriber.Xml;

namespace CORESubscriber.SoapAction
{
    internal class GetLastIndex
    {
        public static bool Run()
        {
            var getLastIndex = SoapRequest.GetSoapContentByAction(SoapActions.GetLastIndex);

            getLastIndex.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            Dataset.ProviderLastIndex = Convert.ToInt64(SoapRequest.Send(SoapActions.GetLastIndex, getLastIndex)
                .Descendants(Provider.GeosynchronizationNamespace + XmlElements.Return.LocalName).First().Value);

            Console.WriteLine("Provider LastIndex: " + Dataset.ProviderLastIndex + ", Subscriber Lastindex: " +
                              Dataset.SubscriberLastIndex);

            return Dataset.ProviderLastIndex > Dataset.SubscriberLastIndex;
        }
    }
}