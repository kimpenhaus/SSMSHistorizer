namespace Dev.IL.Engineering.SSMSHistorizer
{
    using System.Reflection;

    using NLog;
    using NLog.Config;

    internal sealed class LogManager
    {
        public static readonly LogFactory Instance = new LogFactory(new XmlLoggingConfiguration(LogManager.GetNLogConfigFilePath()));

        private static string GetNLogConfigFilePath()
        {
            return string.Format("{0}.config", Assembly.GetExecutingAssembly().Location);
        } 
    }
}