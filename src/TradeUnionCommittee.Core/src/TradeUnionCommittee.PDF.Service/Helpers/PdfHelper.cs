﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Reflection;

namespace TradeUnionCommittee.PDF.Service.Helpers
{
    public class PdfHelper
    {
        public readonly Font Font;
        public readonly Font FontBold;
        public readonly Font SignFont;
        public readonly string Сurrency = "грн";

        public PdfHelper()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathToFont = Path.Combine(basePath, "Fonts", "TimesNewRoman.ttf");

            var baseFont = BaseFont.CreateFont(pathToFont, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font = new Font(baseFont, 14, Font.NORMAL);
            FontBold = new Font(baseFont, 12, Font.BOLD);
            SignFont = new Font(baseFont, 8, Font.NORMAL);
        }

        //--------------------------------------------------------------------------------

        public void AddCell(PdfPTable table, int colspan, string value)
        {
            table.AddCell(new PdfPCell(new Phrase(value, Font))
            {
                PaddingTop = 5,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = colspan
            });
        }

        public void AddBoldCell(PdfPTable table, int colspan, string value)
        {
            table.AddCell(new PdfPCell(new Phrase(value, FontBold))
            {
                PaddingTop = 5,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = colspan
            });
        }

        public void AddTitleCell(PdfPTable table, int colspan, string value)
        {
            table.AddCell(new PdfPCell(new Phrase(value, FontBold))
            {
                PaddingTop = 15,
                PaddingBottom = 15,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = colspan
            });
        }

        //--------------------------------------------------------------------------------

        public Paragraph AddParagraph(string str, int alignment)
        {
            return new Paragraph(str, Font) { Alignment = alignment };
        }

        public void AddEmptyParagraph(IElementListener document, int count)
        {
            for (var i = 0; i < count; i++)
            {
                document.Add(new Phrase(Environment.NewLine));
            }
        }
    }
}
