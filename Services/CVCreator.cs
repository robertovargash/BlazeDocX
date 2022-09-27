using BlazeDocX.Models;
using Microsoft.JSInterop;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace BlazeDocX.Services
{
    public class CVCreator
    {
        private const int size = 150;
        private readonly IJSRuntime jsRuntime;

        public CVCreator(IJSRuntime _jsRuntime)
        {
            jsRuntime = _jsRuntime;
        }

        public async Task Create(CV cv)
        {

            var docX = DocX.Create("profile.docx");

            //await InsertPicture(p, profile.Picture);
            //var t = SetTransparentBorders(docX.InsertTable(3, 3));
            var fullname = docX.InsertParagraph(cv.FullName.ToString().ToUpper());
            fullname.Alignment = Alignment.center;
            fullname.Bold();
            fullname.FontSize(14);
            fullname.Font("Swiss Light 10pt");

            var occupation = docX.InsertParagraph("Software Engineer");
            occupation.Alignment = Alignment.center;
            occupation.Bold();
            occupation.FontSize(10);
            occupation.Font("Arial");

            var email = docX.InsertParagraph(cv.Email.ToString().ToLower());
            email.Alignment = Alignment.center;
            email.Bold();
            email.FontSize(10);
            email.Font("Arial");
            email.InsertHorizontalLine(HorizontalBorderPosition.bottom, BorderStyle.Tcbs_single,size:12,space:10);
            

            var summary_header = docX.InsertParagraph("RESUME SUMMARY");
            summary_header.Alignment = Alignment.center;
            summary_header.Bold();
            summary_header.FontSize(10);
            summary_header.Font("Arial");
            summary_header.SpacingBefore(3);


            var bulletedlist = docX.AddList(new ListOptions() { ListType = ListItemType.Bulleted });

            Formatting listformat = new Formatting();
            listformat.FontFamily = new Font("Arial");
            listformat.Size = 10;

            foreach (var summary in cv.ResumeSummary)
            {
                bulletedlist.AddListItem(summary.Summary.ToString(), 0, listformat);
            }
            docX.InsertList(bulletedlist);
            //t.MergeCellsInColumn(0, 0, 1);
            //t.SetColumnWidth(0, size);

            //var firstRow = t.Rows[0];
            //firstRow.Height = size / 2;

            //System.Drawing.Color darkGray = System.Drawing.Color.DarkGray;

            //SetTransparentBorders(t.Rows[1].Cells[0]);

            //SetTransparentBorders(firstRow.Cells[1]).Paragraphs[0]
            //    .Append(cv.FullName)
            //    .FontSize(16)
            //    //.UnderlineStyle(UnderlineStyle.singleLine)
            //    .Bold()
            //    .Color(System.Drawing.Color.Black)
            //    .AppendLine().Alignment = Alignment.center;

            //SetTransparentBorders(firstRow.Cells[2]).Paragraphs[0]
            //    .Append("Last name:")
            //    .FontSize(13)
            //    .UnderlineStyle(UnderlineStyle.singleLine)
            //    .Bold()
            //    .Color(darkGray)
            //    .AppendLine(cv.LastName);

            //var secondRow = t.Rows[1];
            //secondRow.Height = size / 2;

            //SetTransparentBorders(secondRow.Cells[1]).Paragraphs[0]
            //    .Append("E-mail:")
            //    .FontSize(13)
            //    .UnderlineStyle(UnderlineStyle.singleLine)
            //    .Bold()
            //    .Color(darkGray)
            //    .AppendLine(cv.Email);            

            using MemoryStream memStream = new();
            docX.SaveAs(memStream);
            await jsRuntime.InvokeVoidAsync("blazeDocX.downloadStream", memStream.GetBuffer(), $"CV - {cv.FullName}.docx");

            docX.Dispose();
        }

        private static Cell SetTransparentBorders(Cell cell)
        {
            System.Drawing.Color transparent = System.Drawing.Color.Transparent;

            Border noBorder = new(BorderStyle.Tcbs_nil, BorderSize.one, 0, transparent);
            cell.SetBorder(TableCellBorderType.Left, noBorder);
            cell.SetBorder(TableCellBorderType.Right, noBorder);
            cell.SetBorder(TableCellBorderType.Top, noBorder);
            cell.SetBorder(TableCellBorderType.Bottom, noBorder);
            return cell;
        }

        private static Table SetTransparentBorders(Table cell)
        {
            Border noBorder = new(BorderStyle.Tcbs_nil, BorderSize.one, 0, System.Drawing.Color.Transparent);
            cell.SetBorder(TableBorderType.Left, noBorder);
            cell.SetBorder(TableBorderType.Right, noBorder);
            cell.SetBorder(TableBorderType.Top, noBorder);
            cell.SetBorder(TableBorderType.Bottom, noBorder);
            return cell;
        }

    }
}
