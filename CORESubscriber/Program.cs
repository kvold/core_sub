using System;
using System.Collections.Generic;
using CORESubscriber.SoapAction;

namespace CORESubscriber
{
    internal class Program
    {
        internal static Dictionary<string, List<string>> Actions = new Dictionary<string, List<string>>
        {
            { "sync", new List<string>{ "configFile", "datasetId" } },
            { "add", new List<string> { "uri", "user", "password"} }
        };


        private static void Main(string[] args)
        {
            if (args.Length > 0) RunAction(args);
            else WriteHelp();
        }

        private static void RunAction(IReadOnlyList<string> args)
        {
            var actionArg = args[0];

            switch (actionArg)
            {
                case "sync":
                    Config.ConfigFileProvider = args[1];
                    Config.DatasetId = args[2];
                    if (GetLastIndex.Run()) OrderChangelog.Run();
                    break;
                case "add":
                    Config.ApiUrl = args[1];
                    Config.User = args[2];
                    Config.Password = args[3];
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