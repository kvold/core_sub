using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CORESubscriber.SoapAction;

namespace CORESubscriber
{
    internal class Program
    {
        internal static Dictionary<string, List<string>> Actions = new Dictionary<string, List<string>>
        {
            {"add", new List<string> {"uri", "user", "password", "outputFile"}},
            {"sync", new List<string> {"configFile", "tempFolder"}}
        };


        private static void Main(string[] args)
        {
            if (args.Length > 0)
                Run(args);

            else
                WriteHelp();
        }

        private static void Run(IReadOnlyList<string> args)
        {
            switch (args[0].ToLower())
            {
                case "sync":

                    SetSyncVariables(args);

                    SynchronzeSubscribedDatasets();

                    break;

                case "add":

                    SetAddVariables(args);

                    GetCapabilities.Run();

                    break;

                default:

                    WriteHelp();

                    break;
            }
        }

        private static void SetSyncVariables(IReadOnlyList<string> args)
        {
            Provider.ConfigFile = args[1];

            if (args.Count > 1) Config.DownloadFolder = args[2];

            Provider.ReadSettings();
        }

        private static void SetAddVariables(IReadOnlyList<string> args)
        {
            Provider.ApiUrl = args[1];

            Provider.User = args[2];

            Provider.Password = args[3];

            if (args.Count > 3) Provider.ConfigFile = args[4];
        }

        private static void SynchronzeSubscribedDatasets()
        {
            foreach (var subscribed in GetSubscribedElements())
            {
                SetDatasetVariables(subscribed);

                if (!GetLastIndex.Run()) continue;

                OrderChangelog.Run();

                GetChangelogStatus.Run();

                Changelog.Get(GetChangelog.Run());

                Changelog.Execute();
            }
        }

        private static void SetDatasetVariables(XObject subscribed)
        {
            Dataset.Id = subscribed.Parent?.Attribute("datasetId")?.Value;

            Dataset.SubscriberLastIndex = Convert.ToInt64(subscribed.Parent?.Attribute("subscriberLastindex")?.Value);
        }

        private static IEnumerable<XElement> GetSubscribedElements()
        {
            return Provider.ConfigFileXml.Descendants("dataset").Descendants("subscribed")
                .Where(s => string.Equals(s.Value.ToString(), bool.TrueString,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        private static void WriteHelp()
        {
            Console.WriteLine("Syntax: action [parameters]");

            Console.WriteLine();

            foreach (var action in Actions)
            {
                Console.WriteLine(action.Key + ":");

                foreach (var parameter in action.Value) Console.WriteLine("\t" + parameter);
            }
        }
    }
}