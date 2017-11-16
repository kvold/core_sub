using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CORESubscriber.SoapAction
{
    public class GetCapabilities : ISoapAction
    {


        public void Run(string[] args)
        {
            const string action = "GetCapabilities";

            var getCapabilities = SoapRequest.GetSoapContentByAction(action);

            var responseContent = SoapRequest.Send(action, getCapabilities);

            var capabilitiesFileName = "Capabilities/" +
                                       responseContent.Descendants(Config.OwsNs + "Title").First().Value
                                           .Replace(" ", "_") + ".xml";

            responseContent.Save(new FileStream(capabilitiesFileName, FileMode.OpenOrCreate));

            Config.SetProviderDefaults(capabilitiesFileName);

            var datasetsList = GetDatasets(responseContent);

            Config.UpdateConfig(datasetsList);
        }

        private static IEnumerable<XElement> GetDatasets(XContainer result)
        {
            var datasetsList = new List<XElement>();

            foreach (var dataset in result.Descendants(Config.GeosynchronizationNs + "datasets")
                .Descendants())
            {
                var datasetElement = new XElement("dataset");

                datasetElement.Add(Config.DatasetDefaults);

                foreach (var field in dataset.Descendants().Where(d => Config.DatasetFields.Contains(d.Name.LocalName)))
                {
                    datasetElement.Add(new XAttribute(field.Name.LocalName.Trim(), field.Value.Trim()));

                    Console.WriteLine(field.Name.LocalName + ": " + field.Value.Trim());
                }

                if (datasetElement.Attributes().Count() == Config.DatasetDefaults.Count) continue;

                datasetsList.Add(datasetElement);
            }

            return datasetsList;
        }


    }
}