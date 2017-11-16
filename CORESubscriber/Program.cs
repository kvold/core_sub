using System;
using CORESubscriber.SoapAction;

namespace CORESubscriber
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var action = args[0];

            switch (action)
            {
                case "sync":
                    Config.ConfigFileProvider = args[1];
                    Config.DatasetId = args[2];
                    if(GetLastIndex.Run()) OrderChangelog.Run();
                    break;
                case "add":
                    Config.ApiUrl = args[1];
                    Config.User = args[2];
                    Config.Password = args[3];
                    GetCapabilities.Run();
                    break;
                default:
                    throw new NotImplementedException("Action " + action + "not implemented");
            }
        }
    }
}