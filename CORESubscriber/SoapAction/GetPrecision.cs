using System;
using System.Linq;
using CORESubscriber.Xml;

namespace CORESubscriber.SoapAction
{
    internal class GetPrecision
    {
        public static Precision Run()
        {
            var getPrecision = SoapRequest.GetSoapContentByAction(SoapActions.GetPrecision);

            getPrecision.Descendants(XmlNamespaces.Geosynchronization + XmlAttributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            var returnValue = SoapRequest.Send(SoapActions.GetPrecision, getPrecision)
                .Descendants(XmlNamespaces.Geosynchronization + XmlElements.Return.LocalName).First();

            var precision = new Precision
            {
                Tolerance = Convert.ToDouble(returnValue.Descendants(XmlNamespaces.Geosynchronization + XmlAttributes.Tolerance.LocalName).First().Value),
                EpsgCode = returnValue.Descendants(XmlNamespaces.Geosynchronization + XmlAttributes.EpsgCode.LocalName).First().Value,
                Decimals = returnValue.Descendants(XmlNamespaces.Geosynchronization + XmlAttributes.Decimals.LocalName).First().Value
            };

            return precision;
        }
    }

    internal class Precision
    {
        public string Decimals { get; set; }

        public double Tolerance { get; set; }
    
        public string EpsgCode { get; set; }
    }
}