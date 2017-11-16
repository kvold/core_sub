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

            var fileName = responseContent.Descendants(Config.OwsNs + "Title").First().Value
                               .Replace(" ", "_") + ".xml";

            Provider.ConfigFile = "Providers/" + fileName;

            var datasetsList = GetDatasets(responseContent);

            Provider.Save(datasetsList);

            return true;
        }

        private static IEnumerable<XElement> GetDatasets(XContainer result)
        {
            var datasetsList = new List<XElement>();

            foreach (var dataset in result.Descendants(Config.GeosynchronizationNs + "datasets")
                .Descendants())
            {
                var datasetElement = new XElement("dataset");

                datasetElement.Add(Provider.DatasetDefaults);

                foreach (var field in dataset.Descendants().Where(d => Provider.DatasetFields.Contains(d.Name.LocalName)))
                {
                    datasetElement.Add(new XAttribute(field.Name.LocalName.Trim(), field.Value.Trim()));

                    Console.WriteLine(field.Name.LocalName + ": " + field.Value.Trim());
                }

                if (datasetElement.Attributes().Count() == Provider.DatasetDefaults.Count) continue;

                datasetsList.Add(datasetElement);
            }

            return datasetsList;
        }
    }
}