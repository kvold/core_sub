using System.Xml.Linq;

namespace CORESubscriber.Xml
{
    public class XmlNamespaces
    {
        internal static readonly XNamespace Ows = "http://www.opengis.net/ows/1.1";

        internal static readonly XNamespace Geosynchronization =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.2/produkt";

        internal static readonly XNamespace Geosynchronization11 =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        internal static readonly XNamespace Changelog =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/endringslogg";

        internal static readonly XNamespace Wfs = "http://www.opengis.net/wfs/2.0";

        internal static readonly XNamespace Soap = "http://schemas.xmlsoap.org/soap/envelope/";
    }
}