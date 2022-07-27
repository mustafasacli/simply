using Simply.Data;
using Simply.Data.Objects;
using Mst.Dexter.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SimpleInfra.SqlGui
{
    public partial class FrmSql : Form
    {
        public FrmSql()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.LoadCombo();
        }

        private void arrangeData(DataTable dataTable, bool includeBlobs = false)
        {
            if (includeBlobs)
            {
                if (dataTable == null)
                    return;

                if (dataTable.Rows.Count == 0)
                    return;

                List<string> strs = new List<string>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (column.DataType == typeof(byte[]))
                    {
                        strs.Add(column.ColumnName);
                    }
                }
                foreach (string str in strs)
                {
                    dataTable.Columns.Remove(str);
                }
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            this.GetData();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtConnStr.Text))
            {
                try
                {
                    using (IDbConnection connection =
                        DxConnectionFactory.Instance.GetConnection(
                            this.cmbxConnTypes.GetItemText(cmbxConnTypes.SelectedItem)))
                    {
                        connection.ConnectionString = this.txtConnStr.Text;
                        connection.Open();
                        connection.Close();
                    }
                    MessageBox.Show("Connection successed.");
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    MessageBox.Show(string.Format("Message : {0}\nStack Trace : {1}", exception.Message, exception.StackTrace));
                }
            }
            else
            {
                MessageBox.Show("Connection string should be defined.");
            }
        }

        private void cmbxConnTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.connType = (this.cmbxConnTypes.SelectedIndex < 0 ? ConnectionTypes.Sql : (ConnectionTypes)this.cmbxConnTypes.SelectedItem);
        }

        private void GetData()
        {
            DataTable table = null;
            this.grdTable.DataSource = table;
            this.grdTable.Refresh();
            try
            {
                if (string.IsNullOrWhiteSpace(this.txtConnStr.Text))
                {
                    MessageBox.Show("Connection string should be Defined.");
                    return;
                }
                else if (this.txtQuery.Text.Length > 0)
                {
                    table = this.GetTableAsync(this.cmbxConnTypes.GetItemText(cmbxConnTypes.SelectedItem), this.txtConnStr.Text, this.txtQuery.Text);
                    //table = this.GetTable(this.cmbxConnTypes.GetItemText(cmbxConnTypes.SelectedItem), this.txtConnStr.Text, this.txtQuery.Text);
                }
            }
            catch (Exception exception1)
            {
                //Exception exception = exception1;
                MessageBox.Show(string.Format("Message : {0}\nStack Trace : {1}", exception1.Message, exception1.StackTrace));
            }
            if (this.chkRemoveBlobs.Checked)
            {
                this.arrangeData(table, this.chkRemoveBlobs.Checked);
            }
            this.grdTable.DataSource = table;
            this.grdTable.Refresh();
        }

        public DataTable GetTable(string ConnType, string connectionString, string query)
        {
            DataTable dataTable = new DataTable();

            using (IDbConnection connection = DxConnectionFactory.Instance.GetConnection(ConnType))
            {
                connection.ConnectionString = connectionString;
                dataTable = connection.GetResultSetQuery(new DbCommandDefinition { CommandText = query, CommandType = CommandType.Text }).Result.Tables[0];
            }

            return dataTable;
        }

        public DataTable GetTableAsync(string ConnType, string connectionString, string query)
        {
            DataTable dataTable = new DataTable();

            using (IDbConnection connection = DxConnectionFactory.Instance.GetConnection(ConnType))
            {
                connection.ConnectionString = connectionString;
                dataTable = connection.GetResultSetQuery(new DbCommandDefinition { CommandText = query, CommandType = CommandType.Text }).Result.Tables[0];
            }

            return dataTable;
        }

        private void grdTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.loadColAndValue(e.ColumnIndex);
        }

        private void grdTable_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            this.loadColAndValue(e.ColumnIndex);
        }

        private void loadColAndValue(int colIndex)
        {
            this.txtValue.Text = string.Empty;
            this.txtColumn.Text = string.Empty;
            if (this.grdTable.SelectedCells.Count > 0 && colIndex > -1)
            {
                this.txtColumn.Text = this.grdTable.Columns[colIndex].HeaderText;
                this.txtValue.Text = string.Format("{0}", this.grdTable.SelectedCells[0].Value);
            }
        }

        private void LoadCombo()
        {
            try
            {
                this.cmbxConnTypes.Items.Clear();
                this.cmbxConnTypes.Items.AddRange(DxConnectionFactory.Instance.ConnectionKeys.ToArray());

                this.cmbxConnTypes.Refresh();
                this.cmbxConnTypes.SelectedIndex = (this.cmbxConnTypes.Items.Count > 0 ? 1 : -1);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                MessageBox.Show(string.Format("Message : {0}\nStack Trace : {1}", exception.Message, exception.StackTrace));
            }
        }

        internal void ExportToExcel(string fileName)
        {
            try
            {
                using (System.IO.StreamWriter excelDocumentWriter = new System.IO.StreamWriter(fileName))
                {
                    const string startExcelXML = "<xml version>\r\n<Workbook " +
                          "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                          " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                          "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                          "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                          "office:spreadsheet\">\r\n <Styles>\r\n " +
                          "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                          "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                          "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                          "\r\n <Protection/>\r\n </Style>\r\n " +
                          "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                          "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                          "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                          " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                          "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                          "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +
                          "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                          "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                          "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                          "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                          "</Styles>\r\n ";
                    const string endExcelXML = "</Workbook>";

                    int rowCount = 0;
                    int sheetCount = 1;
                    excelDocumentWriter.Write(startExcelXML);
                    excelDocumentWriter.Write("<Worksheet ss:Name=\"Shortcut" + sheetCount + "\">");
                    excelDocumentWriter.Write("<Table>");
                    excelDocumentWriter.Write("<Row>");
                    for (int x = 0; x < grdTable.ColumnCount; x++)
                    {
                        excelDocumentWriter.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                        excelDocumentWriter.Write(grdTable.Columns[x].HeaderText);
                        excelDocumentWriter.Write("</Data></Cell>");
                    }
                    excelDocumentWriter.Write("</Row>");
                    string XMLstring;

                    foreach (DataGridViewRow gRow in grdTable.Rows)
                    {
                        //gRow.Cells[0].
                        rowCount++;
                        //if the number of rows is > 64000 create a new page to continue output
                        if (rowCount == 64000)
                        {
                            rowCount = 0;
                            sheetCount++;
                            excelDocumentWriter.Write("</Table>");
                            excelDocumentWriter.Write(" </Worksheet>");
                            excelDocumentWriter.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                            excelDocumentWriter.Write("<Table>");
                        }
                        excelDocumentWriter.Write("<Row>"); //ID=" + rowCount + "
                        foreach (DataGridViewCell cell in gRow.Cells)
                        {
                            XMLstring = string.Empty;
                            XMLstring = cell.Value?.ToString() ?? "";
                            XMLstring = XMLstring.Trim();
                            XMLstring = XMLstring.Replace("&", "&");
                            XMLstring = XMLstring.Replace(">", ">");
                            XMLstring = XMLstring.Replace("<", "<");
                            excelDocumentWriter.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                           "<Data ss:Type=\"String\">");
                            excelDocumentWriter.Write(XMLstring);
                            excelDocumentWriter.Write("</Data></Cell>");
                        }

                        excelDocumentWriter.Write("</Row>");
                    }

                    excelDocumentWriter.Write("</Table>");
                    excelDocumentWriter.Write(" </Worksheet>");
                    excelDocumentWriter.Write(endExcelXML);
                    excelDocumentWriter.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Message : {0}\nStack Trace : {1}", exception.Message, exception.StackTrace));
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                saveExcelExportDialog.InitialDirectory = Environment.GetFolderPath(
                        Environment.SpecialFolder.DesktopDirectory);
                DialogResult saveDialogResult = saveExcelExportDialog.ShowDialog();
                if (saveDialogResult == DialogResult.OK)
                {
                    ExportToExcel(saveExcelExportDialog.FileName);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Message : {0}\nStack Trace : {1}", exception.Message, exception.StackTrace));
            }
        }
    }
}