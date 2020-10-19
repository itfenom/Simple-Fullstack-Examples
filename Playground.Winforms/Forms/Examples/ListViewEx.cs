using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Playground.Core.AdoNet;
using Playground.Winforms.Utilities;

namespace Playground.Winforms.Forms.Examples
{
    public partial class ListViewEx : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private DialogResult _iAnswer;
        // ReSharper disable once NotAccessedField.Local
        private DataTable _userDt;
        private AppUserCollection _appUserColl;

        public ListViewEx()
        {
            _userDt = new DataTable();
            InitializeComponent();
        }

        public void ShowForm(Form oParent)
        {
            LoadData();
            _iAnswer = ShowDialog(oParent);
            Close();
        }

        private void LoadData()
        {
            _appUserColl = new AppUserCollection();

            if (_appUserColl.LoadData())
            {
                foreach (AppUserCollection item in _appUserColl)
                {
                    // ReSharper disable once InconsistentNaming
                    var _item = new ListViewItem();

                    _item.Text = item.Id.ToString(CultureInfo.InvariantCulture);
                    _item.SubItems.Add(item.LoginName);
                    _item.SubItems.Add(item.Email);
                    _item.SubItems.Add(Convert.ToDateTime(item.DateModified).ToShortDateString());

                    listView1.Items.Add(_item);
                }
            }
            //_sql = "SELECT ID, LOGIN_NAME, EMAIL, DATE_MODIFIED FROM APP_USER";
            //_userDT = tqtBaw.ExecuteQuery(_sql);

            //if (_userDT.Rows.Count == 0) return;

            //foreach (DataRow _r in _userDT.Rows)
            //{
            //    GenericItem _genericItem = new GenericItem();
            //    _genericItem.Tag = _r["ID"];
            //    _genericItem.Text = _r["LOGIN_NAME"].ToString();

            //    ListViewItem _listViewItem = new ListViewItem();
            //    _listViewItem.Text = _r["ID"].ToString();
            //    _listViewItem.SubItems.Add(_r["LOGIN_NAME"].ToString());
            //    _listViewItem.SubItems.Add(_r["EMAIL"].ToString());
            //    _listViewItem.SubItems.Add(Convert.ToDateTime(_r["DATE_MODIFIED"].ToString()).ToString("MM/dd/yyyy"));
            //    _listViewItem.Tag = _genericItem;
            //    this.listView1.Items.Add(_listViewItem);
            //}
        }

        private void BtnDisplayChecked_Click(object sender, EventArgs e)
        {
            var checkedRows = GetCheckedRows();

            if (checkedRows == 0)
            {
                Helpers.DisplayInfo(this, "No rows were selected.", "ListView");
                return;
            }

            string msg = string.Empty;
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    msg += $"ID: {item.SubItems[0].Text}, " +
                           $"Name: {item.SubItems[1].Text}, " +
                           $"Email: {item.SubItems[2].Text}, " +
                           $"Date: {item.SubItems[3].Text}\n";
                    //GenericItem _genericItem = new GenericItem();
                    //_genericItem = (GenericItem)item.Tag;
                    //_msg += string.Format("ID: {0}, Name: {1}, Email: {2}, Date: {3}\n", _genericItem.Tag.ToString(), _genericItem.Text, item.SubItems[2].Text, item.SubItems[3].Text);
                }
            }

            MessageBox.Show(msg, @"Checked row in List View", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int GetCheckedRows()
        {
            int retVal = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    retVal++;
                }
            }
            return retVal;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                Helpers.DisplayInfo(this, "Please enter name to search user by name!", "Search");
                return;
            }

            rTxtBox.Clear();

            var oForm = new ListViewSearchDialog();

            var searchTerm = txtSearch.Text.Trim();
            var dt = DAL.Seraph.ExecuteQuery("SELECT ID, LOGIN_NAME, EMAIL, DATE_MODIFIED FROM APP_USER WHERE LOGIN_NAME LIKE '" + searchTerm + "%'");
            string retVal = oForm.ShowForm(this, dt);

            if (!string.IsNullOrEmpty(retVal))
            {
                rTxtBox.Text = retVal;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    public class AppUserCollection : CollectionBase
    {
        public decimal Id { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }
        public DateTime? DateModified { get; set; }

        // ReSharper disable once EmptyConstructor
        public AppUserCollection()
        {

        }

        public AppUserCollection this[int index]
        {
            get => (AppUserCollection)List[index];
            set => List[index] = value;
        }

        public void Add(AppUserCollection obj)
        {
            List.Add(obj);
        }

        public void Remove(AppUserCollection obj)
        {
            List.Remove(obj);
        }

        public override string ToString()
        {
            return $"{Id.ToString(CultureInfo.InvariantCulture)} - {LoginName}";
        }

        public bool LoadData()
        {
            var dt = DAL.Seraph.ExecuteQuery("SELECT ID, LOGIN_NAME, EMAIL, DATE_MODIFIED FROM APP_USER");
            foreach (DataRow row in dt.Rows)
            {
                var obj = new AppUserCollection();

                obj.Id = Convert.ToDecimal(row["ID"]);
                obj.LoginName = row["LOGIN_NAME"].ToString();
                obj.Email = row["EMAIL"].ToString();
                obj.DateModified = Convert.ToDateTime(row["DATE_MODIFIED"]);

                List.Add(obj);
            }

            if (List.Count > 0)
            {
                return true;
            }
            return false;

            /*
            bool _retVal = false;
            string _sql = string.Empty;
            OracleConnection _oConn = new OracleConnection();

            _oConn.ConnectionString = DAL.GetSeraphConnectionString();
            _oConn.Open();

            using (OracleCommand _oCmd = new OracleCommand(_sql, _oConn))
            {
                OracleDataReader _oReader = _oCmd.ExecuteReader();

                while (_oReader.Read())
                {
                    AppUserCollection _obj = new AppUserCollection();

                    _obj.Id = Convert.ToDecimal(_oReader["ID"]);
                    _obj.LoginName = _oReader["LOGIN_NAME"].ToString();
                    _obj.Email = _oReader["EMAIL"].ToString();
                    _obj.DateModified = Convert.ToDateTime(_oReader["DATE_MODIFIED"]);

                    List.Add(_obj);
                }

                _oReader.Dispose();
            }

            if (List.Count > 0)
            {
                _retVal = true;
            }

            _oConn.Close();
            _oConn.Dispose();

            return _retVal;
            */
        }
    }
}
