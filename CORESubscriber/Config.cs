using System.Xml.Linq;

namespace CORESubscriber
{
    public class Config
    {
        internal const string XmlMediaType = "text/xml";

        internal static readonly XNamespace OwsNs = "http://www.opengis.net/ows/1.1";

        internal static readonly XNamespace GeosynchronizationNs =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";
    }
}