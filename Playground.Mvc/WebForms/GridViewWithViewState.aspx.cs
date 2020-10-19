using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Playground.Mvc.WebForms
{
    public partial class GridViewWithViewState : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FirstGridViewRow();
                this.Calendar1.SelectedDate = DateTime.Today.Date;
            }
        }

        private void FirstGridViewRow()
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("RowNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("userID", typeof(string)));
            dt.Columns.Add(new DataColumn("PartNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("LotID", typeof(string)));
            dt.Columns.Add(new DataColumn("ExpirationDate", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Quantity", typeof(string)));
            dt.Columns.Add(new DataColumn("Comments", typeof(string)));

            dr = dt.NewRow();
            dr["RowNumber"] = 1;
            dr["userID"] = string.Empty;
            dr["PartNumber"] = string.Empty;
            dr["LotID"] = string.Empty;
            dr["ExpirationDate"] = DateTime.Today.ToShortDateString();
            dr["Quantity"] = string.Empty;
            dr["Comments"] = string.Empty;
            dt.Rows.Add(dr);

            ViewState["CurrentTable"] = null;
            ViewState["CurrentTable"] = dt;

            dgvIssuenceLogEntry.DataSource = dt;
            dgvIssuenceLogEntry.DataBind();
        }

        private void AddNewRow()
        {
            int rowIndex = 0;
            string _userName = string.Empty;

            if (ViewState["CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                DataRow drCurrentRow = null;
                if (dtCurrentTable.Rows.Count > 0)
                {
                    _userName = (this.dgvIssuenceLogEntry.Rows[0].FindControl("txtUser") as TextBox).Text;

                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    {
                        TextBox TextBoxUser = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[1].FindControl("txtUser");
                        TextBox TextBoxPartNumber = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[2].FindControl("txtPartNumber");
                        TextBox TextBoxLotID = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtLotID");
                        TextBox TextBoxExpirationDate = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtExpirationDate");
                        TextBox TextBoxQuantity = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtQuantity");
                        TextBox TextBoxComments = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtcomments");

                        drCurrentRow = dtCurrentTable.NewRow();
                        drCurrentRow["RowNumber"] = i + 1;

                        dtCurrentTable.Rows[i - 1]["userID"] = TextBoxUser.Text;
                        dtCurrentTable.Rows[i - 1]["PartNumber"] = TextBoxPartNumber.Text;
                        dtCurrentTable.Rows[i - 1]["LotID"] = TextBoxLotID.Text;
                        dtCurrentTable.Rows[i - 1]["ExpirationDate"] = Convert.ToDateTime(TextBoxExpirationDate.Text).ToShortDateString();
                        dtCurrentTable.Rows[i - 1]["Quantity"] = TextBoxQuantity.Text;
                        dtCurrentTable.Rows[i - 1]["Comments"] = TextBoxComments.Text;

                        rowIndex++;
                    }
                    dtCurrentTable.Rows.Add(drCurrentRow);
                    ViewState["CurrentTable"] = dtCurrentTable;

                    dgvIssuenceLogEntry.DataSource = dtCurrentTable;
                    dgvIssuenceLogEntry.DataBind();
                }
            }
            else
            {
                Response.Write("ViewState is null");
            }
            SetPreviousData(_userName);
        }

        private void SetPreviousData(string userName = "")
        {
            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        TextBox TextBoxUser = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[1].FindControl("txtUser");
                        TextBox TextBoxPartNumber = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[2].FindControl("txtPartNumber");
                        TextBox TextBoxLotID = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtLotID");
                        TextBox TextBoxExpirationDate = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtExpirationDate");
                        TextBox TextBoxQuantity = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtQuantity");
                        TextBox TextBoxComments = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtcomments");

                        //TextBoxUser.Text = dt.Rows[i]["userID"].ToString();
                        TextBoxUser.Text = userName;
                        TextBoxPartNumber.Text = dt.Rows[i]["PartNumber"].ToString();
                        TextBoxLotID.Text = dt.Rows[i]["LotID"].ToString();
                        TextBoxExpirationDate.Text = dt.Rows[i]["ExpirationDate"].ToString();
                        TextBoxQuantity.Text = dt.Rows[i]["Quantity"].ToString();
                        TextBoxComments.Text = dt.Rows[i]["Comments"].ToString();

                        rowIndex++;
                    }

                    int _lastRow = dt.Rows.Count - 1;
                    dgvIssuenceLogEntry.Rows[_lastRow].Cells[2].FindControl("txtPartNumber").Focus();
                }
            }
        }

        private void SetRowData()
        {
            int rowIndex = 0;

            if (ViewState["CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                DataRow drCurrentRow = null;
                if (dtCurrentTable.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    {
                        TextBox TextBoxUser = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[1].FindControl("txtUser");
                        TextBox TextBoxPartNumber = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[2].FindControl("txtPartNumber");
                        TextBox TextBoxLotID = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtLotID");
                        TextBox TextBoxExpirationDate = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtExpirationDate");
                        TextBox TextBoxQuantity = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtQuantity");
                        TextBox TextBoxComments = (TextBox)dgvIssuenceLogEntry.Rows[rowIndex].Cells[3].FindControl("txtcomments");

                        drCurrentRow = dtCurrentTable.NewRow();
                        drCurrentRow["RowNumber"] = i + 1;
                        dtCurrentTable.Rows[i - 1]["userID"] = TextBoxUser.Text;
                        dtCurrentTable.Rows[i - 1]["PartNumber"] = TextBoxPartNumber.Text;
                        dtCurrentTable.Rows[i - 1]["LotID"] = TextBoxLotID.Text;
                        dtCurrentTable.Rows[i - 1]["ExpirationDate"] = DateTime.MinValue.ToShortDateString();
                        dtCurrentTable.Rows[i - 1]["Quantity"] = TextBoxQuantity.Text;
                        dtCurrentTable.Rows[i - 1]["Comments"] = TextBoxComments.Text;
                        rowIndex++;
                    }

                    ViewState["CurrentTable"] = dtCurrentTable;
                }
            }
            else
            {
                Response.Write("ViewState is null");
            }
        }

        protected void ButtonAdd_Click(object sender, EventArgs e)
        {
            AddNewRow();
        }

        protected void dgvIssuenceLogEntry_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            SetRowData();
            string _userName = string.Empty;

            if (ViewState["CurrentTable"] != null)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                DataRow drCurrentRow = null;
                int rowIndex = Convert.ToInt32(e.RowIndex);
                if (dt.Rows.Count > 1)
                {
                    dt.Rows.Remove(dt.Rows[rowIndex]);
                    drCurrentRow = dt.NewRow();
                    ViewState["CurrentTable"] = dt;
                    dgvIssuenceLogEntry.DataSource = dt;
                    dgvIssuenceLogEntry.DataBind();

                    _userName = dt.Rows[0]["UserID"].ToString();

                    for (int i = 0; i < dgvIssuenceLogEntry.Rows.Count - 1; i++)
                    {
                        dgvIssuenceLogEntry.Rows[i].Cells[0].Text = Convert.ToString(i + 1);
                    }

                    SetPreviousData(_userName);
                }
            }
        }

        protected void OkButton_Click(object sender, EventArgs e)
        {
            MessagesRow.Visible = false;
            MessageValue.Text = string.Empty;
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            string _validateMsg = string.Empty;

            _validateMsg = ValidateData();

            if (string.IsNullOrEmpty(_validateMsg))
            {
                SetRowData();

                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow _row in dt.Rows)
                    {
                        string _user = _row["userID"].ToString();
                        string _partNumber = _row["PartNumber"].ToString();
                        string _lotID = _row["LotID"].ToString();
                        string _expirationDate = _row["ExpirationDate"].ToString();
                        double _quantity = Convert.ToDouble(_row["Quantity"]);
                        string _comments = _row["Comments"].ToString();

                        _expirationDate = Convert.ToDateTime(_expirationDate).ToString("MM/dd/yy HH:mm:ss");

                        //ChemUtilities.ChemIssue(_partNumber, _lotID, _quantity, _expirationDate, _user, _comments);
                    }
                }

                FirstGridViewRow();
                MessagesRow.Visible = true;
                MessageValue.Text = "Data submitted. (Dummy Msg!!!)";
            }
            else
            {
                MessagesRow.Visible = true;
                MessageValue.Text = _validateMsg;
            }
        }

        private string ValidateData()
        {
            string _retVal = string.Empty;
            StringBuilder _sb = new StringBuilder();

            foreach (GridViewRow _row in this.dgvIssuenceLogEntry.Rows)
            {
                string _user = (_row.FindControl("txtUser") as TextBox).Text;
                string _partNumber = (_row.FindControl("txtPartNumber") as TextBox).Text;
                string _lotID = (_row.FindControl("txtLotID") as TextBox).Text;
                string _expirationDate = (_row.FindControl("txtExpirationDate") as TextBox).Text;
                string _quantity = (_row.FindControl("txtQuantity") as TextBox).Text;

                if (string.IsNullOrEmpty(_user))
                {
                    _sb.AppendLine("User ID is required at row number: " + (_row.RowIndex + 1));
                }

                if (string.IsNullOrEmpty(_partNumber))
                {
                    _sb.AppendLine("Part Number is required at row number: " + (_row.RowIndex + 1));
                }

                if (string.IsNullOrEmpty(_lotID))
                {
                    _sb.AppendLine("Batch/Lot is required at row number: " + (_row.RowIndex + 1));
                }

                if (string.IsNullOrEmpty(_expirationDate))
                {
                    _sb.AppendLine("Expiration Date is required at row number: " + (_row.RowIndex + 1));
                }
                else
                {
                    DateTime _dateTime;
                    if (!DateTime.TryParse(_expirationDate, out _dateTime))
                    {
                        _sb.AppendLine("Invalid Expiration Date at row number: " + (_row.RowIndex + 1));
                    }
                }

                if (string.IsNullOrEmpty(_quantity))
                {
                    _sb.AppendLine("Quantity is required at row number: " + (_row.RowIndex + 1));
                }
                else
                {
                    double _q;
                    if (!double.TryParse(_quantity, out _q))
                    {
                        _sb.AppendLine("Invalid Quantity at row number: " + (_row.RowIndex + 1));
                    }
                }
            }

            if (_sb.Length > 0)
            {
                _retVal = _sb.ToString();
            }

            return _retVal;
        }

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            string _fileName = string.Empty;

            if (ValidateDate())
            {
                _fileName = "Chemical_Issued_Exported_Data_" + DateTime.Now.ToString("MM_dd_yy_HH_mm_ss") + ".xls";
                DataTable _dt = GetFilteredDataForExport();

                if (_dt.Rows.Count > 0)
                {
                    var _grid = new System.Web.UI.WebControls.GridView();
                    _grid.DataSource = _dt;
                    _grid.AllowPaging = false;
                    _grid.DataBind();

                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename=" + _fileName);
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";

                    using (StringWriter sw = new StringWriter())
                    {
                        HtmlTextWriter hw = new HtmlTextWriter(sw);

                        _grid.RenderControl(hw);

                        //style to format numbers to string
                        string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                        Response.Write(style);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('No data found for the date provided! Try a different date.');", true);
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Invalid Date entered. Try again.');", true);
            }
        }

        private bool ValidateDate()
        {
            bool _retVal = true;
            DateTime _dateTime;

            if (!DateTime.TryParse(lblFromDate.Text, out _dateTime))
            {
                return false;
            }

            if (!DateTime.TryParse(lblToDate.Text, out _dateTime))
            {
                return false;
            }

            return _retVal;
        }

        private DataTable GetFilteredDataForExport()
        {
            DataTable retVal = new DataTable();
            DataRow[] _rows = null;
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
            if (dtCurrentTable.Rows.Count > 0)
            {
                string _fromDate = Convert.ToDateTime(lblFromDate.Text).ToString("MM/dd/yyyy");
                string _toDate = Convert.ToDateTime(lblToDate.Text).AddDays(1).ToString("MM/dd/yyyy");
                string _expression = string.Format("#{0}# >= ExpirationDate AND #{1}# <= ExpirationDate", _fromDate, _toDate);
                _rows = dtCurrentTable.Select(_expression);

                if (_rows.Length > 0)
                {
                    retVal = _rows.CopyToDataTable();
                }
            }

            return retVal;
        }

        protected void Calandar1_SelectionChanged(object sender, EventArgs e)
        {
            if (rbFromDate.Checked)
            {
                lblFromDate.Text = Calendar1.SelectedDate.ToShortDateString();
            }

            if (rbToDate.Checked)
            {
                lblToDate.Text = Calendar1.SelectedDate.ToShortDateString();
            }
        }
    }
}