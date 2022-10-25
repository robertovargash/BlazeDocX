using BlazeDocX.Models;
using Microsoft.AspNetCore.Components;
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
            using (var workbook = Workbook.Create("Accounting"))
            {
                // Get the first worksheet. A workbook contains at least 1 worksheet.
                ExportIncome(accounting, workbook);
                ExportExpense(accounting, workbook);
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
            string calc_endcell = worksheet.Rows[2].Cells[2].Address;

            int i = 1;
            foreach (var a_item in accounting.Incomes.OrderBy(c=>c.Date))
            {
                worksheet.Rows[2 + i].Cells[0].Value = a_item.Category.ToString();
                worksheet.Rows[2 + i].Cells[1].Value = ((DateTime)a_item.Date).ToString("d", DateTimeFormatInfo.CurrentInfo);
                worksheet.Rows[2 + i].Cells[2].Value = a_item.Amount;
                worksheet.Rows[2 + i].Cells[3].Value = a_item.Details;
                if (i < accounting.Incomes.Count)
                {
                    i++;
                }                
            }
            calc_endcell = worksheet.Rows[2 + i].Cells[2].Address;
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Formula = $"=SUM( {bcell}:{calc_endcell} )";
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Style.Font = new Font() { Bold = true };
            worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[2].Style.CustomFormat = "0.00";

            string table_endcell = worksheet.Rows[2 + accounting.Incomes.Count + 1].Cells[3].Address;
            worksheet.Cells["A3", table_endcell].Style.Borders.SetOutline(LineStyle.Medium, System.Drawing.Color.Green);
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

            int i = 1;
            foreach (var e_item in accounting.Expenses.OrderBy(c => c.Date))
            {
                worksheet.Rows[2 + i].Cells[0].Value = e_item.Category.ToString();
                worksheet.Rows[2 + i].Cells[1].Value = ((DateTime)e_item.Date).ToString("d", DateTimeFormatInfo.CurrentInfo);
                worksheet.Rows[2 + i].Cells[2].Value = e_item.Amount;
                worksheet.Rows[2 + i].Cells[3].Value = e_item.Details;
                if (i < accounting.Expenses.Count)
                {
                    i++;
                }
            }
            endcell = worksheet.Rows[2 + i].Cells[2].Address;
            worksheet.Rows[2 + accounting.Expenses.Count + 1].Cells[2].Formula = $"=SUM( {bcell}:{endcell} )";
            worksheet.Rows[2 + accounting.Expenses.Count + 1].Cells[2].Style.Font = new Font() { Bold = true };
            worksheet.Rows[2 + accounting.Expenses.Count + 1].Cells[2].Style.CustomFormat = "0.00";

            string table_endcell = worksheet.Rows[2 + accounting.Expenses.Count + 1].Cells[3].Address;            

            worksheet.Cells["A3", table_endcell].Style.Borders.SetOutline(LineStyle.Medium, System.Drawing.Color.Red);
        }
    }
}
