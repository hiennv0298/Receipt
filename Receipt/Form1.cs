using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace Receipt
{
    public partial class frmReceipt : Form
    {
        public int NumberOrder { get; set; } = 1;
        public frmReceipt()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFoodName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text) || !int.TryParse(txtPrice.Text, out _) || nmuQuantity.Value <= 0)
            {
                MessageBox.Show("Nhập đủ tên món, giá, số lượng", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var price = int.TryParse(txtPrice.Text, out int p) ? p : 0;
            var total = int.TryParse(nmuQuantity.Text, out int q) ? q * price : 0;
            string[] item = { NumberOrder.ToString(), txtFoodName.Text, nmuQuantity.Value.ToString(), price.ToString("N0"), total.ToString("N0") };
            lsvReceipt.Items.Add(new ListViewItem(item));
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtOrderNumber.Text) || (!int.TryParse(txtDeposit.Text, out _) && !string.IsNullOrWhiteSpace(txtDeposit.Text)))
            {
                MessageBox.Show("Nhập đúng thông tin khách hàng", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set the paper size for the receipt (80mm width)
            PaperSize paperSize = new PaperSize("CustomReceipt", Convert.ToInt32(80 * 0.03937 * 100), 32767); // 0.03937 inch per mm
            printDocument1.DefaultPageSettings.PaperSize = paperSize;
            printDocument1.PrintPage += (s, d) => DrawListViewItems(d.Graphics, lsvReceipt.Items);
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument1;
            printPreviewDialog.PrintPreviewControl.Zoom = 1.0;
            DialogResult result = printPreviewDialog.ShowDialog();
            // Start printing
            // printDocument1.Print();

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font boldFont = new Font("Arial", 9, FontStyle.Bold);
            Font boldLargeFont = new Font("Arial", 12, FontStyle.Bold);
            Font boldSmallFont = new Font("Arial", 7, FontStyle.Bold);
            Font regularFont = new Font("Arial", 8);
            SolidBrush brush = new SolidBrush(Color.Black);

            float lineHeight = 16;

            StringFormat centerFormat = new StringFormat
            {
                Alignment = StringAlignment.Center
            };

            // Header content
            string[] headerLines = {
                                    "NHÀ HÀNG NAI VÀNG NEW",
                                    "27 Nguyễn Văn Bá, P. Bình Thọ, TP. Thủ Đức",
                                    "0907 106 391 - 0934 000 597",
                                    "PHIẾU TẠM TÍNH"
                                };

            // Draw each line of the header
            for (int i = 0; i < headerLines.Length; i++)
            {
                float y = i * lineHeight + 15;
                e.Graphics.DrawString(headerLines[i], (i == 0 || i == 3) ? i == 0 ? boldFont : boldLargeFont : regularFont, brush, 157, i == 3 ? y + 10 : y, centerFormat);
            }

            e.Graphics.DrawString($"Số: {txtOrderNumber.Text}", boldSmallFont, brush, 157, 100, centerFormat);
            e.Graphics.DrawString($"Ngày: ", boldFont, brush, 10, 129);
            e.Graphics.DrawString(dtOrderDate.Text, regularFont, brush, 50, 130);
            e.Graphics.DrawString($"Bàn: A33", boldLargeFont, brush, 8, 150);
            e.Graphics.DrawString($"Thu ngân: ", boldFont, brush, 10, 174);
            e.Graphics.DrawString($"Nguyễn Văn Hiền", regularFont, brush, 75, 175);


            // If there is more content to print, set the HasMorePages property to true
            // This will trigger another PrintPage event
            e.HasMorePages = false;
        }

        void DrawListViewItems(Graphics graphics, ListViewItemCollection items)
        {
            // Set up the font and brush for drawing
            Font font = new Font("Arial", 8);
            SolidBrush brush = new SolidBrush(Color.Black);

            // Set up column widths and spacing for 80mm paper width
            float paperWidth = 80; // in mm
            float totalColumnWidth = paperWidth - 20; // Subtracting padding
            float spacing = 8;
            float x = 10; // Initial x-coordinate
            float y = 200; // Initial y-coordinate

            // Calculate column widths based on the available space
            float columnWidth = totalColumnWidth / items[0].SubItems.Count;

            var pen = new Pen(Color.Black);
            graphics.DrawLine(pen, 9, y - 2, 305, y - 2);
            graphics.DrawString("#", font, brush, x + 5, y);
            graphics.DrawLine(pen, x, y - 2, x, y + font.Height + 2);
            graphics.DrawLine(pen, x + 20, y - 2, x + 20, y + font.Height + 2);
            graphics.DrawString("Tên hàng", font, brush, x + 20, y);
            graphics.DrawLine(pen, x + 135, y - 2, x + 135, y + font.Height + 2);
            graphics.DrawString("SL", font, brush, x + 140, y);
            graphics.DrawLine(pen, x + 165, y - 2, x + 165, y + font.Height + 2);
            graphics.DrawString("Đơn giá", font, brush, x + 170, y);
            graphics.DrawLine(pen, x + 225, y - 2, x + 225, y + font.Height + 2);
            graphics.DrawString("TT", font, brush, x + 250, y);
            graphics.DrawLine(pen, x + 295, y - 2, x + 295, y + font.Height + 2);
            graphics.DrawLine(pen, 9, y + font.Height + 2, 305, y + font.Height + 2);


            y += 20;

            //// Draw data rows with gridlines
            //foreach (ListViewItem item in lsvReceipt.Items)
            //{
            //    x = 10; // Reset x-coordinate for each row

            //    for (int i = 0; i < item.SubItems.Count; i++)
            //    {
            //        // Draw vertical line for each column
            //        graphics.DrawLine(pen, x, y, x, y + font.Height);

            //        // Draw data text
            //        graphics.DrawString(item.SubItems[i].Text, font, brush, x, y);

            //        x += columnWidth + spacing;
            //    }

            //    // Draw horizontal line below the data row
            //    graphics.DrawLine(pen, 10, y + font.Height, x, y + font.Height);

            //    y += font.Height + spacing; // Move to the next row
            //}
        }
    }
}
