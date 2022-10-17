using BlazeDocX.Models;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using Xceed.Words.NET;
using Xceed.Workbooks.NET;

namespace BlazeDocX.Services
{
    public class WorkBookCreator
    {
        private const int size = 150;
        private readonly IJSRuntime jsRuntime;
        private const double SPACING_BEFORE = 5d;
        public WorkBookCreator(IJSRuntime _jsRuntime)
        {
            jsRuntime = _jsRuntime;
        }
        public async Task CreateWorkbook(Accounting accounting)
        {
            using (var workbook = Workbook.Create("First"))
            {
                // Get the first worksheet. A workbook contains at least 1 worksheet.
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Incomes";

                // Add a title.
                worksheet.Cells["A1"].Value = "Incomes";
                worksheet.Cells["A1"].Style.Font = new Font() { Bold = true, Size = 15.5d };
                worksheet.Cells["A1"].Style.Font.Color = System.Drawing.Color.Green;
                
                worksheet.MergedCells.Add("A1", "D1");

                worksheet.Rows[2].Cells[0].Value = "Category";
                worksheet.Rows[2].Cells[1].Value = "Date";
                worksheet.Rows[2].Cells[2].Value = "Amount";
                worksheet.Columns["C"].Style.CustomFormat = "0.00";
                worksheet.Rows[2].Cells[3].Value = "Details";
                worksheet.Rows[2].Style.Font.Bold = true;
                string bcell = worksheet.Rows[2].Cells[2].Address;
                string endcell = worksheet.Rows[2].Cells[2].Address;
                for (int i = 1; i <= accounting.Incomes.Count; i++)
                {
                    worksheet.Rows[2 + i].Cells[0].Value = accounting.Incomes[i-1].Category.ToString();
                    worksheet.Rows[2 + i].Cells[1].Value = ((DateTime)accounting.Incomes[i - 1].Date).ToString("d", DateTimeFormatInfo.CurrentInfo);
                    worksheet.Rows[2 + i].Cells[2].Value = accounting.Incomes[i - 1].Amount;
                    worksheet.Rows[2 + i].Cells[3].Value = accounting.Incomes[i - 1].Details;
                    if (i == accounting.Incomes.Count)
                    {
                        endcell = worksheet.Rows[2 + i].Cells[2].Address;
                    }
                }
                worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Formula = $"=SUM( {bcell}:{endcell} )";
                worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Style.Font = new Font() { Bold = true};
                worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Style.CustomFormat = "0.00";
                // Generate stats.
                //var rnd = new Random();
                //var northDivisionTeams = new Dictionary<string, Data>()
                //{
                //      { "Montreal Canadiens", GetStats(rnd) },
                //      { "Toronto Maple Leafs", GetStats(rnd) },
                //      { "Edmonton Oilers", GetStats(rnd) },
                //      { "Calgary Flames", GetStats(rnd) },
                //      { "Winnipeg Jets", GetStats(rnd) },
                //      { "Ottawa Senators", GetStats(rnd) },
                //      { "Vancouver Canucks", GetStats(rnd) }
                //};

                //// Fill cells values and row styles/size.
                //worksheet.Rows[4].Cells[2].Value = "2021 NHL Standings";
                //worksheet.Rows[4].Style.Font.Bold = true;
                //worksheet.Rows[4].Height = 50;

                //worksheet.Rows[5].Cells[0].Value = "North Division";
                //worksheet.Rows[6].Cells[1].Value = "Pts";
                //worksheet.Rows[6].Cells[2].Value = "Wins";
                //worksheet.Rows[6].Cells[3].Value = "%";
                //worksheet.Rows[6].Cells[4].Value = "In Playoffs";
                //worksheet.Rows[6].Cells[5].Value = "Last Win";
                //worksheet.Rows[6].Style.Alignment.Horizontal = HorizontalAlignment.Center;

                //var teams = northDivisionTeams.OrderByDescending(entry => entry.Value.Pts);
                //for (int i = 0; i < teams.Count(); ++i)
                //{
                //    var team = teams.ElementAt(i);
                //    worksheet.Rows[7 + i].Cells[0].Value = team.Key;
                //    worksheet.Rows[7 + i].Cells[1].Value = team.Value.Pts;
                //    worksheet.Rows[7 + i].Cells[2].Value = team.Value.Wins;
                //    worksheet.Rows[7 + i].Cells[3].Value = team.Value.Percent;
                //    worksheet.Rows[7 + i].Cells[4].Value = (i <= 3);
                //    worksheet.Rows[7 + i].Cells[5].Value = team.Value.LastWin;
                //}

                //// Set the style display format for all cells in column "D".
                //worksheet.Columns["D"].Style.CustomFormat = "0.000";

                //// AutoFit all columns with content, from row 6 to 13, with a minimum width of 0 and a maximum width of 255.
                //worksheet.Columns.AutoFit(0, 255, 6, 13);

                //// Set Outline and inside borders for CellRange A7 to F14.
                //worksheet.Cells["A7", "F14"].Style.Borders.SetOutline(LineStyle.Medium, System.Drawing.Color.Blue);
                //worksheet.Cells["A7", "F14"].Style.Borders.SetInside(LineStyle.Medium, System.Drawing.Color.Blue);

                //// Set Fill for CellRange A7 to F7
                //worksheet.Cells["A7", "F7"].Style.Fill.BackgroundColor = System.Drawing.Color.Orange;

                // Save the created workbook.
                using MemoryStream memStream = new();
                workbook.SaveAs(memStream);
                await jsRuntime.InvokeVoidAsync("blazeDocX.downloadStream", memStream.GetBuffer(), $"Test.xlsx");
                //Console.WriteLine("\tCreated: CreateWorkbook.xlsx\n");
            }
        }

        #region Private Methods

        static Data GetStats(Random rnd)
        {
            var wins = rnd.Next(0, 57);
            var pts = (wins * 2) + rnd.Next(0, 56 - wins) / 2;
            var percent = Convert.ToDouble(wins) / 56d;
            var lastWin = new DateTime(2021, rnd.Next(3, 5), rnd.Next(1, 31));

            return new Data()
            {
                Pts = pts,
                Wins = wins,
                Percent = percent,
                LastWin = lastWin
            };
        }

        #endregion

        #region Private Classes

        private struct Data
        {
            public int Pts;
            public int Wins;
            public double Percent;
            public DateTime LastWin;
        };

        #endregion
    }
}
