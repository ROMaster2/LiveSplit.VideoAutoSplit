// Derivative of https://www.codeproject.com/Articles/14544/A-TreeView-Control-with-ComboBox-Dropdown-Nodes
// Originally made by Matt Valerio.

using System;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace LiveSplit.VAS.Models
{
    /// <summary>
    /// A class that inherits from TreeNode that lets you specify a ComboBox to be shown
    /// at the TreeNode's position
    /// </summary>
    public class DropDownTreeNode : TreeNode
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DropDownTreeNode"/> class.
        /// </summary>
        public DropDownTreeNode()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DropDownTreeNode"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public DropDownTreeNode(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DropDownTreeNode"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="children">The children.</param>
        public DropDownTreeNode(string text, TreeNode[] children)
            : base(text, children)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DropDownTreeNode"/> class.
        /// </summary>
        /// <param name="serializationInfo">A <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> containing the data to deserialize the class.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> containing the source and destination of the serialized stream.</param>
        public DropDownTreeNode(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DropDownTreeNode"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="imageIndex">Index of the image.</param>
        /// <param name="selectedImageIndex">Index of the selected image.</param>
        public DropDownTreeNode(string text, int imageIndex, int selectedImageIndex)
            : base(text, imageIndex, selectedImageIndex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DropDownTreeNode"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="imageIndex">Index of the image.</param>
        /// <param name="selectedImageIndex">Index of the selected image.</param>
        /// <param name="children">The children.</param>
        public DropDownTreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children)
            : base(text, imageIndex, selectedImageIndex, children)
        {
        }
        #endregion

        #region Property - ComboBox
        private ComboBox m_ComboBox = new ComboBox();

        /// <summary>
        /// Gets or sets the ComboBox.  Lets you access all of the properties of the internal ComboBox.
        /// </summary>
        /// <example>
        /// For example,
        /// <code>
        /// DropDownTreeNode node1 = new DropDownTreeNode("Some text");
        /// node1.ComboBox.Items.Add("Some text");
        /// node1.ComboBox.Items.Add("Some more text");
        /// node1.IsDropDown = true;
        /// </code>
        /// </example>
        /// <value>The combo box.</value>
        public ComboBox ComboBox
        {
            get
            {
                this.m_ComboBox.DropDownStyle = ComboBoxStyle.Simple;
                return this.m_ComboBox;
            }
            set
            {
                this.m_ComboBox = value;
                this.m_ComboBox.DropDownStyle = ComboBoxStyle.Simple;
            }
        }
        #endregion
    }

    /// <summary>
    /// Provides the usual TreeView control with the ability to edit the labels of the nodes
    /// by using a drop-down ComboBox.
    /// </summary>
    public class DropDownTreeView : TreeView
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DropDownTreeView"/> class.
        /// </summary>
        public DropDownTreeView()
            : base()
        {
        }
        #endregion

        // We'll use this variable to keep track of the current node that is being edited.
        // This is set to something (non-null) only if the node's ComboBox is being displayed.
        private DropDownTreeNode m_CurrentNode = null;

        /// <summary>
        /// Occurs when the <see cref="E:System.Windows.Forms.TreeView.NodeMouseClick"></see> event is fired
        /// -- that is, when a node in the tree view is clicked.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.TreeNodeMouseClickEventArgs"></see> that contains the event data.</param>
        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            // Are we dealing with a dropdown node?
            if (e.Node is DropDownTreeNode)
            {
                this.m_CurrentNode = (DropDownTreeNode)e.Node;

                // Need to add the node's ComboBox to the TreeView's list of controls for it to work
                this.Controls.Add(this.m_CurrentNode.ComboBox);

                // Set the bounds of the ComboBox, with a little adjustment to make it look right
                this.m_CurrentNode.ComboBox.SetBounds(
                    this.m_CurrentNode.TreeView.Width - 60,
                    this.m_CurrentNode.Bounds.Y,
                    60,
                    20);

                // Listen to the SelectedValueChanged event of the node's ComboBox
                this.m_CurrentNode.ComboBox.SelectedValueChanged += ComboBox_SelectedValueChanged;
                this.m_CurrentNode.ComboBox.DropDownClosed += ComboBox_DropDownClosed;

                // Now show the ComboBox
                this.m_CurrentNode.ComboBox.Show();
                this.m_CurrentNode.ComboBox.DroppedDown = true;
            }
            base.OnNodeMouseClick(e);
        }

        /// <summary>
        /// Handles the SelectedValueChanged event of the ComboBox control.
        /// Hides the ComboBox if an item has been selected in it.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void ComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            HideComboBox();
        }

        /// <summary>
        /// Handles the DropDownClosed event of the ComboBox control.
        /// Hides the ComboBox if the user clicks anywhere else on the TreeView or adjusts the scrollbars, or scrolls the mouse wheel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            HideComboBox();
        }

        /// <summary>
        /// Handles the <see cref="E:System.Windows.Forms.Control.MouseWheel"></see> event.
        /// Hides the ComboBox if the user scrolls the mouse wheel.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            HideComboBox();
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Method to hide the currently-selected node's ComboBox
        /// </summary>
        private void HideComboBox()
        {
            if (this.m_CurrentNode != null)
            {
                // Unregister the event listener
                this.m_CurrentNode.ComboBox.SelectedValueChanged -= ComboBox_SelectedValueChanged;
                this.m_CurrentNode.ComboBox.DropDownClosed -= ComboBox_DropDownClosed;

                // Copy the selected text from the ComboBox to the TreeNode
                this.m_CurrentNode.Text = this.m_CurrentNode.ComboBox.Text;

                // Hide the ComboBox
                this.m_CurrentNode.ComboBox.Hide();
                this.m_CurrentNode.ComboBox.DroppedDown = false;

                // Remove the control from the TreeView's list of currently-displayed controls
                this.Controls.Remove(this.m_CurrentNode.ComboBox);

                // And return to the default state (no ComboBox displayed)
                this.m_CurrentNode = null;
            }
        }

        /// <summary>
        /// Fixes double-clicking on checkboxes.
        /// </summary>
        ///
        /// See also:
        /// http://stackoverflow.com/questions/17356976/treeview-with-checkboxes-not-processing-clicks-correctly
        /// http://stackoverflow.com/questions/14647216/c-sharp-treeview-ignore-double-click-only-at-checkbox
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x203) // identified double click
            {
                var local_pos = PointToClient(Cursor.Position);
                var hit_test_info = HitTest(local_pos);

                if (hit_test_info.Location == TreeViewHitTestLocations.StateImage)
                {
                    m.Msg = 0x201; // if checkbox was clicked, turn into single click
                }
            }
            base.WndProc(ref m);
        }
    }
}
