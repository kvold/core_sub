using System.Linq;
using CORESubscriber.Xml;

namespace CORESubscriber.SoapAction
{
    public class GetChangelog
    {
        public static string Run()
        {
            var getChangelog = SoapRequest.GetSoapContentByAction(SoapActions.GetChangelog);

            getChangelog.Descendants(XmlNamespaces.Geosynchronization + XmlAttributes.ChangelogId.LocalName).First()
                    .Value =
                Dataset.OrderedChangelogId.ToString();

            var responseContent = SoapRequest.Send(SoapActions.GetChangelog, getChangelog);

            var returnValue = responseContent
                .Descendants(XmlNamespaces.Geosynchronization + XmlElements.DownloadUri.LocalName).First().Value;

            return returnValue;
        }
    }
}