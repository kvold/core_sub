using System;
using System.Collections.Generic;
using CORESubscriber.SoapAction;

namespace CORESubscriber
{
    internal class Program
    {
        internal static Dictionary<string, List<string>> Actions = new Dictionary<string, List<string>>
        {
            {"sync", new List<string> {"configFile", "datasetId"}},
            {"add", new List<string> {"uri", "user", "password"}}
        };


        private static void Main(string[] args)
        {
            if (args.Length > 0) RunAction(args);
            else WriteHelp();
        }

        private static void RunAction(IReadOnlyList<string> args)
        {
            switch (args[0])
            {
                case "sync":
                    Provider.ConfigFile = args[1];
                    Provider.DatasetId = args[2];
                    if (GetLastIndex.Run())
                    {
                        OrderChangelog.Run();
                        GetChangelogStatus.Run();
                        Provider.OrderedChangelogDownloadUrl = GetChangelog.Run();
                    }
                    break;
                case "add":
                    Provider.ApiUrl = args[1];
                    Provider.User = args[2];
                    Provider.Password = args[3];
                    GetCapabilities.Run();
                    break;
                default:
                    WriteHelp();
                    break;
            }
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