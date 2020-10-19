using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
// ReSharper disable RedundantBaseConstructorCall

namespace Playground.Winforms.Forms.Examples.WorldTreeViewEx
{
    public enum SelectedNodeLevels
    {
        // ReSharper disable once InconsistentNaming
        ROOT,
        // ReSharper disable once InconsistentNaming
        CONTINENT,
        // ReSharper disable once InconsistentNaming
        COUNTRY,
        // ReSharper disable once InconsistentNaming
        STATE,
        // ReSharper disable once InconsistentNaming
        CITY
    }

    internal abstract class WorldTreeNode : TreeNode
    {
        // ReSharper disable once EmptyConstructor
        // ReSharper disable once RedundantBaseConstructorCall
        public WorldTreeNode() : base() { }

        public string ToolTipTextStr = string.Empty;

        public SelectedNodeLevels SelectedNodeLevel;

        public abstract void Create(string name, ArrayList parameterList);

        public abstract void Refresh(ArrayList parameterList);

        public List<string> GetPathFromRoot()
        {
            if (Parent == null)
            {
                return new List<string>() { Text };
            }

            List<string> path = ((WorldTreeNode)Parent).GetPathFromRoot();
            path.Add(Text);
            return path;
        }

        public WorldTreeViewFilters WorldTreeViewFilter => ((WorldTreeView)TreeView).WorldTreeViewFilters;

        public ContextMenu GetRightClickMenu()
        {
            int _roleID = 1;
            string roleName = "ADMIN";

            return GetRightClickMenu(_roleID, roleName);
        }

        protected virtual ContextMenu GetRightClickMenu(int roleId, string roleName)
        {
            var rightClickMenu = new ContextMenu();

            var menuItem = new MenuItem("Root-Level Menu");
            rightClickMenu.MenuItems.Add(menuItem);

            MenuItem miRole = new MenuItem("Role: " + roleName);
            miRole.Tag = roleId;
            menuItem.MenuItems.Add(miRole);

            if ((IsExpanded))
            {
                MenuItem miRefresh = new MenuItem("Refresh");
                miRefresh.Click += miRefresh_Click;
                menuItem.MenuItems.Add(miRefresh);
            }

            return rightClickMenu;
        }

