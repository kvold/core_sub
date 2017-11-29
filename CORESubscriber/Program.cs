using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CORESubscriber.SoapAction;

namespace CORESubscriber
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                    Run(args);

                else
                    WriteHelp();
            }
            catch (Exception e)
            {
                HandleExceptionText(e);
            }
        }

        private static void HandleExceptionText(Exception e)
        {
            Console.WriteLine(e.Message);

            using (var file = File.Exists(Config.ErrorLog) ? File.Open(Config.ErrorLog, FileMode.Append) : File.Open(Config.ErrorLog, FileMode.CreateNew))
                using (var stream = new StreamWriter(file))
                    stream.WriteLine(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz") + ":\r\n\t" + e.Message + ":\r\n" + e.StackTrace);
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
                if(Dataset.ReadVariables(subscribed))
                    if (!GetLastIndex.Run()) continue;

                OrderChangelog.Run();

                GetChangelogStatus.Run();

                Changelog.Get(GetChangelog.Run()).Wait();

                Changelog.Execute();

                Dataset.UpdateSettings();
            }
        }

        private static IEnumerable<XElement> GetSubscribedElements()
        {
            return Provider.ConfigFileXml.Descendants(Config.Elements.Dataset).Descendants(Config.Elements.Subscribed)
                .Where(s => string.Equals(s.Value.ToString(), bool.TrueString,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        private static void WriteHelp()
        {
            Console.WriteLine("Syntax: action [parameters]");

            Console.WriteLine();

            foreach (var action in Config.Actions)
            {
                Console.WriteLine(action.Key + ":");

                foreach (var parameter in action.Value) Console.WriteLine("\t" + parameter);
            }
        }
    }
}