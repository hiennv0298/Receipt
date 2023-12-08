using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace Receipt
{
    public partial class frmReceipt : Form
    {
        public int NumberOrder { get; set; } = 1;
        public float MaxWidthName { get; set; } = 105;
        public float YPos { get; set; } = 200;

        Font boldFont = new Font("Arial", 8, FontStyle.Bold);
        Font boldLargeFont = new Font("Arial", 11, FontStyle.Bold);
        Font boldLargeConfirmFont = new Font("Arial", 9, FontStyle.Bold);
        Font boldSmallFont = new Font("Arial", 6, FontStyle.Bold);
        Font boldSmall2Font = new Font("Arial", 6, FontStyle.Bold);
        Font regularFont = new Font("Arial", 7);
        SolidBrush brush = new SolidBrush(Color.Black);
        Pen pen = new Pen(Color.Black);
        StringFormat centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center
        };

        float lineHeight = 16;


        public frmReceipt()
        {
            InitializeComponent();
            lsvReceipt.FullRowSelect = true;
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
            NumberOrder++;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTable.Text))
            {
                MessageBox.Show("Nhập bàn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCashier.Text))
            {
                MessageBox.Show("Nhập thu ngân", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtDiscount.Text) && !int.TryParse(txtDiscount.Text, out _))
            {
                MessageBox.Show("Nhập mã giảm giá là số", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtDeposit.Text) && !int.TryParse(txtDeposit.Text, out _))
            {
                MessageBox.Show("Nhập đặt cọc là số", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOrderNumber.Text) || !int.TryParse(txtOrderNumber.Text, out int orderNumber))
            {
                MessageBox.Show("Nhập mã hóa đơn là số", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lsvReceipt.Items.Count == 0)
            {
                MessageBox.Show("Chưa thêm món ăn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set the paper size for the receipt (80mm width)
            PaperSize paperSize = new PaperSize("CustomReceipt", Convert.ToInt32(71 * 0.03937 * 100), 32767); // 0.03937 inch per mm
            printDocument1.DefaultPageSettings.PaperSize = paperSize;
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument1;
            printPreviewDialog.PrintPreviewControl.Zoom = 1.0;
            DialogResult result = printPreviewDialog.ShowDialog();
            // Start printing
            printDocument1.Print();
            orderNumber++;
            txtOrderNumber.Text = orderNumber.ToString();
            YPos = 200;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
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
                e.Graphics.DrawString(headerLines[i], (i == 0 || i == 3) ? i == 0 ? boldFont : boldLargeFont : regularFont, brush, 145, i == 3 ? y + 10 : y, centerFormat);
            }

            e.Graphics.DrawString($"Số: {txtOrderNumber.Text}", boldSmallFont, brush, 145, 100, centerFormat);
            e.Graphics.DrawString($"Ngày: ", boldFont, brush, 3, 129);
            e.Graphics.DrawString(dtOrderDate.Text, regularFont, brush, 43, 130);
            e.Graphics.DrawString($"Bàn: {txtTable.Text}", boldLargeFont, brush, 1, 150);
            e.Graphics.DrawString($"Thu ngân: ", boldFont, brush, 3, 174);
            e.Graphics.DrawString($"{txtCashier.Text}", regularFont, brush, 68, 175);

            DrawListViewItems(e, lsvReceipt.Items);

            e.HasMorePages = false;
        }

        void DrawListViewItems(PrintPageEventArgs e, ListViewItemCollection items)
        {
            var graphics = e.Graphics;
            // Set up the font and brush for drawing
            Font font = new Font("Arial", 7);
            Font fontBold = new Font("Arial", 7, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);

            // Set up column widths and spacing for 80mm paper width
            float paperWidth = 71; // in mm
            float totalColumnWidth = paperWidth - 20; // Subtracting padding
            float x = 3; // Initial x-coordinate
            float y = 200; // Initial y-coordinate
            float spacing = 8;
            float yMultiple = 14;

            graphics.DrawLine(pen, 2, y - 2, 280, y - 2);
            graphics.DrawString("#", fontBold, brush, x + 5, y);
            graphics.DrawLine(pen, x, y - 2, x, y + font.Height + 2);
            graphics.DrawLine(pen, x + 20, y - 2, x + 20, y + font.Height + 2);
            graphics.DrawString("Tên hàng", fontBold, brush, x + 20, y);
            graphics.DrawLine(pen, x + 135, y - 2, x + 135, y + font.Height + 2);
            graphics.DrawString("SL", fontBold, brush, x + 140, y);
            graphics.DrawLine(pen, x + 165, y - 2, x + 165, y + font.Height + 2);
            graphics.DrawString("Đơn giá", fontBold, brush, x + 170, y);
            graphics.DrawLine(pen, x + 217, y - 2, x + 217, y + font.Height + 2);
            graphics.DrawString("TT", fontBold, brush, x + 240, y);
            graphics.DrawLine(pen, x + 275, y - 2, x + 275, y + font.Height + 2);
            graphics.DrawLine(pen, 2, y + fontBold.Height + 2, 280, y + font.Height + 2);

            y += 20;
            YPos += 20;
            // Draw data rows with gridlines
            foreach (ListViewItem item in lsvReceipt.Items)
            {
                var isLast = item == lsvReceipt.Items[lsvReceipt.Items.Count - 1];
                float textWidth = MeasureString(item.SubItems[1].Text, font);
                x = 10; // Reset x-coordinate for each row
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            DrawItem(pen, x + 13, y - 5, x + 13, y + font.Height + 2, font, brush, x - 3, y, e, item.SubItems[i].Text, yMultiple, isLast, textWidth > MaxWidthName);
                            break;
                        case 1:
                            DrawItem(pen, x + 128, y - 5, x + 128, y + font.Height + 2, font, brush, x + 15, y, e, item.SubItems[i].Text, yMultiple, isLast, textWidth > MaxWidthName);
                            break;
                        case 2:
                            DrawItem(pen, x + 158, y - 5, x + 158, y + font.Height + 2, font, brush, x + 133, y, e, item.SubItems[i].Text, yMultiple, isLast, textWidth > MaxWidthName);
                            break;
                        case 3:
                            DrawItem(pen, x + 210, y - 5, x + 210, y + font.Height + 2, font, brush, 220 - MeasureString(item.SubItems[i].Text, regularFont), y, e, item.SubItems[i].Text, yMultiple, isLast, textWidth > MaxWidthName);
                            break;
                        case 4:
                            DrawItem(pen, x + 268, y - 5, x + 268, y + font.Height + 2, font, brush, 280 - MeasureString(item.SubItems[i].Text, regularFont), y, e, item.SubItems[i].Text, yMultiple, isLast, textWidth > MaxWidthName);
                            break;
                    }
                }

                // Draw horizontal line below the data row
                if (textWidth > MaxWidthName)
                {
                    graphics.DrawLine(pen, x - 7, y - 5 - yMultiple, x - 7, y + font.Height + 2 + yMultiple);
                    var length = WordWrap(item.SubItems[1].Text, font, MaxWidthName).Length;
                    var height = length * 8;
                    graphics.DrawLine(pen, 3, y + font.Height + 2 + height, 285, y + font.Height + 2 + height);
                    y += font.Height + yMultiple + spacing;
                    YPos += font.Height + yMultiple + spacing;
                }
                else
                {
                    graphics.DrawLine(pen, x - 7, y - 5, x - 7, y + font.Height + 2);
                    graphics.DrawLine(pen, 3, y + font.Height + 2, 285, y + font.Height + 2);
                    y += font.Height + spacing; // Move to the next row
                    YPos += font.Height + spacing; // Move to the next row
                }
            }

            DrawConfirmation(e);
        }

        void DrawItem(Pen pen, float x1Line, float y1Line, float x2Line, float y2Line, Font fontString, SolidBrush brush, float xString, float yString, PrintPageEventArgs e, string text, float yMultiple, bool isLastItem, bool isMultipleLine)
        {
            var graphics = e.Graphics;
            float textWidth = MeasureString(text, fontString);
            if (textWidth > MaxWidthName)
            {
                // Split the text into lines to fit within the maximum width
                string[] lines = WordWrap(text, fontString, MaxWidthName);
                foreach (string line in lines)
                {
                    graphics.DrawString(line, fontString, brush, xString, yString);
                    yString += fontString.GetHeight();
                    graphics.DrawLine(pen, x1Line, y1Line - yMultiple, x2Line, y2Line + yMultiple);
                }
            }
            else
            {
                if (isLastItem)
                {
                    if (isMultipleLine)
                    {
                        graphics.DrawLine(pen, x1Line, y1Line - yMultiple, x2Line, y2Line + yMultiple);
                        graphics.DrawString(text, fontString, brush, xString, yString);
                    }
                    else
                    {
                        graphics.DrawLine(pen, x1Line, y1Line, x2Line, y2Line);
                        graphics.DrawString(text, fontString, brush, xString, yString);
                    }
                }
                else
                {
                    graphics.DrawLine(pen, x1Line, y1Line - yMultiple, x2Line, y2Line + yMultiple);
                    graphics.DrawString(text, fontString, brush, xString, yString);
                }
            }
        }

        void DrawConfirmation(PrintPageEventArgs e)
        {
            var subtotal = CalculateTotal();
            var subTotalStr = subtotal.ToString("N0") + "đ";
            double discount = 0.0;
            double deposit = 0.0;
            YPos += 10;
            e.Graphics.DrawString($"Tiền hàng", boldFont, brush, 3, YPos);
            e.Graphics.DrawString($"{subTotalStr}", boldFont, brush, 278 - MeasureString(subTotalStr, boldFont), YPos);
            YPos += boldFont.Height + 4;
            if (!string.IsNullOrWhiteSpace(txtDiscount.Text) && double.TryParse(txtDiscount.Text, out discount))
            {
                var discountStr = discount.ToString("N0") + "đ";
                e.Graphics.DrawString($"Giảm giá", regularFont, brush, 3, YPos);
                e.Graphics.DrawString($"{discountStr}", regularFont, brush, 278 - MeasureString(discountStr, regularFont), YPos);
                YPos += regularFont.Height + 4;
            }

            if (cbbForm.Text == "Mẫu 1")
            {
                if (chbVip.Checked)
                {
                    var vipStr = chbVip.Checked ? (subtotal * 0.05).ToString("N0") + "đ" : "0đ";
                    e.Graphics.DrawString($"Phí phòng lạnh (5%)", regularFont, brush, 3, YPos);
                    e.Graphics.DrawString($"{vipStr}", regularFont, brush, 278 - MeasureString(vipStr, regularFont), YPos);
                    YPos += regularFont.Height + 8;
                }
            }
            else
            {
                if (chbVip.Checked)
                {
                    var vipStr = chbVip.Checked ? (subtotal * 0.1).ToString("N0") + "đ" : "0đ";
                    e.Graphics.DrawString($"Phí phòng lạnh (10%)", regularFont, brush, 3, YPos);
                    e.Graphics.DrawString($"{vipStr}", regularFont, brush, 278 - MeasureString(vipStr, regularFont), YPos);
                    YPos += regularFont.Height + 8;
                }
            }
            var beforeTax = 0.0;
            var tax = 0.0;

            if (cbbForm.Text == "Mẫu 1")
            {
                beforeTax = chbVip.Checked ? subtotal + subtotal * 0.05 - discount : subtotal - discount;
                var beforeTaxStr = beforeTax.ToString("N0") + "đ";
                e.Graphics.DrawString($"Tiền trước thuế", boldFont, brush, 3, YPos);
                e.Graphics.DrawString($"{beforeTaxStr}", boldFont, brush, 278 - MeasureString(beforeTaxStr, boldFont), YPos);
                YPos += boldFont.Height + 4;

                tax = beforeTax * 0.08;
                var taxStr = tax.ToString("N0") + "đ";
                e.Graphics.DrawString($"Thuế GTGT (8%)", regularFont, brush, 3, YPos);
                e.Graphics.DrawString($"{taxStr}", regularFont, brush, 278 - MeasureString(taxStr, regularFont), YPos);
                YPos += regularFont.Height + 10;
            }

            var total = 0.0;
            if (cbbForm.Text == "Mẫu 1")
            {
                total = beforeTax + tax;
            }
            else
            {
                total = subtotal;
            }

            var totalStr = total.ToString("N0") + "đ";
            e.Graphics.DrawString($"Tổng thanh toán", boldLargeConfirmFont, brush, 3, YPos);
            e.Graphics.DrawString($"{totalStr}", boldLargeConfirmFont, brush, 275 - MeasureString(totalStr, boldLargeConfirmFont), YPos);
            YPos += boldLargeFont.Height + 4;

            if (!string.IsNullOrWhiteSpace(txtDeposit.Text) && double.TryParse(txtDeposit.Text, out deposit))
            {
                var depositStr = deposit.ToString("N0") + "đ";
                e.Graphics.DrawString($"Đặt cọc", regularFont, brush, 3, YPos);
                e.Graphics.DrawString($"{depositStr}", regularFont, brush, 278 - MeasureString(depositStr, regularFont), YPos);
                YPos += regularFont.Height + 4;
            }

            e.Graphics.DrawLine(pen, 2, YPos, 298, YPos);
            YPos += 5;

            var payment = (total - deposit).ToString("N0") + "đ";
            e.Graphics.DrawString($"Còn phải thu", boldFont, brush, 3, YPos);
            e.Graphics.DrawString($"{payment}", boldFont, brush, 278 - MeasureString(payment, boldFont), YPos);
            YPos += boldFont.Height + 4;

            e.Graphics.DrawLine(pen, 2, YPos, 298, YPos);
            YPos += regularFont.Height + 10;

            if (cbbForm.Text == "Mẫu 2")
            {
                e.Graphics.DrawString("Giá trên chưa bao gồm thuế GTGT", boldSmall2Font, brush, 145, YPos, centerFormat);
                YPos += boldSmall2Font.Height + 4;
            }

            e.Graphics.DrawString("Vui lòng kiểm tra kỹ lại nội dung trước khi thanh toán!", boldSmall2Font, brush, 145, YPos, centerFormat);
        }

        float MeasureString(string text, Font font)
        {
            using (Bitmap bitmap = new Bitmap(1, 1))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    SizeF size = graphics.MeasureString(text, font);
                    return size.Width;
                }
            }
        }

        string[] WordWrap(string text, Font font, float maxWidth)
        {
            string[] words = text.Split(' ');
            string currentLine = "";
            var lines = new List<string>();

            foreach (string word in words)
            {
                if (MeasureString(currentLine + word, font) <= maxWidth)
                {
                    currentLine += word + " ";
                }
                else
                {
                    lines.Add(currentLine.Trim());
                    currentLine = word + " ";
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
            {
                lines.Add(currentLine.Trim());
            }

            return lines.ToArray();
        }

        private double CalculateTotal()
        {
            double total = 0.0;

            // Iterate through each item in the ListView
            foreach (ListViewItem item in lsvReceipt.Items)
            {
                // Check if the item has enough sub-items (columns)
                if (item.SubItems.Count >= 4)
                {
                    // Assuming column 4 is the index 3 (0-based index)
                    string column4Value = item.SubItems[4].Text;

                    // Parse the value to a double and add it to the total
                    double column4DoubleValue;
                    if (double.TryParse(column4Value, out column4DoubleValue))
                    {
                        total += column4DoubleValue;
                    }
                    // If parsing fails, you might want to handle it accordingly
                }
            }

            return total;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lsvReceipt.SelectedItems.Count > 0)
            {
                // Enable the menu item if items are selected
                contextMenuStrip1.Enabled = true;
            }
            else
            {
                // Disable the menu item if no items are selected
                contextMenuStrip1.Enabled = false;
            }
        }

        void ReArrangeListView()
        {
            for (int i = 0; i < lsvReceipt.Items.Count; i++)
                lsvReceipt.Items[i].Text = (i + 1).ToString();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lsvReceipt.Items.Clear();
            txtFoodName.Clear();
            txtPrice.Clear();
            nmuQuantity.Value = 0;
            NumberOrder = 1;
            YPos = 200;
        }


        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = lsvReceipt.FocusedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Chưa chọn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lsvReceipt.Items.Remove(selectedItem);
            NumberOrder--;
            ReArrangeListView();
        }


        private void sửaSốLượngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = lsvReceipt.FocusedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Chưa chọn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnUpdate.Enabled = true;
            btnAdd.Enabled = false;
            txtFoodName.Text = selectedItem.SubItems[1].Text;
            txtPrice.Text = selectedItem.SubItems[3].Text.Replace(",", "");
            nmuQuantity.Text = selectedItem.SubItems[2].Text;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var selectedItem = lsvReceipt.FocusedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Chưa chọn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnUpdate.Enabled = false;
            btnAdd.Enabled = true;
            selectedItem.SubItems[1].Text = txtFoodName.Text;
            selectedItem.SubItems[3].Text = (int.TryParse(txtPrice.Text, out int p1) ? p1 : 0).ToString("N0");
            selectedItem.SubItems[2].Text = nmuQuantity.Text;
            selectedItem.SubItems[4].Text = (nmuQuantity.Value * (int.TryParse(txtPrice.Text, out int p) ? p : 0)).ToString("N0");
        }
    }
}