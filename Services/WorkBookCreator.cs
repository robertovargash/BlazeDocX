using BlazeDocX.Models;
using BlazeDocX.Pages.Accounting;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Drawing;
using System.Globalization;
using Xceed.Words.NET;
using Xceed.Workbooks.NET;
using Font = Xceed.Workbooks.NET.Font;

namespace BlazeDocX.Services
{
    public class WorkBookCreator
    {
        private readonly IJSRuntime jsRuntime;
        private const int main_title_row = 1;
        private const int subtitle_row = 2;
        private const int header_row = 3;

        private const int category_list_column = 0;
        private const int date_list_column = 1;
        private const int ammount_list_column = 2;

        private const int category_cat_column = 4;
        private const int ammount_cat_column = 5;

        private const int date_date_column = 7;
        private const int ammount_date_column = 8;

        private static readonly System.Drawing.Color expense_color = System.Drawing.Color.FromArgb(255, 23, 68);
        private static readonly System.Drawing.Color income_color = System.Drawing.Color.FromArgb(76, 175, 80);

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

            // Add main title.
            AddTitleColored(worksheet, income_color, "A1", "I1", "Incomes");

            // Add first subtitle.
            AddTitleColored(worksheet, income_color, "A3", "C3", "Income list");
            ListAllData(worksheet, income_color, accounting.Incomes);

            // Add first subtitle.
            AddTitleColored(worksheet, income_color, "E3", "F3", "By categories");
            TotalIncomesCategory(accounting, income_color, worksheet);

            // Add first subtitle.
            AddTitleColored(worksheet, income_color, "H3", "I3", "By dates");
            TotalByDates(income_color, worksheet, accounting.Incomes);
        }

        private static void ExportExpense(Accounting accounting, Workbook workbook)
        {            
            workbook.Worksheets.Add();
            var worksheet = workbook.Worksheets[1];
            worksheet.Name = "Expenses";

            // Add main title.
            AddTitleColored(worksheet, expense_color, "A1", "I1", "Expenses");

            AddTitleColored(worksheet, expense_color, "A3", "C3", "Expense list");
            ListAllData(worksheet, expense_color, accounting.Expenses);

            // Add first subtitle.
            AddTitleColored(worksheet, expense_color, "E3", "F3", "By categories");
            TotalExpensesCategory(accounting, expense_color, worksheet);

            // Add first subtitle.
            AddTitleColored(worksheet, expense_color, "H3", "I3", "By dates");
            TotalByDates(expense_color, worksheet, accounting.Expenses);
        }

        private static void ListAllData(Worksheet worksheet, System.Drawing.Color color, IEnumerable<AccountModel> income_expense)
        {
            FillListHeader(worksheet);
            FillListData(worksheet, income_expense);            
            FillListSum(worksheet, color, income_expense.Count(), ammount_list_column, "A3");
        }

        private static void FillListSum(Worksheet worksheet, System.Drawing.Color color, int count_elements, int ammount_column, string begin_decorator_cell)
        {
            string begin_cell = worksheet.Rows[header_row + 1].Cells[ammount_column].Address;
            string calc_endcell = worksheet.Rows[header_row + count_elements].Cells[ammount_column].Address;
            worksheet.Rows[header_row + count_elements + 1].Cells[ammount_column].Formula = $"=SUM( {begin_cell}:{calc_endcell} )";
            worksheet.Rows[header_row + count_elements + 1].Cells[ammount_column].Style.Font = new Font() { Bold = true };
            worksheet.Rows[header_row + count_elements + 1].Cells[ammount_column].Style.CustomFormat = "0.00";
            string table_endcell = worksheet.Rows[header_row + count_elements + 1].Cells[ammount_column].Address;
            worksheet.Cells[begin_decorator_cell, table_endcell].Style.Borders.SetOutline(LineStyle.Medium, color);
        }

        private static void FillListData(Worksheet worksheet, IEnumerable<AccountModel> income_expense)
        {            
            int i = 1;            
            foreach (var item in income_expense.OrderBy(c => c.Date))
            {
                if (item != null)
                    if (item is Income)
                        worksheet.Rows[header_row + i].Cells[category_list_column].Value = ((Income)item).Category.ToString();
                    else
                        worksheet.Rows[header_row + i].Cells[category_list_column].Value = ((Expense)item).Category.ToString();
                else
                    worksheet.Rows[header_row + i].Cells[category_list_column].Value = "Other";

                worksheet.Rows[header_row + i].Cells[date_list_column].Value = ((DateTime)item.Date).ToString("d", DateTimeFormatInfo.CurrentInfo);
                worksheet.Rows[header_row + i].Cells[ammount_list_column].Value = item.Amount;
                i++;
            }
        }

