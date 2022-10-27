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
        private const double SPACING_BEFORE = 5d;

        public CVCreator(IJSRuntime _jsRuntime)
        {
            jsRuntime = _jsRuntime;
        }

        public async Task CreateDoc(CV cv)
        {
            var docX = DocX.Create("profile.docx");

            //await InsertPicture(p, profile.Picture);
            //var t = SetTransparentBorders(docX.InsertTable(3, 3));
            InsertParagraph(docX, cv.FullName.ToString().ToUpper(), "Swiss Light 10pt", 14, true, Alignment.center);

            InsertParagraph(docX, cv.Occupation.ToString(), "Arial", 10, true, Alignment.center);

            var email = InsertParagraph(docX, cv.Email.ToString().ToLower(), "Arial", 10, true, Alignment.center);
            email.InsertHorizontalLine(HorizontalBorderPosition.bottom, BorderStyle.Tcbs_single, size: 12, space: 10);

            AddResumeSummary(cv, docX);

            AddWorkExperience(cv, docX);

            AddEducation(cv, docX);

            AddSkills(cv, docX);

            AddReferences(cv, docX);
            using MemoryStream memStream = new();
            docX.SaveAs(memStream);
            await jsRuntime.InvokeVoidAsync("blazeDocX.downloadStream", memStream.GetBuffer(), $"CV - {cv.FullName}.docx");

            docX.Dispose();
        }

        private static void AddReferences(CV cv, DocX docX)
        {
            if (cv.References.Count > 0)
            {
                var references_header = InsertParagraph(docX, "REFERENCES", "Arial", 10, true, Alignment.center);
                references_header.SpacingBefore(SPACING_BEFORE);
                var bulletedReferencelist = docX.AddList(new ListOptions() { ListType = ListItemType.Bulleted });
                Formatting listReferenceformat = new Formatting();
                listReferenceformat.FontFamily = new Font("Arial");
                listReferenceformat.Size = 10;
                foreach (var reference in cv.References)
                {
                    bulletedReferencelist.AddListItem($"{reference.PersonName}: {reference.CompanyName} {reference.Occupation} ({reference.Email})", 0, listReferenceformat);
                }
                docX.InsertList(bulletedReferencelist);
            }
        }

        private static void AddSkills(CV cv, DocX docX)
        {
            if (cv.Skills.Count > 0)
            {
                var skills_header = InsertParagraph(docX, "SKILLS", "Arial", 10, true, Alignment.center);
                skills_header.SpacingBefore(SPACING_BEFORE);

                var keywords = cv.Skills.GroupBy(w => w.Tag).Select(g => new { keyword = g.Key, Tag = g.Select(c => c.Tag) });

                foreach (var item in keywords)
                {
                    InsertParagraph(docX, item.keyword.ToString().ToUpper(), "Arial", 10, false, Alignment.left);
                    var bulletedTaglist = docX.AddList(new ListOptions() { ListType = ListItemType.Bulleted });
                    Formatting listTagformat = new Formatting();
                    listTagformat.FontFamily = new Font("Arial");
                    listTagformat.Size = 10;

                    foreach (var skill in cv.Skills.Where(s => s.Tag == item.keyword.ToString()).ToList())
                    {
                        bulletedTaglist.AddListItem($"{skill.Name}: {skill.Description}", 0, listTagformat);
                    }
                    docX.InsertList(bulletedTaglist);
                }
            }
        }

        private static void AddEducation(CV cv, DocX docX)
        {
            if (cv.Educations.Count > 0)
            {
                var education_header = InsertParagraph(docX, "EDUCATION", "Arial", 10, true, Alignment.center);
                education_header.SpacingBefore(SPACING_BEFORE);

                var t_education = docX.AddTable(cv.Educations.Count, 3);
                t_education.Design = TableDesign.None;
                t_education.Alignment = Alignment.both;
                for (int i = 0; i < cv.Educations.Count; i++)
                {
                    t_education.Rows[i].Cells[0].Paragraphs[0].Append($"{cv.Educations[i].Institute.ToUpper()}, {cv.Educations[i].InstituteCountry.ToUpper()}").Bold();
                    t_education.Rows[i].Cells[0].Paragraphs[0].InsertParagraphAfterSelf(cv.Educations[i].Degree);
                    t_education.Rows[i].Cells[1].Paragraphs[0].Append("Date Started").Bold().Alignment = Alignment.right;
                    t_education.Rows[i].Cells[2].Paragraphs[0].Append("Date Ended").Bold().Alignment = Alignment.right;
                    if (cv.Educations[i].IsCurrent)
                    {
                        t_education.Rows[i].Cells[1].Paragraphs[0].InsertParagraphAfterSelf($"{((DateTime)cv.Educations[i].StartDate).ToString("dd/MM/yyyy")}");
                        t_education.Rows[i].Cells[1].Paragraphs[1].Alignment = Alignment.right;
                        t_education.Rows[i].Cells[2].Paragraphs[0].InsertParagraphAfterSelf($"CURRENT");
                        t_education.Rows[i].Cells[2].Paragraphs[1].Alignment = Alignment.right;
                    }
                    else
                    {
                        t_education.Rows[i].Cells[1].Paragraphs[0].InsertParagraphAfterSelf($"{((DateTime)cv.Educations[i].StartDate).ToString("dd/MM/yyyy")}");
                        t_education.Rows[i].Cells[1].Paragraphs[1].Alignment = Alignment.right;
                        t_education.Rows[i].Cells[2].Paragraphs[0].InsertParagraphAfterSelf($"{((DateTime)cv.Educations[i].EndDate).ToString("dd/MM/yyyy")}");
                        t_education.Rows[i].Cells[2].Paragraphs[1].Alignment = Alignment.right;
                    }
                }
                t_education.SetTableCellMargin(TableCellMarginType.bottom, 5);
                t_education.SetWidthsPercentage(new float[] { 70, 20, 10 });
                docX.InsertTable(t_education);
            }
        }

        private static void AddWorkExperience(CV cv, DocX docX)
        {
            if (cv.WorkExperiences.Count > 0)
            {
                var experience_header = InsertParagraph(docX, "WORK EXPERIENCE", "Arial", 10, true, Alignment.center);
                experience_header.SpacingBefore(SPACING_BEFORE);

                var t_WorkExperiences = docX.AddTable(cv.WorkExperiences.Count, 3);
                t_WorkExperiences.Design = TableDesign.None;
                t_WorkExperiences.Alignment = Alignment.both;
                for (int i = 0; i < cv.WorkExperiences.Count; i++)
                {
                    t_WorkExperiences.Rows[i].Cells[0].Paragraphs[0].Append($"{cv.WorkExperiences[i].CompanyName.ToUpper()}, {cv.WorkExperiences[i].CompanyCountry.ToUpper()}").Bold();
                    t_WorkExperiences.Rows[i].Cells[0].Paragraphs[0].InsertParagraphAfterSelf(cv.WorkExperiences[i].Occupation);
                    t_WorkExperiences.Rows[i].Cells[1].Paragraphs[0].Append("Date Started").Bold().Alignment = Alignment.right;
                    t_WorkExperiences.Rows[i].Cells[2].Paragraphs[0].Append("Date Ended").Bold().Alignment = Alignment.right;
                    if (cv.WorkExperiences[i].IsCurrent)
                    {
                        t_WorkExperiences.Rows[i].Cells[1].Paragraphs[0].InsertParagraphAfterSelf($"{((DateTime)cv.WorkExperiences[i].StartDate).ToString("dd/MM/yyyy")}");
                        t_WorkExperiences.Rows[i].Cells[1].Paragraphs[1].Alignment = Alignment.right;
                        t_WorkExperiences.Rows[i].Cells[2].Paragraphs[0].InsertParagraphAfterSelf($"CURRENT");
                        t_WorkExperiences.Rows[i].Cells[2].Paragraphs[1].Alignment = Alignment.right;
                    }
                    else
                    {
                        t_WorkExperiences.Rows[i].Cells[1].Paragraphs[0].InsertParagraphAfterSelf($"{((DateTime)cv.WorkExperiences[i].StartDate).ToString("dd/MM/yyyy")}");
                        t_WorkExperiences.Rows[i].Cells[1].Paragraphs[1].Alignment = Alignment.right;
                        t_WorkExperiences.Rows[i].Cells[2].Paragraphs[0].InsertParagraphAfterSelf($"{((DateTime)cv.WorkExperiences[i].EndDate).ToString("dd/MM/yyyy")}");
                        t_WorkExperiences.Rows[i].Cells[2].Paragraphs[1].Alignment = Alignment.right;
                    }
                }
                t_WorkExperiences.SetTableCellMargin(TableCellMarginType.bottom, 5);
                t_WorkExperiences.SetWidthsPercentage(new float[] { 70, 20, 10 });
                docX.InsertTable(t_WorkExperiences);
            }
        }

        private static void AddResumeSummary(CV cv, DocX docX)
        {
            if (cv.ResumeSummary.Count > 0)
            {
                var summary_header = InsertParagraph(docX, "RESUME SUMMARY", "Arial", 10, true, Alignment.center);
                summary_header.SpacingBefore(SPACING_BEFORE);

                var bulletedlist = docX.AddList(new ListOptions() { ListType = ListItemType.Bulleted });
                Formatting listformat = new Formatting();
                listformat.FontFamily = new Font("Arial");
                listformat.Size = 10;
                foreach (var summary in cv.ResumeSummary)
                {
                    bulletedlist.AddListItem(summary.Summary.ToString(), 0, listformat);
                }
                docX.InsertList(bulletedlist);
            }
        }

        private static Paragraph InsertParagraph(DocX docX, string paragraph, string fontfamilyname, double fontsize, bool bold = false, Alignment alignment = Alignment.left)
        {
            var fullname = docX.InsertParagraph(paragraph);
            fullname.Alignment = alignment;
            fullname.Bold(bold);
            fullname.FontSize(fontsize);
            fullname.Font(fontfamilyname);
            return fullname;
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
