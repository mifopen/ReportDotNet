using System.Drawing;
using Shouldly;
using Xunit;
using static ReportDotNet.Core.Factories;

namespace ReportDotNet.Core.Tests
{
    public class CellBuilderTest
    {
        [Fact]
        public void Simple()
        {
            var cellBuilder = Cell()
                              .Alignment(Alignment.Center)
                              .BackgroundColor(Color.Blue)
                              .Bold()
                              .BorderSize(7)
                              .Borders(Borders.Bottom | Borders.Right)
                              .Borders(top: BorderStyle.DotDash, bottom: BorderStyle.DotDotDash)
                              .FontSize(26)
                              .Margin(left: 3, right: 9)
                              .MergeDown()
                              .MergeUp()
                              .SpaceBetweenLines(1.15)
                              .TextDirection(TextDirection.LeftRight_TopBottom)
                              .VerticalAlignment(VerticalAlignment.Center)
                              .Width(32)
                              .Add("some text");
            var cellParameters = cellBuilder.Build().Parameters;
            cellParameters.Width.ShouldBe(32);
            cellParameters.BackgroundColor.ShouldBe(Color.Blue);
            cellParameters.BorderSize.ShouldBe(7);
            cellParameters.Borders.ShouldBe(Borders.Bottom | Borders.Right);
            cellParameters.TopBorderStyle.ShouldBe(BorderStyle.DotDash);
            cellParameters.LeftBorderStyle.ShouldBe(BorderStyle.Single);
            cellParameters.BottomBorderStyle.ShouldBe(BorderStyle.DotDotDash);
            cellParameters.RightBorderStyle.ShouldBe(BorderStyle.Single);
            cellParameters.MarginLeft.ShouldBe(3);
            cellParameters.MarginRight.ShouldBe(9);
            cellParameters.MergeDown.ShouldBeTrue();
            cellParameters.MergeUp.ShouldBeTrue();
            cellParameters.TextDirection.ShouldBe(TextDirection.LeftRight_TopBottom);
            cellParameters.VerticalAlignment.ShouldBe(VerticalAlignment.Center);
            cellParameters.Width.ShouldBe(32);
            var paragraphParameters = cellParameters.Paragraph.Parameters;
            paragraphParameters.Alignment.ShouldBe(Alignment.Center);
            paragraphParameters.FontSize.ShouldBe(26);
            paragraphParameters.Bold.ShouldBeTrue();
            paragraphParameters.SpaceBetweenLines.ShouldBe(1.15);
            paragraphParameters.Parts
                               .ShouldHaveSingleItem()
                               .ShouldBeOfType<TextPart>()
                               .Text.ShouldBe("some text");
        }
    }
}