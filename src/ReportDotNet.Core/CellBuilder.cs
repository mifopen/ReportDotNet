using System;
using System.Collections.Generic;
using System.Drawing;

namespace ReportDotNet.Core
{
    public class CellBuilder
    {
        private readonly CellParameters parameters = new CellParameters();
        private string text;
        private int? fontSize;
        private Alignment? alignment;
        private bool bold;
        private double? spaceBetweenLines;

        public CellBuilder Width(int? width) => Chain(p => p.Width = width);
        public CellBuilder Add(Paragraph paragraph) => Chain(p => p.Paragraph = paragraph);
        public CellBuilder Add(string text) => Chain(_ => this.text = text);
        public CellBuilder FontSize(int fontSize) => Chain(_ => this.fontSize = fontSize);
        public CellBuilder Alignment(Alignment alignment) => Chain(_ => this.alignment = alignment);
        public CellBuilder Bold(bool bold = true) => Chain(_ => this.bold = bold);
        public CellBuilder SpaceBetweenLines(double? spaceBetweenLines) => Chain(_ => this.spaceBetweenLines = spaceBetweenLines);
        public CellBuilder VerticalAlignment(VerticalAlignment verticalAlignment) => Chain(p => p.VerticalAlignment = verticalAlignment);
        public CellBuilder Borders(Borders borders) => Chain(p => p.Borders = borders);
        public CellBuilder MergeUp(bool mergeUp = true) => Chain(p => p.MergeUp = mergeUp);
        public CellBuilder MergeDown(bool mergeDown = true) => Chain(p => p.MergeDown = mergeDown);
        public CellBuilder TextDirection(TextDirection textDirection) => Chain(p => p.TextDirection = textDirection);
        public CellBuilder BackgroundColor(Color backgroundColor) => Chain(p => p.BackgroundColor = backgroundColor);
        public CellBuilder BorderSize(int borderSize) => Chain(p => p.BorderSize = borderSize);

        public CellBuilder Margin(int? left = null,
                                  int? right = null,
                                  int? top = null,
                                  int? bottom = null) => Chain(p =>
                                                              {
                                                                  p.MarginLeft = left;
                                                                  p.MarginRight = right;
                                                                  p.MarginTop = top;
                                                                  p.MarginBottom = bottom;
                                                                  
                                                              });


        public CellBuilder Borders(BorderStyle left = BorderStyle.Single,
                                   BorderStyle top = BorderStyle.Single,
                                   BorderStyle right = BorderStyle.Single,
                                   BorderStyle bottom = BorderStyle.Single) => Chain(p =>
                                                                                     {
                                                                                         p.LeftBorderStyle = left;
                                                                                         p.TopBorderStyle = top;
                                                                                         p.RightBorderStyle = right;
                                                                                         p.BottomBorderStyle = bottom;
                                                                                     });

        public Cell Build()
        {
            //todo clone parameters?
            parameters.Paragraph = parameters.Paragraph ?? new Paragraph(new ParagraphParameters
                                                                         {
                                                                             Parts = new List<Part> { new TextPart { Text = text } },
                                                                             FontSize = fontSize,
                                                                             Alignment = alignment,
                                                                             Bold = bold,
                                                                             SpaceBetweenLines = spaceBetweenLines
                                                                         });
            return new Cell(parameters);
        }

        public static implicit operator Cell(CellBuilder builder) => builder.Build();

        private CellBuilder Chain(Action<CellParameters> action)
        {
            action(parameters);
            return this;
        }
    }
}