        private void miRefresh_Click(object sender, EventArgs e)
        {
            var parameterList = new ArrayList();
            Refresh(parameterList);
            try
            {
                TreeView.SelectedNode = this;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        public virtual void OnMouseDoubleClick()
        {

        }

        public TreeNode GetAncestorOfType(Type type)
        {
            Type treeNodeType = (new TreeNode()).GetType();
            // ReSharper disable once CheckForReferenceEqualityInstead.1
            if (type.Equals(treeNodeType) || type.IsAssignableFrom(treeNodeType))
            {
                return null;
            }

            TreeNode ancestor = this;
            while (ancestor != null)
            {
                Type ancestorType = ancestor.GetType();
                // ReSharper disable once CheckForReferenceEqualityInstead.1
                if (ancestorType.Equals(type) || ancestorType.IsAssignableFrom(type))
                {
                    return ancestor;
                }
                ancestor = ancestor.Parent;
            }

            return null;
        }

        public virtual ContinentTreeNode GetContinentNode()
        {
            return GetAncestorOfType((new ContinentTreeNode()).GetType()) as ContinentTreeNode;
        }

        //public virtual CountryTreeNode GetCountryNode()
        //{
        //    return GetAncestorOfType((new CountryTreeNode()).GetType()) as CountryTreeNode;
        //}

        //public virtual StateTreeNode GetStateNode()
        //{
        //    return GetAncestorOfType((new StateTreeNode()).GetType()) as StateTreeNode;
        //}

        //public virtual CityTreeNode GetCityNode()
        //{
        //    return GetAncestorOfType((new CityTreeNode()).GetType()) as CityTreeNode;
        //}

    }

    internal abstract class WorldParentTreeNode : WorldTreeNode
    {
        protected bool ChildrenAreLoaded;
        // ReSharper disable once InconsistentNaming
        protected int _continentId = -1;
        // ReSharper disable once InconsistentNaming
        protected int _countryId = -1;
        // ReSharper disable once InconsistentNaming
        protected int _stateId = -1;

        public int ContinentId => _continentId;

        public int CountryId => _countryId;

        public int StateId => _stateId;

        // ReSharper disable once PublicConstructorInAbstractClass
        public WorldParentTreeNode()
            // ReSharper disable once RedundantBaseConstructorCall
            : base()
        {
            Nodes.Add(new TreeNode());
        }

        public override void Refresh(ArrayList parameterList)
        {
            //RefreshSelf(parameterList);

            if (ChildrenAreLoaded)
            {
                if (IsExpanded)
                {
                    //refresh children...
                    var newChildList = GetChildList();
                    var oldChildList = new Dictionary<Type, SortedDictionary<string, TreeNode>>();

                    //make a temporary copy of the child nodes under this node (so the tree isn't redrawn every time we remove one
                    var childNodesCopy = new List<TreeNode>();
                    foreach (TreeNode node in Nodes)
                    {
                        childNodesCopy.Add(node);
                    }
                    Nodes.Clear();

                    //put the old nodes in their data structure
                    foreach (TreeNode node in childNodesCopy)
                    {
                        Type type = node.GetType();
                        SortedDictionary<string, TreeNode> dictionary;

                        if (oldChildList.ContainsKey(type))
                        {
                            dictionary = oldChildList[type];
                        }
                        else
                        {
                            dictionary = new SortedDictionary<string, TreeNode>();
                            oldChildList.Add(type, dictionary);
                        }
                        dictionary.Add(node.Tag.ToString(), node);
                    }
                    childNodesCopy.Clear(); //purely for garbage collection

                    //run through the new list, one type at a time
                    foreach (Type type in newChildList.Keys)
                    {
                        Dictionary<string, ArrayList> newDictionary = newChildList[type];
                        if (oldChildList.ContainsKey(type))
                        {
                            SortedDictionary<string, TreeNode> oldDictionary = oldChildList[type];

                            //go through the nodes in the order they were found in the new list
                            foreach (string name in newDictionary.Keys)
                            {
                                if (oldDictionary.ContainsKey(name))
                                {
                                    //refresh the old node
                                    WorldTreeNode child = oldDictionary[name] as WorldTreeNode;
                                    if (child != null)
                                    {
                                        //Below codes will change the image based on if statement condition.
                                        //if (type.Name == "ContinentTreeNode")
                                        //{
                                        //    if (newChildList[type][name][1].ToString() == "SWR")
                                        //    {
                                        //        child.SelectedImageIndex = 4;
                                        //        child.ImageIndex = 4;
                                        //    }
                                        //    else
                                        //    {
                                        //        child.SelectedImageIndex = 0;
                                        //        child.ImageIndex = 0;
                                        //    }
                                        //}

                                        Nodes.Add(child);
                                        child.Refresh(newDictionary[name]);
                                    }
                                }
                                else
                                {
                                    //create a new node
                                    AddChild(type, name, newDictionary[name]);
                                }
                            }
                        }
                        else
                        {
                            foreach (string name in newDictionary.Keys)
                            {
                                AddChild(type, name, newDictionary[name]);
                            }
                        }
                    }
                }
                else
                {
                    Nodes.Clear();
                    Nodes.Add(new TreeNode());
                    ChildrenAreLoaded = false;
                }
            }
        }

        public virtual void OnExpand()
        {

            if (ChildrenAreLoaded) { return; }

            Dictionary<Type, Dictionary<string, ArrayList>> newChildList = GetChildList();

            Nodes.Clear();
            foreach (Type type in newChildList.Keys)
            {
                Dictionary<string, ArrayList> newDictionary = newChildList[type];
                foreach (string name in newDictionary.Keys)
                {
                    AddChild(type, name, newDictionary[name]);
                }
            }

            ChildrenAreLoaded = true;
        }

        protected virtual void RefreshSelf(ArrayList parameterList)
        {
            if (parameterList != null && parameterList.Count > 0)
            {
                if ((int)parameterList[0] != -1)
                {
                    _continentId = (int)parameterList[0];
                }
            }
            else if (Parent is WorldParentTreeNode)
            {
                WorldParentTreeNode parentNode = (WorldParentTreeNode)Parent;

                if (parentNode.ContinentId != -1)
                {
                    //DataTable binaryTable = tqtBaw.ExecuteQuery(
                    //    " SELECT DIVISION_ID"
                    //    + " FROM NFL_DIVISION"
                    //    + " WHERE DIVISION_ID = " + parentNode.DivisionID);

                    //if (binaryTable.Rows.Count == 0)
                    //{
                    //    //if (this.GetType().ToString() != "BawDataViewer.WaferTreeNode")
                    //    //Parent.Nodes.Remove(this);
                    //}
                    //else
                    //{
                    //    _ContinentID = Convert.ToInt32(binaryTable.Rows[0]["DIVISION_ID"]);
                    //}
                }
                else
                {
                    Parent.Nodes.Remove(this);
                }
            }
        }

        protected abstract Dictionary<Type, Dictionary<string, ArrayList>> GetChildList();

        private static Type[] emptyTypeArray = new Type[] { };

        private static object[] emptyObjectArray = new object[] { };

        private void AddChild(Type type, string name, ArrayList parameterList)
        {
            ConstructorInfo constructor = type.GetConstructor(emptyTypeArray);

            // ReSharper disable once PossibleNullReferenceException
            WorldTreeNode child = (WorldTreeNode)constructor.Invoke(emptyObjectArray);

            child.Create(name, parameterList);

            //This code changes node icon to something different!!!
            //if (type.Name == "TeamTreeNode")
            //{
            //    if (parameterList[parameterList.Count - 1].ToString() == "SWR")
            //    {
            //        child.SelectedImageIndex = 4;
            //        child.ImageIndex = 4;
            //    }
            //}
            Nodes.Add(child);
        }

        private void SetUpToolTipText(string countryName)
        {
            ToolTipTextStr = string.Empty;
            ToolTipTextStr = "{" + countryName + "}";
            ToolTipText = ToolTipTextStr;
        }
    }

    internal class WorldRootTreeNode : WorldParentTreeNode
    {
        public WorldRootTreeNode()
            : base()
        {
            ImageIndex = 0;
            SelectedImageIndex = 0;
        }

        public override void Create(string name, ArrayList parameterList)
        {
            Tag = Text = @"World";
            SelectedNodeLevel = SelectedNodeLevels.ROOT;
            SetUpToolTipText();
        }

        private void SetUpToolTipText()
        {
            var continents = WorldTreeViewRepository.GetAllContinents();
            ToolTipTextStr = string.Empty;
            int continentCounter = 0;

            foreach (ContinentObject item in continents)
            {
                ToolTipTextStr += item.Id + " - " + item.FieldName + "\n";
                continentCounter++;
            }

            if (!string.IsNullOrEmpty(ToolTipTextStr))
            {
                ToolTipTextStr += "\nTotal Continents: " + continentCounter;
                ToolTipText = ToolTipTextStr;
            }
        }

        protected override void RefreshSelf(ArrayList parameterList)
        {
            //nothing to do...
        }

        protected override Dictionary<Type, Dictionary<string, ArrayList>> GetChildList()
        {
            var childList = new Dictionary<Type, Dictionary<string, ArrayList>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            childList.Add(Type.GetType(@"Playground.Winforms.Forms.Examples.WorldTreeViewEx.ContinentTreeNode"), GetContinentList());

            return childList;
        }

        private Dictionary<string, ArrayList> GetContinentList()
        {
            var continentList = new Dictionary<string, ArrayList>();

            List<ContinentObject> continents;

            if (!string.IsNullOrEmpty(WorldTreeViewFilter.ContinentName))
            {
                continents = WorldTreeViewRepository.GetContinentsByContinentName(WorldTreeViewFilter.ContinentName);
            }
            else if (!string.IsNullOrEmpty(WorldTreeViewFilter.CountryName))
            {
                continents = WorldTreeViewRepository.GetContinentsByCountryName(WorldTreeViewFilter.CountryName);
            }
            else if (!string.IsNullOrEmpty(WorldTreeViewFilter.StateName))
            {
                continents = WorldTreeViewRepository.GetContinentsByStateName(WorldTreeViewFilter.StateName);
            }
            else if (!string.IsNullOrEmpty(WorldTreeViewFilter.CityName))
            {
                continents = WorldTreeViewRepository.GetContinentsByCityName(WorldTreeViewFilter.CityName);
            }
            else
            {
                continents = WorldTreeViewRepository.GetAllContinents();
            }

            if (continents.Count > 0)
            {
                foreach (ContinentObject item in continents)
                {
                    if (!continentList.ContainsKey(item.FieldName))
                    {
                        ArrayList parameterList = new ArrayList();
                        parameterList.Add(item.Id);
                        parameterList.Add(-1);
                        continentList.Add(item.FieldName, parameterList);
                    }
                }
            }

            return continentList;
        }
    }

    internal class ContinentTreeNode : WorldParentTreeNode
    {
        public ContinentTreeNode()
            : base()
        {
            ImageIndex = 1;
            SelectedImageIndex = 1;
        }

        public override void Create(string name, ArrayList parameterList)
        {
            Tag = Text = name;
            _continentId = (int)parameterList[0];
            SelectedNodeLevel = SelectedNodeLevels.CONTINENT;
            SetUpToolTipText();
        }

        protected override void RefreshSelf(ArrayList parameterList)
        {
            base.RefreshSelf(parameterList);

            SetUpToolTipText();
        }

        private void SetUpToolTipText()
        {
            ToolTipTextStr = string.Empty;
            ToolTipTextStr = "{ " + Tag + " }";
            ToolTipText = ToolTipTextStr;
        }

        protected override Dictionary<Type, Dictionary<string, ArrayList>> GetChildList()
        {
            var childList = new Dictionary<Type, Dictionary<string, ArrayList>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            childList.Add(Type.GetType(@"Playground.Winforms.Forms.Examples.WorldTreeViewEx.CountryTreeNode"), GetCountryList());
            return childList;
        }

        private Dictionary<string, ArrayList> GetCountryList()
        {
            var countryList = new Dictionary<string, ArrayList>();

            if (_continentId != -1)
            {
                var countries = WorldTreeViewRepository.GetCountriesByContinentId(_continentId);

                foreach (CountryObject item in countries)
                {
                    if (!countryList.ContainsKey(item.FieldName))
                    {
                        ArrayList parameterList = new ArrayList();
                        parameterList.Add(item.Id);
                        parameterList.Add(item.ParentId);
                        countryList.Add(item.FieldName, parameterList);
                    }
                }
            }

            return countryList;
        }

        protected override ContextMenu GetRightClickMenu(int roleId, string roleName)
        {
            var rightClickMenu = new ContextMenu();

            var miContinentTreeNode = new MenuItem("Continent-Level Menu");
            rightClickMenu.MenuItems.Add(miContinentTreeNode);

            //Continent info
            MenuItem miInfo = new MenuItem("Continent Info");
            miInfo.Click += miInfo_Click;
            miContinentTreeNode.MenuItems.Add(miInfo);

            return rightClickMenu;
        }

        private void miInfo_Click(object sender, EventArgs e)
        {
            var msg = $"Tag: {Tag}\nContinent ID: {_continentId.ToString()}";

            MessageBox.Show(msg, @"Continent Info");
        }
    }

    internal class CountryTreeNode : WorldParentTreeNode
    {
        public CountryTreeNode()
            : base()
        {
            ImageIndex = 2;
            SelectedImageIndex = 2;
        }

        public override void Create(string name, ArrayList parameterList)
        {
            Tag = Text = name;
            _countryId = (int)parameterList[0];
            _continentId = (int)parameterList[1];
            SelectedNodeLevel = SelectedNodeLevels.COUNTRY;
            SetUpToolTipText();
        }

        private void SetUpToolTipText()
        {
            ToolTipTextStr = string.Empty;
            ToolTipTextStr = "{ " + Tag + " }";
            ToolTipText = ToolTipTextStr;
        }

        public override void OnExpand()
        {
            base.OnExpand();
        }

        protected override void RefreshSelf(ArrayList parameterList)
        {
            base.RefreshSelf(parameterList);
        }

        protected override ContextMenu GetRightClickMenu(int roleId, string roleName)
        {
            var rightClickMenu = new ContextMenu();
            var miTeamTreeNode = new MenuItem("Country-Level Menu");
            rightClickMenu.MenuItems.Add(miTeamTreeNode);

            var miInfo = new MenuItem("Country Info");
            miInfo.Click += miInfo_Click;
            miTeamTreeNode.MenuItems.Add(miInfo);

            return rightClickMenu;
        }

        private void miInfo_Click(object sender, EventArgs e)
        {
            var msg = $"Tag: {Tag}\nCountry ID: {_countryId.ToString()}";

            MessageBox.Show(msg, @"Country Info");
        }

        protected override Dictionary<Type, Dictionary<string, ArrayList>> GetChildList()
        {
            var childList = new Dictionary<Type, Dictionary<string, ArrayList>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            childList.Add(Type.GetType(@"Playground.Winforms.Forms.Examples.WorldTreeViewEx.StateTreeNode"), GetStateList());

            return childList;
        }

        private Dictionary<string, ArrayList> GetStateList()
        {
            var stateList = new Dictionary<string, ArrayList>();

            if (_countryId != -1)
            {
                var states = WorldTreeViewRepository.GetStatesByCountryId(_countryId);

                if (states.Count > 0)
                {
                    foreach (StateObject item in states)
                    {
                        if (!stateList.ContainsKey(item.FieldName))
                        {
                            ArrayList parameterList = new ArrayList();
                            parameterList.Add(item.Id);
                            parameterList.Add(item.ParentId);

                            stateList.Add(item.FieldName, parameterList);
                        }
                    }
                }
            }

            return stateList;
        }
    }

    internal class StateTreeNode : WorldParentTreeNode
    {
        public StateTreeNode()
            : base()
        {
            ImageIndex = 3;
            SelectedImageIndex = 3;
        }

        public override void Create(string name, ArrayList parameterList)
        {
            Tag = Text = name;
            _stateId = (int)parameterList[0];
            _countryId = (int)parameterList[1];
            SelectedNodeLevel = SelectedNodeLevels.STATE;
            SetUpToolTipText();
        }

        private void SetUpToolTipText()
        {
            ToolTipTextStr = string.Empty;
            ToolTipTextStr = "{ " + Tag + " }";
            ToolTipText = ToolTipTextStr;
        }

        public override void OnExpand()
        {
            base.OnExpand();
        }

        protected override void RefreshSelf(ArrayList parameterList)
        {
            base.RefreshSelf(parameterList);
        }

        protected override ContextMenu GetRightClickMenu(int roleId, string roleName)
        {
            var rightClickMenu = new ContextMenu();
            var miTeamTreeNode = new MenuItem("State-Level Menu");
            rightClickMenu.MenuItems.Add(miTeamTreeNode);

            var miInfo = new MenuItem("State Info");
            miInfo.Click += miInfo_Click;
            miTeamTreeNode.MenuItems.Add(miInfo);

            return rightClickMenu;
        }

        private void miInfo_Click(object sender, EventArgs e)
        {
            var msg = $"Tag: {Tag}\nState ID: {_stateId.ToString()}";

            MessageBox.Show(msg, @"State Info");
        }

        protected override Dictionary<Type, Dictionary<string, ArrayList>> GetChildList()
        {
            var childList = new Dictionary<Type, Dictionary<string, ArrayList>>();

            // ReSharper disable once AssignNullToNotNullAttribute
            childList.Add(Type.GetType(@"Playground.Winforms.Forms.Examples.WorldTreeViewEx.CityTreeNode"), GetCityList());

            return childList;
        }

        private Dictionary<string, ArrayList> GetCityList()
        {
            var cityList = new Dictionary<string, ArrayList>();

            if (_countryId != -1)
            {
                var cities = WorldTreeViewRepository.GetCitiesByStateId(_stateId);

                if (cities.Count > 0)
                {
                    foreach (CityObject item in cities)
                    {
                        if (!cityList.ContainsKey(item.FieldName))
                        {
                            ArrayList parameterList = new ArrayList();
                            parameterList.Add(item.Id);
                            parameterList.Add(item.ParentId);

                            cityList.Add(item.FieldName, parameterList);
                        }
                    }
                }
            }

            return cityList;
        }
    }

    internal abstract class WorldLeafTreeNode : WorldTreeNode
    {
        protected int _cityId;
        protected int _stateId;

        public int CityId => _cityId;

        public int StateId => _stateId;

        // ReSharper disable once PublicConstructorInAbstractClass
        public WorldLeafTreeNode()
            : base()
        {
            ImageIndex = 4;
            SelectedImageIndex = 4;
        }
    }

    internal class CityTreeNode : WorldLeafTreeNode
    {
        public CityTreeNode()
            : base()
        {

        }

        public override void Create(string name, ArrayList parameterList)
        {
            Tag = Text = name;
            _cityId = (int)parameterList[0];
            _stateId = (int)parameterList[1];
            SetUpText(_cityId.ToString());
            SelectedNodeLevel = SelectedNodeLevels.CITY;
            SetUpToolTipText();
        }

        private void SetUpToolTipText()
        {
            ToolTipTextStr = string.Empty;
            ToolTipTextStr = "{ " + Tag + " }";
            ToolTipText = ToolTipTextStr;
        }

        private void SetUpText(string str)
        {
            Text = Tag + @" [ " + str + @" ]";
        }

        public override void Refresh(ArrayList parameterList)
        {
            if (parameterList != null && parameterList.Count > 0)
            {
                SetUpText(parameterList[0].ToString());
            }

            TreeView.SelectedNode = this;
        }

        protected override ContextMenu GetRightClickMenu(int roleId, string roleName)
        {
            var rightClickMenu = new ContextMenu();
            var miTreeDataTreeNode = new MenuItem("City-Level Menu");
            rightClickMenu.MenuItems.Add(miTreeDataTreeNode);

            var miAdd = new MenuItem("Info");
            miAdd.Click += miInfo_Click;
            miTreeDataTreeNode.MenuItems.Add(miAdd);

            return rightClickMenu;
        }

        private void miInfo_Click(object sender, EventArgs e)
        {
            var msg = $"Tag: {Tag}\nCity ID: {_cityId.ToString()}";
            MessageBox.Show(msg, @"City Info");
        }
    }
}
