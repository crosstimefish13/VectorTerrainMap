using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TerrainMapGUILibrary.Extensions;
using TerrainMapGUILibrary.Themes;
using static TerrainMapGUILibrary.Controls.ProcessControl.ProcessItemCollection;

namespace TerrainMapGUILibrary.Controls
{
    [DefaultProperty("Items")]
    [DesignTimeVisible(true)]
    [ToolboxItem(true)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Controls")]
    public sealed class ProcessControl : ControlExtension
    {
        private int selectedIndex;

        private Button btnBack;

        private Button btnNext;

        private ControlExtension ceContent;

        private ControlExtension cePanel;


        internal ObservableCollection<ProcessItem> InnerItems { get; private set; }


        [Category("Function")]
        [Description("The process items.")]
        [MergableProperty(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProcessItemCollection Items { get; private set; }

        [Category("Function")]
        [Description("The selected process item.")]
        [DefaultValue(-1)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                if (Items.Count > 1 && selectedIndex >= 0 && selectedIndex < Items.Count - 1)
                { btnNext.Enabled = true; }
                else { btnNext.Enabled = false; }

                if (Items.Count > 1 && selectedIndex > 0 && selectedIndex < Items.Count)
                { btnBack.Enabled = true; }
                else { btnBack.Enabled = false; }

                Items.SetSelected(selectedIndex);
            }
        }


        public ProcessControl()
        {
            selectedIndex = -1;
            InnerItems = new ObservableCollection<ProcessItem>();
            Items = new ProcessItemCollection() { Owner = this };

            InitializeComponent();
        }


        private void InitializeComponent()
        {
            btnBack = new Button();
            btnNext = new Button();
            ceContent = new ControlExtension();
            cePanel = new ControlExtension();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Text = "< Back";
            btnBack.Font = FontTheme.Normal();
            btnBack.Location = new Point(345, 267);
            btnBack.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBack.Enabled = false;
            btnBack.Click += (sender, e) => { SelectedIndex -= 1; };
            Controls.Add(btnBack);
            // 
            // btnNext
            // 
            btnNext.Text = "Next >";
            btnNext.Font = FontTheme.Normal();
            btnNext.Location = new Point(430, 267);
            btnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNext.Enabled = false;
            btnNext.Click += (sender, e) => { SelectedIndex += 1; };
            Controls.Add(btnNext);
            // 
            // ceContent
            // 
            ceContent.Location = new Point(185, 10);
            ceContent.Size = new Size(320, 247);
            ceContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            cePanel.Controls.Add(ceContent);
            // 
            // cePanel
            // 
            cePanel.Controls.Add(ceContent);
            cePanel.BackColor = SystemColors.Control;
            cePanel.Location = new Point(0, 0);
            cePanel.Size = new Size(515, 257);
            cePanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            Controls.Add(cePanel);
            // 
            // this
            // 
            BackColor = SystemColors.Control;
            Size = new Size(515, 300);
            ResumeLayout(false);
            PerformLayout();
        }

        internal static void RefreshItems(ControlExtension owner, ObservableCollection<object> items)
        {
            if (owner == null || items == null) { return; }

            owner.Controls.Clear();
            foreach (var item in items)
            {
                var processItem = item as ProcessItem;
                processItem.Owner = owner;
                processItem.Items = items;
                processItem.AddToOwner();
            }

            owner.Invalidate();
        }


        internal void SetSelected(int index)
        {
            if (Owner == null || items == null) { return; }

            foreach (var item in items)
            {
                var processItem = item as ProcessItem;
                bool isSelected = items.IndexOf(item) == index;
                (item as ProcessItem).SetSelected(isSelected);
            }
        }


        [DefaultProperty("Header")]
        [DesignTimeVisible(false)]
        [ToolboxItem(false)]
        [TypeConverter(typeof(ProcessItemConverter))]
        public sealed class ProcessItem : PanelExtension
        {
            private Label lblMark;

            private Label lblHeader;

            private Label lblDescription;


            [Category("Function")]
            [Description("Header displays on left steps panel.")]
            [DefaultValue("")]
            [Browsable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public string Header
            {
                get { return lblHeader.Text; }
                set
                {
                    lblHeader.Text = value;
                    RefreshItems(Owner, Items);
                }
            }

            [Category("Function")]
            [Description("Description displays on top of content panel.")]
            [DefaultValue("")]
            [Browsable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public string Description
            {
                get { return lblDescription.Text; }
                set
                {
                    lblDescription.Text = value;
                    RefreshItems(Owner, Items);
                }
            }


            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override bool AutoSize
            {
                get { return base.AutoSize; }
                set { base.AutoSize = value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override AnchorStyles Anchor
            {
                get { return base.Anchor; }
                set { base.Anchor = value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override DockStyle Dock
            {
                get { return base.Dock; }
                set { base.Dock = value; }
            }

            [DefaultValue(typeof(Size), "0, 0")]
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override Size MaximumSize
            {
                get { return base.MaximumSize; }
                set { base.MaximumSize = value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override Size MinimumSize
            {
                get { return base.MinimumSize; }
                set { base.MinimumSize = value; }
            }


            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Enabled
            {
                get { return base.Enabled; }
                set { base.Enabled = value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point Location
            {
                get { return base.Location; }
                set { base.Location = value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size PreferredSize
            {
                get { return base.PreferredSize; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Visible
            {
                get { return base.Visible; }
                set { base.Visible = value; }
            }


            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler AutoSizeChanged
            {
                add { base.AutoSizeChanged += value; }
                remove { base.AutoSizeChanged -= value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler DockChanged
            {
                add { base.DockChanged += value; }
                remove { base.DockChanged -= value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler EnabledChanged
            {
                add { base.EnabledChanged += value; }
                remove { base.EnabledChanged -= value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler LocationChanged
            {
                add { base.LocationChanged += value; }
                remove { base.LocationChanged -= value; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler VisibleChanged
            {
                add { base.VisibleChanged += value; }
                remove { base.VisibleChanged -= value; }
            }


            public ProcessItem()
                : this("", "")
            { }

            public ProcessItem(string header = "", string description = "")
            {
                InitializeComponent();

                Owner = null;
                Items = null;
                Header = header;
                Description = description;
            }


            public override string ToString()
            {
                return $"ProcessItem: {{{Header}}}";
            }


            internal void AddToOwner(ControlExtension panel, ObservableCollection<ProcessItem> items)
            {
                if (panel == null || items == null) { return; }

                panel.SuspendLayout();
                // 
                // lblDescription
                // 
                lblDescription.Enabled = false;
                lblDescription.Visible = false;
                panel.Controls.Add(lblDescription);
                // 
                // this
                // 
                int ceContentLocationY = Items.Max(i => (i as ProcessItem).lblDescription.Bounds.Bottom) + 10;
                //Location = new Point(185, ceContentLocationY);
                //Size = new Size(Owner.Width - 195, Owner.Height - ceContentLocationY);
                Dock = DockStyle.Fill;
                Enabled = false;
                Visible = false;
                Owner.Controls.Add(this);
                // 
                // lblMark
                // 
                int lblMarkOffsetLocationY = ceContentLocationY;
                int itemIndex = Items.IndexOf(this);
                if (itemIndex > 0)
                { lblMarkOffsetLocationY = (Items[itemIndex - 1] as ProcessItem).lblHeader.Bounds.Bottom + 10; }

                lblMark.Location = new Point(10, lblMarkOffsetLocationY);
                Owner.Controls.Add(lblMark);
                // 
                // lblHeader
                // 
                lblHeader.Location = new Point(25, lblMarkOffsetLocationY + 1);
                Owner.Controls.Add(lblHeader);
                // 
                // Owner
                // 
                Owner.ResumeLayout(false);
                Owner.PerformLayout();
            }

            internal void SetSelected(bool isSelected)
            {
                if (isSelected == true)
                {
                    lblMark.Text = "■";
                    lblDescription.Enabled = true;
                    lblDescription.Visible = true;
                    Enabled = true;
                    Visible = true;
                }
                else
                {
                    lblMark.Text = "□";
                    lblDescription.Enabled = false;
                    lblDescription.Visible = false;
                    Enabled = false;
                    Visible = false;
                }
            }


            private void InitializeComponent()
            {
                lblMark = new Label();
                lblHeader = new Label();
                lblDescription = new Label();
                // 
                // lblMark
                // 
                lblMark.Text = "□";
                lblMark.Font = FontTheme.Normal();
                lblMark.Location = new Point(10, 10);
                lblMark.Size = new Size(15, 15);
                lblMark.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                // 
                // lblHeader
                // 
                lblHeader.Font = FontTheme.Normal();
                lblHeader.Location = new Point(10, 10);
                lblHeader.MaximumSize = new Size(150, 0);
                lblHeader.AutoSize = true;
                lblHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                // 
                // lblDescription
                // 
                lblDescription.Font = FontTheme.Normal();
                lblDescription.Location = new Point(185, 10);
                lblDescription.MaximumSize = new Size(300, 0);
                lblDescription.AutoSize = true;
                lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                // 
                // ceContent
                // 
                Location = new Point(180, 10);
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            }
        }


        public sealed class ProcessItemConverter : ExpandableObjectConverter
        {
            public ProcessItemConverter()
            { }


            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(InstanceDescriptor))
                {
                    return true;
                }

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                Type destinationType)
            {
                if (value != null && destinationType != null)
                {
                    if (value is ProcessItem && destinationType == typeof(InstanceDescriptor))
                    {
                        // convert from instance to ProcessItem
                        var source = value as ProcessItem;
                        var ci = typeof(ProcessItem).GetConstructor(new Type[] { typeof(string), typeof(string) });
                        if (ci != null)
                        {
                            var instance = new InstanceDescriptor(ci,
                                new object[] { source.Header, source.Description });
                            return instance;
                        }
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }


        [ListBindable(false)]
        public sealed class ProcessItemCollection : IList, ICollection, IEnumerable
        {
            object IList.this[int index]
            {
                get { return Owner.InnerItems[index]; }
                set { Owner.InnerItems[index] = value as ProcessItem; }
            }

            bool IList.IsFixedSize
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get { return this; }
            }

            bool ICollection.IsSynchronized
            {
                get { return true; }
            }


            internal ProcessControl Owner { get; set; }


            public ProcessItem this[int index]
            {
                get { return Owner.InnerItems[index]; }
                set { Owner.InnerItems[index] = value; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public int Count
            {
                get { return Owner.InnerItems.Count; }
            }


            public ProcessItemCollection()
            {
                Owner = null;
            }


            public int Add(ProcessItem value)
            {
                Owner.InnerItems.Add(value);
                return Owner.InnerItems.Count - 1;
            }

            public void Clear()
            {
                Owner.InnerItems.Clear();
            }

            public bool Contains(ProcessItem value)
            {
                return Owner.InnerItems.Contains(value);
            }

            public void CopyTo(IEnumerable<ProcessItem> array, int index)
            {
                Owner.InnerItems.CopyTo(array.ToArray(), index);
            }

            public IEnumerator GetEnumerator()
            {
                return Owner.InnerItems.Cast<ProcessItem>().GetEnumerator();
            }

            public int IndexOf(ProcessItem value)
            {
                return Owner.InnerItems.IndexOf(value);
            }

            public void Insert(int index, ProcessItem value)
            {
                Owner.InnerItems.Insert(index, value);
            }

            public void Remove(ProcessItem value)
            {
                Owner.InnerItems.Remove(value);
            }

            public void RemoveAt(int index)
            {
                Owner.InnerItems.RemoveAt(index);
            }


            int IList.Add(object value)
            {
                Owner.InnerItems.Add(value as ProcessItem);
                return Owner.InnerItems.Count - 1;
            }

            bool IList.Contains(object value)
            {
                return Owner.InnerItems.Contains(value);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                Owner.InnerItems.CopyTo(array.Cast<ProcessItem>().ToArray(), index);
            }

            int IList.IndexOf(object value)
            {
                return Owner.InnerItems.IndexOf(value as ProcessItem);
            }

            void IList.Insert(int index, object value)
            {
                Owner.InnerItems.Insert(index, value as ProcessItem);
            }

            void IList.Remove(object value)
            {
                Owner.InnerItems.Remove(value as ProcessItem);
            }
        }
    }
}
