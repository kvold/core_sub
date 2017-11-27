using System;
using System.Linq;
using System.Threading.Tasks;

namespace CORESubscriber.SoapAction
{
    public class GetChangelogStatus
    {
        public static void Run()
        {
            const string action = "GetChangelogStatus";

            var getChangelogStatus = SoapRequest.GetSoapContentByAction(action);

            getChangelogStatus.Descendants(Config.GeosynchronizationNs + "changelogId").First().Value =
                Dataset.OrderedChangelogId.ToString();

            var queryCounter = 0;

            while (true)
            {
                queryCounter++;

                var responseContent = SoapRequest.Send(action, getChangelogStatus);

                var returnValue = responseContent.Descendants(Config.GeosynchronizationNs + "return").First().Value;

                Console.WriteLine("Query " + queryCounter + ": changelog with ID " + Dataset.OrderedChangelogId + " is " + returnValue);

                switch (returnValue)
                {
                    case "working":
                        Task.Delay(3000).Wait();
                        continue;
                    case "finished":
                        return;
                    default:
                        throw new Exception("Status for changelog with ID " + Dataset.OrderedChangelogId + ": " +
                                            returnValue);
                }
            }
        }
    }
}