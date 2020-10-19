using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Playground.Winforms.Forms.Examples.WorldTreeViewEx
{
    public class WorldTreeView : TreeView
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Color _backgroundColor = Color.AliceBlue;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private ImageList _imageListIcons;

        public WorldTreeViewFilters WorldTreeViewFilters { get; set; }

        public WorldTreeView(WorldTreeViewFilters filters)
            // ReSharper disable once RedundantBaseConstructorCall
            : base()
        {
            WorldTreeViewFilters = filters;

            _imageListIcons = new ImageList();
            _imageListIcons.Images.Add(Properties.Resources.World);
            _imageListIcons.Images.Add(Properties.Resources.Continent);
            _imageListIcons.Images.Add(Properties.Resources.Country);
            _imageListIcons.Images.Add(Properties.Resources.State);
            _imageListIcons.Images.Add(Properties.Resources.City);
            _imageListIcons.TransparentColor = Color.FromArgb(0, 255, 0);

            ImageList = _imageListIcons;
            ShowNodeToolTips = true;

            // ReSharper disable once RedundantDelegateCreation
            BeforeExpand += new TreeViewCancelEventHandler(WorldTreeView_BeforeExpand);
            // ReSharper disable once RedundantDelegateCreation
            NodeMouseClick += new TreeNodeMouseClickEventHandler(WorldTreeView_NodeMouseClick);
            // ReSharper disable once RedundantDelegateCreation
            NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(WorldTreeView_NodeMouseDoubleClick);
            // ReSharper disable once RedundantDelegateCreation
            AfterSelect += new TreeViewEventHandler(WorldTreeView_AfterSelect);

            RecreateTreeFromScratch();
        }

        public void RefreshTree()
        {
            WorldParentTreeNode node = Nodes[0] as WorldParentTreeNode;
            if (node != null)
            {
                node.Refresh(new ArrayList());
            }
        }

        public void RecreateTreeFromScratch()
        {
            Nodes.Clear();
            BackColor = _backgroundColor;

            var rootNode = new WorldRootTreeNode();
            Nodes.Add(rootNode);
            rootNode.Create(null, null);
            rootNode.Expand();
        }

        // ReSharper disable once UnusedMember.Local
        private void InitializeComponent()
        {
            SuspendLayout();
            ResumeLayout(false);
        }

        private void WorldTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            var node = e.Node as WorldParentTreeNode;

            if (node == null) { return; }
            node.OnExpand();
        }

        private void WorldTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                WorldTreeNode node = e.Node as WorldTreeNode;
                if (node != null)
                {
                    ContextMenu rightClickMenu = node.GetRightClickMenu();
                    rightClickMenu.Show(this, e.Location);
                }
            }
        }

        private void WorldTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            WorldTreeNode node = e.Node as WorldTreeNode;
            if (node != null)
            {
                node.OnMouseDoubleClick();
            }
        }

        private void WorldTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            HandleNodeSelection(e.Node as WorldTreeNode);
        }

        private void HandleNodeSelection(WorldTreeNode oNode)
        {
            switch (oNode.SelectedNodeLevel)
            {
                case SelectedNodeLevels.ROOT:
                    Console.WriteLine(@"Selected Node was: " + SelectedNodeLevels.ROOT);
                    break;
                case SelectedNodeLevels.CONTINENT:
                    Console.WriteLine(@"Selected Node was: " + SelectedNodeLevels.CONTINENT);
                    break;
                case SelectedNodeLevels.COUNTRY:
                    Console.WriteLine(@"Selected Node was : " + SelectedNodeLevels.COUNTRY);
                    break;
                case SelectedNodeLevels.STATE:
                    Console.WriteLine(@"Selected Node was : " + SelectedNodeLevels.STATE);
                    break;
                case SelectedNodeLevels.CITY:
                    Console.WriteLine(@"Selected Node was : " + SelectedNodeLevels.CITY);
                    break;
            }
        }
    }
}
