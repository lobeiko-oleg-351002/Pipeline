using BllEntities;
using Client.EventClasses.Events;
using Client.EventClasses.Sorting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.DataGridControls
{
    public class DataGridPopulationManager
    {
        static Dictionary<string, int> ColumnIndicies = new Dictionary<string, int>();
        DataGridView grid;

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

        public DataGridPopulationManager(DataGridView dataGridView)
        {
            InitDataGridView(dataGridView);
            grid = dataGridView;
        }

        public void InitDataGridView(DataGridView grid)
        {
            ColumnIndicies.Clear();

            grid.Columns.Add(CreateTextColumn(true, 0, COL_SENDER));
            grid.Columns.Add(CreateTextColumn(true, 0, COL_TITLE));
            grid.Columns.Add(CreateTextColumn(true, 73, COL_DATE));
            grid.Columns.Add(CreateTextColumn(false, 50, COL_TIME));
            grid.Columns.Add(CreateTextColumn(true, 0, COL_STATUS));
            grid.Columns.Add(CreateTextColumn(true, 90, COL_ATTRIBUTE));
            grid.Columns.Add(CreateButtonColumn(false, 50, COL_FILE));
            grid.Columns.Add(CreateTextColumn(false, 0, COL_DESCTIPTION));
            grid.Columns.Add(CreateTextColumn(false, 0, COL_NOTE));
        }

        public void AddRowToDataGridUsingEvent(UiEvent uiEvent)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(grid);
            row.DefaultCellStyle.Font = new Font("Tahoma", 9);

            BllEvent Event = uiEvent.EventData;

            row.Cells[ColumnIndicies[COL_SENDER]].Value = Event.Sender.Fullname;
            row.Cells[ColumnIndicies[COL_TITLE]].Value = Event.Name;
            row.Cells[ColumnIndicies[COL_DATE]].Value = Event.Date.ToString(DATE_FORMAT);
            row.Cells[ColumnIndicies[COL_TIME]].Value = Event.Date.ToString(TIME_FORMAT);
            row.Cells[ColumnIndicies[COL_DESCTIPTION]].Value = Event.Description;
            row.Cells[ColumnIndicies[COL_NOTE]].Value = uiEvent.Note;

            SetStatusInRow(row, Event);
            SetAttributeInRow(row, Event);
            SetFileCountInRow(row, Event);

            uiEvent.SetRowStyle(row);

            grid.Rows.Add(row);

            StatusStyleManager.SetStatusStyle(uiEvent, row);
        }

        public void SetStatusInRow(DataGridViewRow row, BllEvent Event)
        {
            if (Event.StatusLib.SelectedEntities.Count > 0)
            {
                var status = Event.StatusLib.SelectedEntities.Last();
                row.Cells[ColumnIndicies[COL_STATUS]].Value = status.Entity.Name + " " + status.Date;

            }
        }

        private void SetAttributeInRow(DataGridViewRow row, BllEvent Event)
        {
            foreach (var attr in Event.AttributeLib.SelectedEntities)
            {
                row.Cells[ColumnIndicies[COL_ATTRIBUTE]].Value += attr.Entity.Name + "; ";
            }
        }

        private void SetFileCountInRow(DataGridViewRow row, BllEvent Event)
        {
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
        }

        private DataGridViewTextBoxColumn CreateTextColumn(bool isSortable, int width, string headerText)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            return (DataGridViewTextBoxColumn)InitColumn(col, isSortable, width, headerText);
        }

        private DataGridViewButtonColumn CreateButtonColumn(bool isSortable, int width, string headerText)
        {
            DataGridViewButtonColumn col = new DataGridViewButtonColumn();
            return (DataGridViewButtonColumn)InitColumn(col, isSortable, width, headerText);
        }

        private DataGridViewColumn InitColumn(DataGridViewColumn col, bool isSortable, int width, string headerText)
        {
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

        public SortableColumn GetSortingUsingColHeader(string header)
        {
            SortableColumn clickedColumn = null;
            switch(header)
            {
                case COL_SENDER:
                    clickedColumn = new SortingBySender();
                    break;
                case COL_TITLE:
                    clickedColumn = new SortingByName();
                    break;
                case COL_STATUS:
                    clickedColumn = new SortingByStatus();
                    break;
                case COL_DATE:
                    clickedColumn = new SortingByDate();
                    break;
                case COL_ATTRIBUTE:
                    clickedColumn = new SortingByAttribute();
                    break;
                default:
                    clickedColumn = new SortingByDate();
                    break;
            }

            return clickedColumn;
        }

        public void PopulateDataGrid(List<UiEvent> events)
        {
            try
            {
                grid.Rows.Clear();
                foreach (var item in events)
                {
                    AddRowToDataGridUsingEvent(item);
                }
            }
            catch
            {
                PopulateDataGrid(events);
            }
        }

        public void SetClosedStyleForRow(DataGridViewRow row)
        {
            RowStyleManager.MakeRowBackgroungAsClosed(row);
        }

        public void SetDeletedStyleForRow(DataGridViewRow row)
        {
            RowStyleManager.MakeRowBackgroungAsDeleted(row);
        }

        public void SetRegularStyleForRow(DataGridViewRow row)
        {
            RowStyleManager.MakeRowBackgroundAsRegular(row);
        }

        public void MakeRowInvisible(DataGridViewRow row)
        {
            row.Visible = false;
        }

        public void MakeRowVisible(DataGridViewRow row)
        {
            row.Visible = true;
        }

        public static int GetStatusColumnNum()
        {
            return ColumnIndicies[COL_STATUS];
        }

        public void SetNoteToRow(DataGridViewRow row, string note)
        {
            row.Cells[ColumnIndicies[COL_NOTE]].Value = note; 
        }
    }
}
