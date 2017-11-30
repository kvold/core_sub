using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CORESubscriber.Xml;

namespace CORESubscriber.SoapAction
{
    public class GetCapabilities
    {
        public static bool Run()
        {
            var getCapabilities = SoapRequest.GetSoapContentByAction(SoapActions.GetCapabilities);

            var responseContent = SoapRequest.Send(SoapActions.GetCapabilities, getCapabilities);

            if (Provider.ConfigFile == null)
                Provider.ConfigFile = responseContent.Descendants(XmlNamespaces.Ows + XmlElements.Title.LocalName)
                                          .First().Value
                                          .Replace(" ", "_") + ".xml";

            var datasetsList = GetDatasets(responseContent);

            Provider.Save(datasetsList);

            Console.WriteLine("Saved " + datasetsList.Count + " datasets to " + Provider.ConfigFile);

            return true;
        }

        private static IList<XElement> GetDatasets(XContainer result)
        {
            var datasetsList = new List<XElement>();

            foreach (var dataset in result
                .Descendants(XmlNamespaces.Geosynchronization + XmlElements.Datasets.LocalName)
                .Descendants())
            {
                var datasetElement = new XElement(XmlElements.Dataset);

                foreach (var field in dataset.Descendants()
                    .Where(d => Capabilities.Fields.Contains(d.Name.LocalName)))
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