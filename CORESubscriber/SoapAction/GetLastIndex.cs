using System;
using System.Linq;
using CORESubscriber.Xml;

namespace CORESubscriber.SoapAction
{
    internal class GetLastIndex
    {
        public static bool Run()
        {
            const string action = "GetLastIndex";

            var getLastIndex = SoapRequest.GetSoapContentByAction(action);

            getLastIndex.Descendants(XmlNamespaces.Geosynchronization + XmlNames.Attributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            Dataset.ProviderLastIndex = Convert.ToInt64(SoapRequest.Send(action, getLastIndex)
                .Descendants(XmlNamespaces.Geosynchronization + XmlNames.Elements.Return.LocalName).First().Value);

            Console.WriteLine("Provider LastIndex: " + Dataset.ProviderLastIndex + ", Subscriber Lastindex: " +
                              Dataset.SubscriberLastIndex);

            return Dataset.ProviderLastIndex > Dataset.SubscriberLastIndex;
        }
    }
}