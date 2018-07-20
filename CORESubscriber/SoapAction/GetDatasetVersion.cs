using System;
using System.Linq;
using CORESubscriber.Xml;

namespace CORESubscriber.SoapAction
{
    internal class GetDatasetVersion
    {
        public static string Run()
        {
            var getDatasetVersion = SoapRequest.GetSoapContentByAction(SoapActions.GetDatasetVersion);

            getDatasetVersion.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            return SoapRequest.Send(SoapActions.GetDatasetVersion, getDatasetVersion)
                .Descendants(Provider.GeosynchronizationNamespace + XmlElements.Return.LocalName).First().Value;
        }
    }
}