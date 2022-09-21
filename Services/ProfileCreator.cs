using BlazeDocX.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.Xml.Linq;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace BlazeDocX.Services
{
    public class ProfileCreator

    {
        private const int size = 150;
        private readonly IJSRuntime jsRuntime;

        public ProfileCreator(IJSRuntime _jsRuntime)
        {
            jsRuntime = _jsRuntime;
        }

        public async Task Create(Profile profile)
        {

            var docX = DocX.Create("profile.docx");

            //await InsertPicture(p, profile.Picture);
            var t = SetTransparentBorders(docX.InsertTable(3, 3));

            t.MergeCellsInColumn(0, 0, 1);
            t.SetColumnWidth(0, size);

            var firstRow = t.Rows[0];
            firstRow.Height = size/2;

            var pictureCell = SetTransparentBorders(firstRow.Cells[0]);
            pictureCell.VerticalAlignment = VerticalAlignment.Center;
            
            pictureCell.InsertContent(@$"<img src=""{profile.Picture}"">", ContentType.Html);

            System.Drawing.Color darkGray = System.Drawing.Color.DarkGray;

            SetTransparentBorders(t.Rows[1].Cells[0]);

            SetTransparentBorders(firstRow.Cells[1]).Paragraphs[0]
                .Append("First name:")
                .FontSize(13)
                .UnderlineStyle(UnderlineStyle.singleLine)
                .Bold()
                .Color(darkGray)
                .AppendLine(profile.FirstName);

            SetTransparentBorders(firstRow.Cells[2]).Paragraphs[0]
                .Append("Last name:")
                .FontSize(13)
                .UnderlineStyle(UnderlineStyle.singleLine)
                .Bold()
                .Color(darkGray)
                .AppendLine(profile.LastName);

            var secondRow = t.Rows[1];
            secondRow.Height = size / 2;

            SetTransparentBorders(secondRow.Cells[1]).Paragraphs[0]
                .Append("E-mail:")
                .FontSize(13)
                .UnderlineStyle(UnderlineStyle.singleLine)
                .Bold()
                .Color(darkGray)
                .AppendLine(profile.Email);

            SetTransparentBorders(t.Rows[1].Cells[2]).Paragraphs[0]
                .Append("Phone number:")
                .FontSize(13)
                .UnderlineStyle(UnderlineStyle.singleLine)
                .Bold()
                .Color(darkGray)
                .AppendLine(profile.PhoneNumber);

            var row = t.Rows[2];
            row.MergeCells(0, 2);

            SetTransparentBorders(row.Cells[0]).Paragraphs[0]
                .Append("Resume:")
                .FontSize(13)
                .UnderlineStyle(UnderlineStyle.singleLine)
                .Bold()
                .Color(darkGray)
                .AppendLine(profile.Resume);

            using MemoryStream memStream = new();
            docX.SaveAs(memStream);
            await jsRuntime.InvokeVoidAsync("blazeDocX.downloadStream", memStream.GetBuffer(), $"Profile - {profile.FullName}.docx");

            docX.Dispose();
        }

        public async Task<string?> GetImageBase64(IBrowserFile? profilePhoto)
        {
            //if (profilePhoto?.OpenReadStream() is { } stream)
            //{
            //    var imageBuffer = await GetCroppedImageBuffer(stream);
            //    var imgBase64 = Convert.ToBase64String(imageBuffer);
            //    return $"data:image/jpeg;base64,{imgBase64}";
            //}
            //return null;
            var resizedFile = await profilePhoto.RequestImageFileAsync(profilePhoto.ContentType, 640, 480); // resize the image file    
            var buf = new byte[resizedFile.Size]; // allocate a buffer to fill with the file's data
            using (var stream = resizedFile.OpenReadStream())
            {
                await stream.ReadAsync(buf); // copy the stream to the buffer
            }
            var imgBase64 = Convert.ToBase64String(buf);
            return $"data:image/jpeg;base64,{imgBase64}";

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

        private static async Task<byte[]> GetCroppedImageBuffer(Stream imageStream)
        {
            var img = await SixLabors.ImageSharp.Image.LoadAsync(imageStream);

            (float width, float height) = (size, size);

            if (img.Height > img.Width)
            {
                var aspectRatio = (float)img.Width / img.Height;
                height = size / aspectRatio;
                width = size;
            }
            else if (img.Width > img.Height)
            {

                var aspectRatio = (float)img.Height / img.Width;
                width = size / aspectRatio;
                height = size;
            }

            (int cropX, int cropY) = width > height 
                // Width is larger
                ? ((int)(width - height) / 2, 0)
                : width < height 
                    // Height is larger
                    ? (0, (int)(height - width) / 2) 
                    : (0, 0);

            img.Mutate(i => i.Resize((int)width, (int)height).Crop(new SixLabors.ImageSharp.Rectangle(cropX, cropY, size, size)));
            using MemoryStream imageMemory = new();
            img.SaveAsPng(imageMemory);
            return imageMemory.GetBuffer();
        }

        //    private async Task InsertPicture(DocX docX, IBrowserFile? browserPicture)
        //    {
        //        Picture? picture = null;
        //        if (browserPicture is not null)
        //        {
        //            using var photoStream = browserPicture.OpenReadStream();
        //            using var picture1 = new MemoryStream();

        //            await photoStream.CopyToAsync(picture1);

        //            DotNetStreamReference reference = new(picture1);
        //            picture = docX.AddImage(reference.Stream).CreatePicture();

        //            (float width, float height) = (picture.Width, picture.Height);
        //            (float cropLeft, float cropTop, float cropRight, float cropBottom) = (0, 0, 0, 0);
        //            var minSide = Math.Min(picture.Width, picture.Height);

        //            if (width != height)
        //            {
        //                if (minSide == width)
        //                {
        //                    //fix height to keep ratio
        //                    var increase = minSide / height * 100;
        //                    width = 100;
        //                    height = 100 + increase;
        //                    cropTop = cropBottom = increase / 2;
        //                }
        //                else
        //                {
        //                    //fix width to keep ratio
        //                    var increase = minSide / width * 100;
        //                    height = 100;
        //                    width = 100 + increase;
        //                    cropLeft = cropRight = increase / 2;
        //                }
        //            }
        //            else
        //            {
        //                width = height = 100;
        //            }

        //            picture.Cropping.Left = cropLeft;
        //            picture.Cropping.Right = cropRight;
        //            picture.Cropping.Top = cropTop;
        //            picture.Cropping.Bottom = cropBottom;
        //            picture.Width = width;
        //            picture.Height = height;
        //            picture.SetPictureShape(BasicShapes.ellipse);


        //            p.InsertPicture(picture);
        //        }
        //    }
    }
}
