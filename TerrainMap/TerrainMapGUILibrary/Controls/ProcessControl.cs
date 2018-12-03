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

namespace TerrainMapGUILibrary.Controls
{
    [DefaultEvent("SelectChanged")]
    [DefaultProperty("Items")]
    [DesignTimeVisible(true)]
    [ToolboxItem(true)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Controls")]
    public sealed class ProcessControl : ControlExtension
    {
        private int selectedIndex;

        private ButtonExtension btnBack;

        private ButtonExtension btnNext;

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
                if (InnerItems.Count <= 0) { selectedIndex = -1; }
                else if (selectedIndex < -1 || selectedIndex >= InnerItems.Count) { selectedIndex = 0; }

                ChangeSelect();
            }
        }


        [Category("Function")]
        [Description("Occurs when select changed.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler SelectChanged;


        public ProcessControl()
        {
            selectedIndex = -1;
            InnerItems = new ObservableCollection<ProcessItem>();
            InnerItems.CollectionChanged += (sender, e) => { RefreshItems(); };
            Items = new ProcessItemCollection() { Owner = this };
            SelectChanged = null;

            InitializeComponent();
        }


        private void InitializeComponent()
        {
            btnBack = new ButtonExtension();
            btnNext = new ButtonExtension();
            ceContent = new ControlExtension();
            cePanel = new ControlExtension();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Text = "< Back";
            btnBack.Location = new Point(345, 267);
            btnBack.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBack.Enabled = false;
            btnBack.Click += (sender, e) => { SelectedIndex -= 1; };
            Controls.Add(btnBack);
            // 
            // btnNext
            // 
            btnNext.Text = "Next >";
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

        private void RefreshItems()
        {
            cePanel.SuspendLayout();
            ceContent.Controls.Clear();
            cePanel.Controls.Clear();
            foreach (var item in InnerItems)
            {
                // 
                // lblDescription
                // 
                item.DescriptionLabel.Enabled = false;
                item.DescriptionLabel.Visible = false;
                cePanel.Controls.Add(item.DescriptionLabel);
            }

            int offsetY = 10;
            if (InnerItems.Count > 0) { offsetY = InnerItems.Max(i => i.DescriptionLabel.Bounds.Bottom) + 10; }

            // 
            // ceContent
            // 
            ceContent.Location = new Point(185, offsetY);
            ceContent.Size = new Size(cePanel.Width - 195, cePanel.Height - offsetY);
            cePanel.Controls.Add(ceContent);

            foreach (var item in InnerItems)
            {
                // 
                // lblMark
                // 
                item.MarkLabel.Location = new Point(10, offsetY);
                cePanel.Controls.Add(item.MarkLabel);
                // 
                // lblHeader
                // 
                item.HeaderLabel.Location = new Point(25, offsetY + 1);
                cePanel.Controls.Add(item.HeaderLabel);
                // 
                // item
                // 
                item.Enabled = false;
                item.Visible = false;
                ceContent.Controls.Add(item);

                offsetY = item.HeaderLabel.Bounds.Bottom + 10;
            }

            // 
            // cePanel
            // 
            cePanel.ResumeLayout(false);
            cePanel.PerformLayout();

            if (InnerItems.Count > 0) { SelectedIndex = 0; }
            else { SelectedIndex = -1; }
        }

        private void ChangeSelect()
        {
            foreach (var item in InnerItems)
            {
                if (InnerItems.IndexOf(item) == selectedIndex)
                {
                    item.MarkLabel.Text = "■";
                    item.DescriptionLabel.Enabled = true;
                    item.DescriptionLabel.Visible = true;
                    item.Enabled = true;
                    item.Visible = true;
                }
                else
                {
                    item.MarkLabel.Text = "□";
                    item.DescriptionLabel.Enabled = false;
                    item.DescriptionLabel.Visible = false;
                    item.Enabled = false;
                    item.Visible = false;
                }
            }

            if (InnerItems.Count > 1 && selectedIndex >= 0 && selectedIndex < InnerItems.Count - 1)
            { btnNext.Enabled = true; }
            else { btnNext.Enabled = false; }

            if (InnerItems.Count > 1 && selectedIndex > 0 && selectedIndex < InnerItems.Count)
            { btnBack.Enabled = true; }
            else { btnBack.Enabled = false; }

            if (SelectChanged != null) { SelectChanged.Invoke(this, new EventArgs()); }
        }


        [DefaultProperty("Header")]
        [DesignTimeVisible(false)]
        [ToolboxItem(false)]
        [TypeConverter(typeof(ProcessItemConverter))]
        public sealed class ProcessItem : PanelExtension
        {
            internal LabelExtension MarkLabel { get; set; }

            internal LabelExtension HeaderLabel { get; set; }

            internal LabelExtension DescriptionLabel { get; set; }


            [Category("Function")]
            [Description("Header displays on left steps panel.")]
            [DefaultValue("")]
            [Browsable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public string Header
            {
                get { return HeaderLabel.Text; }
                set { HeaderLabel.Text = value; }
            }

            [Category("Function")]
            [Description("Description displays on top of content panel.")]
            [DefaultValue("")]
            [Browsable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public string Description
            {
                get { return DescriptionLabel.Text; }
                set { DescriptionLabel.Text = value; }
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

                Header = header;
                Description = description;
            }


            public override string ToString()
            {
                return $"ProcessItem: {{{Header}}}";
            }


            private void InitializeComponent()
            {
                MarkLabel = new LabelExtension();
                HeaderLabel = new LabelExtension();
                DescriptionLabel = new LabelExtension();
                // 
                // lblMark
                // 
                MarkLabel.Text = "□";
                MarkLabel.Location = new Point(10, 10);
                MarkLabel.Size = new Size(15, 15);
                MarkLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                // 
                // lblHeader
                // 
                HeaderLabel.Location = new Point(10, 10);
                HeaderLabel.MaximumSize = new Size(150, 0);
                HeaderLabel.AutoSize = true;
                HeaderLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                // 
                // lblDescription
                // 
                DescriptionLabel.Location = new Point(185, 10);
                DescriptionLabel.MaximumSize = new Size(300, 0);
                DescriptionLabel.AutoSize = true;
                DescriptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                // 
                // this
                // 
                Dock = DockStyle.Fill;
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
