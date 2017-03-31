using System.Collections.Generic;

namespace ReportDotNet.Core
{
	public class ParagraphParameters
	{
		public int? FontSize { get; set; }
		public Alignment? Alignment { get; set; }
		public bool Bold { get; set; }
		public IEnumerable<Part> Parts { get; set; } = new Part[0];
		public double? SpaceBetweenLines { get; set; }
	}
}