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

            getDatasetVersion.Descendants(XmlNamespaces.Geosynchronization + XmlAttributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            return SoapRequest.Send(SoapActions.GetDatasetVersion, getDatasetVersion)
                .Descendants(XmlNamespaces.Geosynchronization + XmlElements.Return.LocalName).First().Value;
        }
    }
}