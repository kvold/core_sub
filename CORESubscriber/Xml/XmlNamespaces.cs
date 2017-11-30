using System.Xml.Linq;

namespace CORESubscriber.Xml
{
    public class XmlNamespaces
    {
        internal static readonly XNamespace Ows = "http://www.opengis.net/ows/1.1";

        internal static readonly XNamespace Geosynchronization =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        internal static readonly XNamespace Changelog =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/endringslogg";

        internal static readonly XNamespace Wfs = "http://www.opengis.net/wfs/2.0";
    }
}