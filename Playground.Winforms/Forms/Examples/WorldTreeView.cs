using System;
using System.Linq;
using System.Windows.Forms;
using Playground.Winforms.Forms.Examples.WorldTreeViewEx;

namespace Playground.Winforms.Forms.Examples
{
    public partial class WorldTreeView : Form
    {
        private WorldTreeViewEx.WorldTreeView _worldTreeView;
        public WorldTreeViewFilters WorldTreeViewFilters { get; set; }

        public WorldTreeView()
        {
            InitializeComponent();

            WorldTreeViewFilters = new WorldTreeViewFilters();

            _worldTreeView = new WorldTreeViewEx.WorldTreeView(WorldTreeViewFilters);
            tableLayoutPanel1.Controls.Add(_worldTreeView, 1, 1);
            _worldTreeView.Dock = DockStyle.Fill;
        }

        private void RefreshTree()
        {
            Cursor = Cursors.WaitCursor;
            _worldTreeView.RefreshTree();
            Cursor = Cursors.Arrow;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTree();
            txtFilterBy.Clear();
            cboFilterBy.SelectedIndex = -1;
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            //Set all filters properties to NULL!
            ClearFilters();

            txtFilterBy.Clear();
            cboFilterBy.SelectedIndex = -1;
            _worldTreeView.RecreateTreeFromScratch();
            _worldTreeView.Focus();
            RefreshTree();
        }

        private void ClearFilters()
        {
            WorldTreeViewFilters.ContinentName = null;
            WorldTreeViewFilters.CountryName = null;
            WorldTreeViewFilters.StateName = null;
            WorldTreeViewFilters.CityName = null;
        }

        private void txtFilterBy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var searchBy = cboFilterBy.Text;

                if (searchBy.ToLower().Contains("select") || string.IsNullOrEmpty(txtFilterBy.Text))
                {
                    MessageBox.Show(@"Please select search by", @"Search", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                switch (searchBy.ToUpper())
                {
                    case "CITY":
                        LoadTreeByCityName(txtFilterBy.Text.Trim());
                        break;
                    case "STATE":
                        LoadTreeByStateName(txtFilterBy.Text.Trim());
                        break;
                    case "COUNTRY":
                        LoadTreeByCountryName(txtFilterBy.Text.Trim());
                        break;
                    case "CONTINENT":
                        LoadTreeByContinentName(txtFilterBy.Text.Trim());
                        break;
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void LoadTreeByContinentName(string continentName)
        {
            if (_worldTreeView.Nodes.Count == 0)
            {
                MessageBox.Show("Tree is empty.", "Search by Continent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ClearFilters();

            WorldTreeViewFilters.ContinentName = continentName;

            var continent = WorldTreeViewRepository.GetAllContinents().Where(s => s.FieldName.ToLower().StartsWith(continentName.ToLower())).FirstOrDefault();
            if (continent == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            foreach (TreeNode rootNode in _worldTreeView.Nodes)
            {
                rootNode.Expand();

                foreach (TreeNode continentNode in rootNode.Nodes)
                {
                    if (continentNode.Tag.ToString() == continent.FieldName)
                    {
                        continentNode.Expand();
                        break;
                    }
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void LoadTreeByCountryName(string countryName)
        {
            if (_worldTreeView.Nodes.Count == 0)
            {
                MessageBox.Show("Tree is empty.", "Search by Country", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ClearFilters();

            WorldTreeViewFilters.CountryName = countryName;

            var country = WorldTreeViewRepository.GetAllCountries().Where(s => s.FieldName.ToLower().StartsWith(countryName.ToLower())).FirstOrDefault();
            if (country == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            var continent = WorldTreeViewRepository.GetAllContinents().Where(c => c.Id == country.ParentId).FirstOrDefault();
            if (continent == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            foreach (TreeNode rootNode in _worldTreeView.Nodes)
            {
                rootNode.Expand();

                foreach (TreeNode continentNode in rootNode.Nodes)
                {
                    if (continentNode.Text != continent.FieldName) continue;

                    continentNode.Expand();

                    foreach (TreeNode countryNode in continentNode.Nodes)
                    {
                        if (countryNode.Tag.ToString() == country.FieldName)
                        {
                            countryNode.Expand();
                            break;
                        }
                    }
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void LoadTreeByStateName(string stateName)
        {
            if (_worldTreeView.Nodes.Count == 0)
            {
                MessageBox.Show("Tree is empty.", "Search by State", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ClearFilters();

            WorldTreeViewFilters.StateName = stateName;

            var state = WorldTreeViewRepository.GetAllStates().Where(s => s.FieldName.ToLower().StartsWith(stateName.ToLower())).FirstOrDefault();
            if (state == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            var country = WorldTreeViewRepository.GetAllCountries().Where(c => c.Id == state.ParentId).FirstOrDefault();
            if (country == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            var continent = WorldTreeViewRepository.GetAllContinents().Where(c => c.Id == country.ParentId).FirstOrDefault();
            if (continent == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            foreach (TreeNode rootNode in _worldTreeView.Nodes)
            {
                rootNode.Expand();

                foreach (TreeNode continentNode in rootNode.Nodes)
                {
                    if (continentNode.Text != continent.FieldName) continue;

                    continentNode.Expand();

                    foreach (TreeNode countryNode in continentNode.Nodes)
                    {
                        if (countryNode.Text != country.FieldName) continue;

                        countryNode.Expand();

                        foreach (TreeNode stateNode in countryNode.Nodes)
                        {
                            if (stateNode.Tag.ToString() == state.FieldName)
                            {
                                stateNode.Expand();
                                break;
                            }
                        }
                    }
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void LoadTreeByCityName(string cityName)
        {
            if (_worldTreeView.Nodes.Count == 0)
            {
                MessageBox.Show("Tree is empty.", "Search by State", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ClearFilters();

            WorldTreeViewFilters.CityName = cityName;

            var city = WorldTreeViewRepository.GetAllCities().Where(c => c.FieldName.ToLower().StartsWith(cityName.ToLower())).FirstOrDefault();
            if (city == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            var state = WorldTreeViewRepository.GetAllStates().Where(s => s.Id == city.ParentId).FirstOrDefault();
            if (state == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            var country = WorldTreeViewRepository.GetAllCountries().Where(c => c.Id == state.ParentId).FirstOrDefault();
            if (country == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            var continent = WorldTreeViewRepository.GetAllContinents().Where(c => c.Id == country.ParentId).FirstOrDefault();
            if (continent == null)
            {
                _worldTreeView.RecreateTreeFromScratch();
                return;
            }

            foreach (TreeNode rootNode in _worldTreeView.Nodes)
            {
                rootNode.Expand();

                foreach (TreeNode continentNode in rootNode.Nodes)
                {
                    if (continentNode.Text != continent.FieldName) continue;

                    continentNode.Expand();

                    foreach (TreeNode countryNode in continentNode.Nodes)
                    {
                        if (countryNode.Text != country.FieldName) continue;

                        countryNode.Expand();

                        foreach (TreeNode stateNode in countryNode.Nodes)
                        {
                            if (stateNode.Text != state.FieldName) continue;

                            stateNode.Expand();

                            foreach (TreeNode cityNode in stateNode.Nodes)
                            {
                                if (cityNode.Tag.ToString() == city.FieldName)
                                {
                                    cityNode.Expand();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
