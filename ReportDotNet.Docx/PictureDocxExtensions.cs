using System.Drawing;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportDotNet.Core;
using Anchor = DocumentFormat.OpenXml.Drawing.Wordprocessing.Anchor;
using BlipFill = DocumentFormat.OpenXml.Drawing.Pictures.BlipFill;
using NonVisualDrawingProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties;
using NonVisualPictureDrawingProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties;
using NonVisualPictureProperties = DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties;
using Outline = DocumentFormat.OpenXml.Drawing.Outline;
using Picture = ReportDotNet.Core.Picture;
using ShapeProperties = DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties;

namespace ReportDotNet.Docx
{
	internal static class PictureDocxExtensions
	{
		internal static OpenXmlElement Convert(this Picture picture, WordprocessingDocument document)
		{
			var imagePart = document.MainDocumentPart.AddImagePart(ImagePartType.Png);
			imagePart.FeedData(new MemoryStream(picture.Bytes));

			int width, height;
			using (var img = Image.FromStream(new MemoryStream(picture.Bytes)))
			{
				width = img.Width;
				height = img.Height;
			}

			var offsets = FillRectangleOffsetsCalculator.Calculate(new Size(width, height), new Size(picture.MaxWidth, picture.MaxHeight));
			var offset = new Offset { X = 0, Y = 0 };
			if (picture.OffsetX.HasValue)
				offset.X = picture.OffsetX*OpenXmlUnits.EmuPerPixel;
			if (picture.OffsetY.HasValue)
				offset.Y = picture.OffsetY*OpenXmlUnits.EmuPerPixel;

			var testPicture = picture as TestPicture;

			return new Drawing(
				new Anchor(
					new WrapNone(),
					new DocProperties
					{
						Id = 1,
						Name = "name",
						Description = "description"
					},
					new Graphic
					{
						GraphicData = new GraphicData(new DocumentFormat.OpenXml.Drawing.Pictures.Picture
													  {
														  NonVisualPictureProperties = new NonVisualPictureProperties
																					   {
																						   NonVisualDrawingProperties = new NonVisualDrawingProperties
																														{
																															Id = 0,
																															Name = "name"
																														},
																						   NonVisualPictureDrawingProperties = new NonVisualPictureDrawingProperties()
																					   },
														  BlipFill = new BlipFill(new Stretch
																				  {
																					  FillRectangle = new FillRectangle
																									  {
																										  Top = offsets.Top,
																										  Left = offsets.Left,
																										  Bottom = offsets.Bottom,
																										  Right = offsets.Right
																									  }
																				  })
																	 {
																		 Blip = new Blip(testPicture != null
																							 ? new ColorReplacement { RgbColorModelHex = new RgbColorModelHex { Val = testPicture.Color.ToHex() } }
																							 : null)
																				{
																					Embed = document.MainDocumentPart.GetIdOfPart(imagePart),
																				}
																	 },
														  ShapeProperties = new ShapeProperties(new PresetGeometry
																								{
																									Preset = ShapeTypeValues.Rectangle,
																									AdjustValueList = new AdjustValueList()
																								},
																								testPicture != null
																									? new Outline(new SolidFill { RgbColorModelHex = new RgbColorModelHex { Val = testPicture.Color.ToHex() } })
																									: null)
																			{
																				Transform2D = new Transform2D
																							  {
																								  Offset = offset,
																								  Extents = new Extents { Cx = picture.MaxWidth*OpenXmlUnits.EmuPerPixel, Cy = picture.MaxHeight*OpenXmlUnits.EmuPerPixel }
																							  }
																			}
													  })
									  {
										  Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"
									  }
					})
				{
					DistanceFromTop = 0,
					DistanceFromBottom = 0,
					DistanceFromLeft = 0,
					DistanceFromRight = 0,
					SimplePos = false,
					RelativeHeight = 0,
					BehindDoc = false,
					Locked = false,
					LayoutInCell = true,
					AllowOverlap = true,
					SimplePosition = new SimplePosition { X = 0, Y = 0 },
					HorizontalPosition = new HorizontalPosition
										 {
											 RelativeFrom = HorizontalRelativePositionValues.Column,
											 HorizontalAlignment = new HorizontalAlignment("right")
										 },
					VerticalPosition = new VerticalPosition
									   {
										   RelativeFrom = VerticalRelativePositionValues.Paragraph,
										   VerticalAlignment = new DocumentFormat.OpenXml.Drawing.Wordprocessing.VerticalAlignment("top")
									   },
					Extent = new Extent
							 {
								 Cx = picture.MaxWidth*OpenXmlUnits.EmuPerPixel,
								 Cy = picture.MaxHeight*OpenXmlUnits.EmuPerPixel
							 },
					EffectExtent = new EffectExtent
								   {
									   LeftEdge = 0,
									   TopEdge = 0,
									   RightEdge = 0,
									   BottomEdge = 0
								   }
				});
		}
	}
}