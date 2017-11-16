using System.Linq;

namespace CORESubscriber.SoapAction
{
    internal class GetLastIndex : ISoapAction
    {
        public void Run(string[] args)
        {
            const string action = "GetLastIndex";

            var getLastIndex = SoapRequest.GetSoapContentByAction(action);

            getLastIndex.Descendants(Config.GeosynchronizationNs + "datasetId").First().Value = Config.DatasetId;

            var lastIndex = SoapRequest.Send(action, getLastIndex).Descendants(Config.GeosynchronizationNs + "return").First().Value;
        }
    }
}