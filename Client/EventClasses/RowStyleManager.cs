using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.EventClasses
{
    public static class RowStyleManager
    {
        public static void MakeRegularRowFont(DataGridViewRow row)
        {
            SetRowFontStyle(FontStyle.Regular, row);
        }

        public static void MakeBoldRowFont(DataGridViewRow row)
        {
            SetRowFontStyle(FontStyle.Bold, row);
        }

        private static void SetRowFontStyle(FontStyle fontStyle, DataGridViewRow row)
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(row.DefaultCellStyle.Font, fontStyle);
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.Style.Font = style.Font;
            }
        }

        public static void MakeRowBackgroundAsRegular(DataGridViewRow row)
        {
            row.DefaultCellStyle.ForeColor = Color.Black;
            row.DefaultCellStyle.BackColor = Color.White;
        }

        public static void MakeRowBackgroungAsDeleted(DataGridViewRow row)
        {
            row.DefaultCellStyle.ForeColor = Color.Red;
            row.DefaultCellStyle.BackColor = Color.DarkRed;
        }

        public static void MakeRowBackgroungAsClosed(DataGridViewRow row)
        {
            row.DefaultCellStyle.ForeColor = Color.Gray;
            row.DefaultCellStyle.BackColor = Color.LightGray;
        }

        public static void MakeCellBoldFont(DataGridViewCell cell)
        {
            cell.Style.Font = new Font(cell.Style.Font, FontStyle.Bold);
        }

        public static void MakeCellRegularFont(DataGridViewCell cell)
        {
            cell.Style.Font = new Font(cell.Style.Font, FontStyle.Regular);
        }
    }
}
