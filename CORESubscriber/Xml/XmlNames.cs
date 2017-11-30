using System.Xml.Linq;

namespace CORESubscriber.Xml
{
    internal class XmlNames
    {
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

            public static XName Count = "count";
        }
    }
}