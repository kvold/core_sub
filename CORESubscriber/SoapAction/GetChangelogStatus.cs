using System;
using System.Linq;
using System.Threading.Tasks;
using CORESubscriber.Xml;

namespace CORESubscriber.SoapAction
{
    public class GetChangelogStatus
    {
        public static void Run()
        {
            var getChangelogStatus = SoapRequest.GetSoapContentByAction(SoapActions.GetChangelogStatus);

            getChangelogStatus.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.ChangelogId.LocalName)
                    .First().Value =
                Dataset.OrderedChangelogId.ToString();

            var queryCounter = 0;

            while (true)
            {
                queryCounter++;

                var responseContent = SoapRequest.Send(SoapActions.GetChangelogStatus, getChangelogStatus);

                var returnValue = responseContent.Descendants(Provider.GeosynchronizationNamespace + "return").First()
                    .Value;

                Console.WriteLine("Query " + queryCounter + ": changelog with ID " + Dataset.OrderedChangelogId +
                                  " is " + returnValue);

                switch (returnValue)
                {
                    case "working":
                        Task.Delay(Config.StatusQueryDelay).Wait();
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