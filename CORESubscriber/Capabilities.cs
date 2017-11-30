using System.Collections.Generic;
using CORESubscriber.Xml;

namespace CORESubscriber
{
    internal class Capabilities
    {
        internal static readonly List<string> Fields =
            new List<string>
            {
                XmlAttributes.DatasetId.LocalName,

                "name",

                "version",

                XmlAttributes.ApplicationSchema.LocalName
            };
    }
}