        private static void FillListHeader(Worksheet worksheet)
        {
            worksheet.Rows[header_row].Cells[category_list_column].Value = "Category";
            worksheet.Rows[header_row].Cells[date_list_column].Value = "Date";
            worksheet.Rows[header_row].Cells[ammount_list_column].Value = "Amount";
            worksheet.Columns[ammount_list_column].Style.CustomFormat = "0.00";
            worksheet.Rows[header_row].Style.Font.Bold = true;            
        }

        private static void FillCategoryHeader(Worksheet worksheet)
        {
            worksheet.Rows[header_row].Cells[category_cat_column].Value = "Category";
            worksheet.Rows[header_row].Cells[ammount_cat_column].Value = "Amount";
            worksheet.Columns[ammount_cat_column].Style.CustomFormat = "0.00";
            worksheet.Rows[header_row].Style.Font.Bold = true;
        }

        private static void TotalIncomesCategory(Accounting accounting, System.Drawing.Color color, Worksheet worksheet)
        {
            FillCategoryHeader(worksheet);
            var groupedIncome = accounting.Incomes.OrderBy(x => x.Category).GroupBy(inc => inc.Category).Select(groupedIncomeByCat => new { Category = groupedIncomeByCat.Key, Amount = groupedIncomeByCat.Sum(income => income.Amount) }).ToList();

            int i = 1;
            int count_elements = groupedIncome.Count;
            foreach (var item in groupedIncome)
            {
                worksheet.Rows[header_row + i].Cells[category_cat_column].Value = item.Category.ToString();
                worksheet.Rows[header_row + i].Cells[ammount_cat_column].Value = item.Amount;
                if (i < count_elements)
                {
                    i++;
                }
            }
            FillListSum(worksheet, color, count_elements, ammount_cat_column, "E3");
        }

        private static void TotalByDates(System.Drawing.Color color, Worksheet worksheet, IEnumerable<AccountModel> incomes_expenses)
        {
            worksheet.Rows[header_row].Cells[date_date_column].Value = "Date";
            worksheet.Rows[header_row].Cells[ammount_date_column].Value = "Amount";
            worksheet.Columns[ammount_date_column].Style.CustomFormat = "0.00";

            worksheet.Rows[header_row].Style.Font.Bold = true; 
            var grouped = incomes_expenses.OrderBy(x => x.Date).GroupBy(inc => inc.Date.Value.Date).Select(groupedIncomeByDate =>
                     new
                     {
                         Date = groupedIncomeByDate.Key, Amount = groupedIncomeByDate.Sum(monthIncome => monthIncome.Amount),
                     }).ToList();

            int i = 1;
            int count_elements = grouped.Count;
            foreach (var item in grouped)
            {
                worksheet.Rows[header_row + i].Cells[date_date_column].Value = item.Date.ToString("d");
                worksheet.Rows[header_row + i].Cells[ammount_date_column].Value = item.Amount;
                if (i < count_elements)
                {
                    i++;
                }
            }
            FillListSum(worksheet, color, count_elements, ammount_date_column, "H3");
        }

        private static void TotalExpensesCategory(Accounting accounting, System.Drawing.Color color, Worksheet worksheet)
        {
            FillCategoryHeader(worksheet);
            var groupedExpenses = accounting.Expenses.OrderBy(x => x.Category).GroupBy(inc => inc.Category).Select(groupedIncomeByCat => new { Category = groupedIncomeByCat.Key, Amount = groupedIncomeByCat.Sum(income => income.Amount) }).ToList();

            int i = 1;
            int count_elements = groupedExpenses.Count;
            foreach (var item in groupedExpenses)
            {
                worksheet.Rows[header_row + i].Cells[category_cat_column].Value = item.Category.ToString();
                worksheet.Rows[header_row + i].Cells[ammount_cat_column].Value = item.Amount;
                if (i < count_elements)
                {
                    i++;
                }
            }
            FillListSum(worksheet, color, count_elements, ammount_cat_column, "E3");
        }

        private static void AddTitleColored(Worksheet worksheet, System.Drawing.Color color, string cell_from, string cell_end, string title)
        {
            worksheet.Cells[cell_from].Value = title;
            worksheet.Cells[cell_from].Style.Font = new Font
            {
                Bold = true,
                Size = 15.5d,
                Color = color
            };
            worksheet.MergedCells.Add(cell_from, cell_end);
        }

    }
}
