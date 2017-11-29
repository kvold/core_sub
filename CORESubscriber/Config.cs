using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace CORESubscriber
{
    public class Config
    {
        internal static Dictionary<string, List<string>> Actions = new Dictionary<string, List<string>>
        {
            {
                "add", new List<string>
                {
                    Attributes.Uri.LocalName,
                    Attributes.User.LocalName,
                    Attributes.Password.LocalName,
                    "outputFile"
                }
            },
            {
                "sync", new List<string>
                {
                    "configFile",
                    "tempFolder"
                }
            }
        };

        internal const string XmlMediaType = "text/xml";

        internal static readonly XNamespace OwsNs = "http://www.opengis.net/ows/1.1";

        internal static readonly XNamespace GeosynchronizationNs = "http://skjema.geonorge.no/standard/geosynkronisering/1.1/produkt";

        internal static readonly XNamespace ChangelogNs =
            "http://skjema.geonorge.no/standard/geosynkronisering/1.1/endringslogg";

        internal static readonly XNamespace WfsNs = "http://www.opengis.net/wfs/2.0";

        internal static string DownloadFolder = Path.GetTempPath();

        internal static string ErrorLog = "errorLog.txt";

        internal static int StatusQueryDelay = 3000;

        internal class Elements
        {
            public static XName Provider = "provider";

            public static XName Dataset = "dataset";

            public static XName Subscribed = "subscribed";

            public static XName AbortedChangelog = "abortedChangelog";

            public static XName WfsClient = "wfsClient";

            public static XName Order = "order";

            public static XName Return = "return";

            public static XName DownloadUri = "downloadUri";

            public static XName Title = "Title";

            public static XName Datasets = "datasets";
        }

        internal class Attributes
        {
            public static XName Password = "password";

            public static XName Namespace = "nameSpace";

            public static XName TargetNamespace = "targetNamespace";

            public static XName Uri = "uri";

            public static XName User = "user";

            public static XName ApplicationSchema = "applicationSchema";

            public static XName DatasetId = "datasetId";

            public static XName Transaction = "transaction";

            public static XName SubscriberLastIndex = "subscriberLastindex";

            public static XName EndIndex = "endIndex";

            public static XName ChangelogPath = "changelogPath";

            public static XName ChangelogId = "changelogId";

            public static XName StartIndex = "startIndex";
        }
    }
}