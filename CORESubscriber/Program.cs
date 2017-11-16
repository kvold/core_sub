namespace CORESubscriber
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Config.ReadArgs(args);
            SoapAction.ISoapAction soapAction = new SoapAction.GetCapabilities();
            soapAction.Run(args);
        }
    }
}