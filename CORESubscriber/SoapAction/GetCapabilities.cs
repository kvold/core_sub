using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CORESubscriber.SoapAction
{
    public class GetCapabilities
    {
        public static bool Run()
        {
            const string action = "GetCapabilities";

            var getCapabilities = SoapRequest.GetSoapContentByAction(action);

            var responseContent = SoapRequest.Send(action, getCapabilities);

            if (Provider.ConfigFile == null)
                Provider.ConfigFile = responseContent.Descendants(Config.OwsNs + "Title").First().Value
                                          .Replace(" ", "_") + ".xml";

            var datasetsList = GetDatasets(responseContent);

            Provider.Save(datasetsList);

            Console.WriteLine("Saved " + datasetsList.Count + " datasets to " + Provider.ConfigFile);

            return true;
        }

        private static IList<XElement> GetDatasets(XContainer result)
        {
            var datasetsList = new List<XElement>();

            foreach (var dataset in result.Descendants(Config.GeosynchronizationNs + "datasets").Descendants())
            {
                var datasetElement = new XElement("dataset");
                
                foreach (var field in dataset.Descendants()
                    .Where(d => Dataset.Fields.Contains(d.Name.LocalName)))
                {
                    datasetElement.Add(new XAttribute(field.Name.LocalName.Trim(), field.Value.Trim()));
                }

                if (!datasetElement.Attributes().Any()) continue;

                datasetElement.Add(Dataset.DefaultElements);

                datasetsList.Add(datasetElement);
            }

            return datasetsList;
        }
    }
}