namespace SCADA.Service.Helper
{
    public class NLogHelper
    {
        public NLogHelper()
        {
            Logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public static NLogHelper NLogProcessHelperIns { get; private set; } = new();
        public NLog.Logger Logger { get; set; }
    }
}