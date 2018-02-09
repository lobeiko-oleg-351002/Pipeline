using BllEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.EventClasses
{
    public class DataGridManager
    {
        Dictionary<string, int> ColumnIndicies = new Dictionary<string, int>();

        const string COL_SENDER = "От";
        const string COL_TITLE = "Заголовок";
        const string COL_DATE = "Дата";
        const string COL_TIME = "Время";
        const string COL_STATUS = "Статус";
        const string COL_ATTRIBUTE = "Атрибуты";
        const string COL_FILE = "Файлы";
        const string COL_DESCTIPTION = "Описание";
        const string COL_NOTE = "Заметки";

        const string DATE_FORMAT = "dd.MM.yyyy";
        const string TIME_FORMAT = "HH:mm";

        public DataGridViewRow AddRowToDataGridUsingEvent(DataGridView grid, BllEvent Event)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(grid);

            row.Cells[ColumnIndicies[COL_SENDER]].Value = Event.Sender.Fullname;
            row.Cells[ColumnIndicies[COL_TITLE]].Value = Event.Name;
            row.Cells[ColumnIndicies[COL_DATE]].Value = Event.Date.ToString(DATE_FORMAT);
            row.Cells[ColumnIndicies[COL_TIME]].Value = Event.Date.ToString(TIME_FORMAT);

            if (Event.StatusLib.SelectedEntities.Count > 0)
            {
                var status = Event.StatusLib.SelectedEntities.Last();
                row.Cells[ColumnIndicies[COL_STATUS]].Value = status.Entity.Name + " " + status.Date;
            }

            foreach (var attr in Event.AttributeLib.SelectedEntities)
            {
                row.Cells[ColumnIndicies[COL_ATTRIBUTE]].Value += attr.Entity.Name + "; ";
            }

            var cellFile = (DataGridViewButtonCell)row.Cells[ColumnIndicies[COL_FILE]];
            if (Event.FilepathLib.Entities.Count == 0)
            {
                cellFile.Value = "-";
                cellFile.ReadOnly = true;
            }
            else
            {
                cellFile.Value += " " + Event.FilepathLib.Entities.Count + " ф.";
            }

            row.Cells[ColumnIndicies[COL_DESCTIPTION]].Value = Event.Description;

            grid.Rows.Add(row);

            return row;
        }

        
        private DataGridViewColumn CreateColumn(bool isSortable, int width, string headerText)
        {
            DataGridViewColumn col = new DataGridViewColumn();

            if (isSortable)
            {
                col.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            else
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            if (width == 0)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                col.Width = width;
            }
           
            col.HeaderText = headerText; 

            int columnsCount = ColumnIndicies.Count;
            ColumnIndicies.Add(headerText, columnsCount);

            return col;
        }

        public DataGridView InitDataGridView(DataGridView grid)
        {
            ColumnIndicies.Clear();

            grid.Columns.Add(CreateColumn(true, 0, COL_SENDER));
            grid.Columns.Add(CreateColumn(true, 0, COL_TITLE));
            grid.Columns.Add(CreateColumn(true, 0, COL_DATE));
            grid.Columns.Add(CreateColumn(false, 60, COL_TIME));
            grid.Columns.Add(CreateColumn(true, 0, COL_STATUS));
            grid.Columns.Add(CreateColumn(true, 90, COL_ATTRIBUTE));
            grid.Columns.Add(CreateColumn(false, 50, COL_FILE));
            grid.Columns.Add(CreateColumn(false, 0, COL_DESCTIPTION));
            grid.Columns.Add(CreateColumn(false, 0, COL_NOTE));

            return grid;
        }
    }
}
