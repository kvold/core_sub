using System.Linq;

namespace CORESubscriber.SoapAction
{
    public class GetChangelog
    {
        public static string Run()
        {
            const string action = "GetChangelog";

            var getChangelog = SoapRequest.GetSoapContentByAction(action);

            getChangelog.Descendants(Config.GeosynchronizationNs + Config.Attributes.ChangelogId.LocalName).First().Value =
                Dataset.OrderedChangelogId.ToString();

            var responseContent = SoapRequest.Send(action, getChangelog);

            var returnValue = responseContent.Descendants(Config.GeosynchronizationNs + Config.Elements.DownloadUri.LocalName).First().Value;

            return returnValue;
        }
    }
}