using System;
using System.Collections.Generic;
using System.Globalization;
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

            if (responseContent.Descendants(XmlNamespaces.Soap + XmlElements.Fault.LocalName).Any())
            {
                // TODO: Handle faults here. For example wrong version of contract etc
                Config.Version = Config.Version == XmlNamespaces.Geosynchronization.NamespaceName
                    ? XmlNamespaces.Geosynchronization11.NamespaceName
                    : XmlNamespaces.Geosynchronization.NamespaceName;

                getCapabilities = SoapRequest.GetSoapContentByAction(SoapActions.GetCapabilities);

                responseContent = SoapRequest.Send(SoapActions.GetCapabilities, getCapabilities);
            }

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

                if(Config.Version != XmlNamespaces.Geosynchronization11.NamespaceName) GetVersionAndPrecision(datasetElement);

                datasetsList.Add(datasetElement);
            }

            return datasetsList;
        }

        private static void GetVersionAndPrecision(XElement datasetElement)
        {
            //Debug.Assert(datasetElement != null, nameof(datasetElement) + " != null");

            Dataset.Id = (string) datasetElement.Attribute(XmlAttributes.DatasetId.LocalName);

            //datasetElement.Attribute(XmlAttributes.DatasetVersion).Value = GetDatasetVersion.Run();

            var precision = GetPrecision.Run();

            var datasetPrecision = datasetElement.Descendants(XmlElements.Precision).First();
            datasetPrecision.Attribute(XmlAttributes.Tolerance).Value = precision.Tolerance.ToString(CultureInfo.InvariantCulture);
            datasetPrecision.Attribute(XmlAttributes.Decimals).Value = precision.Decimals;
            datasetPrecision.Attribute(XmlAttributes.EpsgCode).Value = precision.EpsgCode;
        }
    }
}