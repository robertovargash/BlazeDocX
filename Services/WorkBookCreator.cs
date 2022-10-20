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
        private readonly IJSRuntime jsRuntime;
        public WorkBookCreator(IJSRuntime _jsRuntime)
        {
            jsRuntime = _jsRuntime;
        }
        public async Task CreateWorkbook(Accounting accounting)
        {
            using (var workbook = Workbook.Create("First"))
            {
                // Get the first worksheet. A workbook contains at least 1 worksheet.
                ExportIncome(accounting, workbook);
                ExportExpense(accounting, workbook);
                ///COMMENT AND UNCOMMENT THIS FOR ISSUE
                //worksheet.Columns.AutoFit(0, 255, 0, 13);

                // Save the created workbook.
                using MemoryStream memStream = new();
                workbook.SaveAs(memStream);
                await jsRuntime.InvokeVoidAsync("blazeDocX.downloadStream", memStream.GetBuffer(), $"Accounting.xlsx");
            }
        }

        private static void ExportIncome(Accounting accounting, Workbook workbook)
        {
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
                worksheet.Rows[2 + i].Cells[0].Value = accounting.Incomes[i - 1].Category.ToString();
                worksheet.Rows[2 + i].Cells[1].Value = ((DateTime)accounting.Incomes[i - 1].Date).ToString("d", DateTimeFormatInfo.CurrentInfo);
                worksheet.Rows[2 + i].Cells[2].Value = accounting.Incomes[i - 1].Amount;
                worksheet.Rows[2 + i].Cells[3].Value = accounting.Incomes[i - 1].Details;
                if (i == accounting.Incomes.Count)
                {
                    endcell = worksheet.Rows[2 + i].Cells[2].Address;
                }
            }
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Formula = $"=SUM( {bcell}:{endcell} )";
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Style.Font = new Font() { Bold = true };
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Style.CustomFormat = "0.00";
        }

        private static void ExportExpense(Accounting accounting, Workbook workbook)
        {
            workbook.Worksheets.Add();
            var worksheet = workbook.Worksheets[1];
            worksheet.Name = "Expenses";

            // Add a title.
            worksheet.Cells["A1"].Value = "Expenses";
            worksheet.Cells["A1"].Style.Font = new Font
            {
                Bold = true,
                Size = 15.5d,
                Color = System.Drawing.Color.Red
            };

            worksheet.MergedCells.Add("A1", "D1");

            worksheet.Rows[2].Cells[0].Value = "Category";
            worksheet.Rows[2].Cells[1].Value = "Date";
            worksheet.Rows[2].Cells[2].Value = "Amount";
            worksheet.Columns["C"].Style.CustomFormat = "0.00";
            worksheet.Rows[2].Cells[3].Value = "Details";
            worksheet.Rows[2].Style.Font.Bold = true;
            string bcell = worksheet.Rows[2].Cells[2].Address;
            string endcell = worksheet.Rows[2].Cells[2].Address;
            for (int i = 1; i <= accounting.Expenses.Count; i++)
            {
                worksheet.Rows[2 + i].Cells[0].Value = accounting.Expenses[i - 1].Category.ToString();
                worksheet.Rows[2 + i].Cells[1].Value = ((DateTime)accounting.Expenses[i - 1].Date).ToString("d", DateTimeFormatInfo.CurrentInfo);
                worksheet.Rows[2 + i].Cells[2].Value = accounting.Expenses[i - 1].Amount;
                worksheet.Rows[2 + i].Cells[3].Value = accounting.Expenses[i - 1].Details;
                if (i == accounting.Expenses.Count)
                {
                    endcell = worksheet.Rows[2 + i].Cells[2].Address;
                }
            }
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Formula = $"=SUM( {bcell}:{endcell} )";
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Style.Font = new Font() { Bold = true };
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Style.CustomFormat = "0.00";
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
