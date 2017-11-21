using System;
using System.Linq;
using System.Threading.Tasks;

namespace CORESubscriber.SoapAction
{
    public class GetChangelogStatus
    {
        public static void Run()
        {
            while (true)
            {
                const string action = "GetChangelogStatus";

                var getChangelogStatus = SoapRequest.GetSoapContentByAction(action);

                getChangelogStatus.Descendants(Config.GeosynchronizationNs + "changelogId").First().Value = Provider.OrderedChangelogId.ToString();

                var responseContent = SoapRequest.Send(action, getChangelogStatus);

                var returnValue = responseContent.Descendants(Config.GeosynchronizationNs + "return").First().Value;

                Console.WriteLine("Status for changelog with ID " + Provider.OrderedChangelogId + ": " + returnValue);

                switch (returnValue)
                {
                    case "working":
                        Task.Delay(3000).Wait();
                        continue;
                    case "finished":
                        break;
                    default:
                        throw new Exception("Status for changelog with ID " + Provider.OrderedChangelogId + ": " + returnValue);
                }
            }
        }
    }
}