namespace CORESubscriber
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var soapAction = Config.ReadArgs(args);
            soapAction.Run(args);
        }
    }
}