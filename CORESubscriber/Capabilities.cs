using System.Collections.Generic;

namespace CORESubscriber
{
    internal class Capabilities
    {
        internal static readonly List<string> Fields =
            new List<string>
            {
                Config.Attributes.DatasetId.LocalName,

                "name",

                "version",

                Config.Attributes.ApplicationSchema.LocalName
            };
    }
}