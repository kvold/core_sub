using System.Collections.Generic;
using CORESubscriber.Xml;

namespace CORESubscriber
{
    internal class Capabilities
    {
        internal static readonly List<string> Fields =
            new List<string>
            {
                XmlNames.Attributes.DatasetId.LocalName,

                "name",

                "version",

                XmlNames.Attributes.ApplicationSchema.LocalName
            };
    }
}