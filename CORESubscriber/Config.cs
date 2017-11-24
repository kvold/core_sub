using System.IO;
using System.Xml.Linq;

namespace CORESubscriber
{
    public class Config
    {
        internal const string XmlMediaType = "text/xml";

        internal static readonly XNamespace OwsNs = "http://www.opengis.net/ows/1.1";

        internal static readonly XNamespace GeosynchronizationNs =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        internal static readonly XNamespace ChangelogNs =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/endringslogg";

        internal static readonly XNamespace WfsNs = "http://www.opengis.net/wfs/2.0";

        internal static string DownloadFolder = Path.GetTempPath();

        internal static string ErrorLog = "errorLog.txt";
    }
}