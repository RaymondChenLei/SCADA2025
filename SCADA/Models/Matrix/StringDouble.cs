using System.Windows.Media;

namespace SCADA.Models.Matrix
{
    public class StringDouble
    {
        public string Title { get; set; }
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double ActualValue { get; set; }
        public Brush ActualColor { get; set; } = Brushes.White;
    }
}