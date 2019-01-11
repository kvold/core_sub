using System.Linq;
using System.Xml.Linq;
using CORESubscriber.Xml;

namespace CORESubscriber.SoapAction
{
    internal class GetPrecision
    {
        public static Precision Run()
        {
            var returnValue = GetReturnValue();

            return new Precision
            {
                Tolerance = GetTolerance(returnValue),
                EpsgCode = GetEpsgCode(returnValue),
                Decimals = GetDecimals(returnValue)
            };
        }

        private static double GetTolerance(XContainer returnValue)
        {
            return double.Parse(returnValue.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.Tolerance.LocalName).First().Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static XElement GetReturnValue()
        {
            return SoapRequest.Send(SoapActions.GetPrecision, CreateGetPrecisionDocument())
                .Descendants(Provider.GeosynchronizationNamespace + XmlElements.Return.LocalName).First();
        }

        private static XDocument CreateGetPrecisionDocument()
        {
            var getPrecisionDocument = SoapRequest.GetSoapContentByAction(SoapActions.GetPrecision);

            getPrecisionDocument.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.DatasetId.LocalName).First()
                .Value = Dataset.Id;

            return getPrecisionDocument;
        }

        private static string GetDecimals(XContainer returnValue)
        {
            return returnValue.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.Decimals.LocalName).First().Value;
        }

        private static string GetEpsgCode(XContainer returnValue)
        {
            return returnValue.Descendants(Provider.GeosynchronizationNamespace + XmlAttributes.EpsgCode.LocalName).First().Value;
        }
    }

    internal class Precision
    {
        public string Decimals { get; set; }

        public double Tolerance { get; set; }
    
        public string EpsgCode { get; set; }
    }
}