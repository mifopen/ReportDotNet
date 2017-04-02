using System.Collections.Generic;

namespace ReportDotNet.Core
{
	public class ParagraphParameters
	{
		public int? FontSize { get; set; }
		public Alignment? Alignment { get; set; }
		public bool Bold { get; set; }
		public List<Part> Parts { get; set; } = new List<Part>();
		public double? SpaceBetweenLines { get; set; }
	}
}