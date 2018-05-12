namespace ReportDotNet.Web.Examples.StampAndSigns
{
    public class Data
    {
        public byte[] Stamp { get; set; }
        public byte[] BossSign { get; set; }
        public byte[] AccountantSign { get; set; }
        public string BossPosition { get; set; }
        public string BossName { get; set; }
        public string AccountantName { get; set; }
        public bool HasNoAccountant { get; set; }
    }
}