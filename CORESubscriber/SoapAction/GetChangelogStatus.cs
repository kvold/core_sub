using System.Linq;

namespace CORESubscriber.SoapAction
{
    public class GetChangelogStatus
    {
        public static bool Run()
        {
            const string action = "GetChangelogStatus";

            var getChangelogStatus = SoapRequest.GetSoapContentByAction(action);

            getChangelogStatus.Descendants(Config.GeosynchronizationNs + "changelogId").First().Value =
                Provider.OrderedChangelogId.ToString();

            var responseContent = SoapRequest.Send(action, getChangelogStatus);

            return responseContent.Descendants(Config.GeosynchronizationNs + "return").First().Value == "working";
        }
    }
}