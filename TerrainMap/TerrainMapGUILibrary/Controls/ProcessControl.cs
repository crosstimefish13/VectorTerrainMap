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

        private ControlExtension cePanel;

        private Button btnBack;

        private Button btnNext;


        [Browsable(true)]
        [Category("Function")]
        [Description("The process items.")]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProcessItemCollection Items { get; private set; }

        [Browsable(true)]
        [Category("Function")]
        [Description("The selected process item.")]
        [MergableProperty(false)]
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


        [Browsable(true)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool TabStop
        {
            get { return btnBack.TabStop; }
            set
            {
                // control self tab stop always be false
                btnBack.TabStop = value;
                btnNext.TabStop = value;
                foreach (ProcessItem item in Items)
                {
                    item.SetTabStop(value);
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new int TabIndex
        {
            get { return btnBack.TabIndex; }
            set
            {
                btnBack.TabIndex = value;
                btnBack.TabIndex = value;
                foreach (ProcessItem item in Items)
                {
                    item.SetTabIndex(value);
                }
            }
        }


        public ProcessControl()
        {
            InitializeComponent();

            selectedIndex = -1;
            Items = new ProcessItemCollection() { Owner = cePanel };
        }


        private void InitializeComponent()
        {
            cePanel = new ControlExtension();
            btnBack = new Button();
            btnNext = new Button();
            SuspendLayout();
            // 
            // cePanel
            // 
            cePanel.BackColor = SystemColors.Control;
            cePanel.Location = new Point(0, 0);
            cePanel.Size = new Size(515, 257);
            cePanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            Controls.Add(cePanel);
            // 
            // btnBack
            // 
            btnBack.Text = "< Back";
            btnBack.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnBack.Location = new Point(345, 267);
            btnBack.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBack.Enabled = false;
            btnBack.Click += (sender, e) => { SelectedIndex -= 1; };
            Controls.Add(btnBack);
            // 
            // btnNext
            // 
            btnNext.Text = "Next >";
            btnNext.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnNext.Location = new Point(430, 267);
            btnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNext.Enabled = false;
            btnNext.Click += (sender, e) => { SelectedIndex += 1; };
            Controls.Add(btnNext);
            // 
            // this
            // 
            BackColor = SystemColors.Control;
            Size = new Size(515, 300);
            ResumeLayout(false);
            PerformLayout();
        }


        [ListBindable(false)]
        public sealed class ProcessItemCollection : IList, ICollection, IEnumerable
        {
            private ObservableCollection<object> items;


            object IList.this[int index]
            {
                get { return items[index]; }
                set { items[index] = value; }
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


            internal ControlExtension Owner { get; set; }


            public ProcessItem this[int index]
            {
                get { return items[index] as ProcessItem; }
                set { items[index] = value; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public int Count
            {
                get { return items.Count; }
            }


            public ProcessItemCollection()
            {
                items = new ObservableCollection<object>();
                items.CollectionChanged += (sender, e) => { RefreshItems(Owner, items); };

                Owner = null;
            }


            public int Add(ProcessItem value)
            {
                items.Add(value);
                return items.Count - 1;
            }

            public void Clear()
            {
                items.Clear();
            }

            public bool Contains(ProcessItem value)
            {
                return items.Contains(value);
            }

            public void CopyTo(IEnumerable<ProcessItem> array, int index)
            {
                items.CopyTo(array.ToArray(), index);
            }

            public IEnumerator GetEnumerator()
            {
                return items.Cast<ProcessItem>().GetEnumerator();
            }

            public int IndexOf(ProcessItem value)
            {
                return items.IndexOf(value);
            }

            public void Insert(int index, ProcessItem value)
            {
                items.Insert(index, value);
            }

            public void Remove(ProcessItem value)
            {
                items.Remove(value);
            }

            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
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


            int IList.Add(object value)
            {
                items.Add(value);
                return items.Count - 1;
            }

            bool IList.Contains(object value)
            {
                return items.Contains(value);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                items.CopyTo(array.Cast<ProcessItem>().ToArray(), index);
            }

            int IList.IndexOf(object value)
            {
                return items.IndexOf(value);
            }

            void IList.Insert(int index, object value)
            {
                items.Insert(index, value);
            }

            void IList.Remove(object value)
            {
                items.Remove(value);
            }


            [TypeConverter(typeof(ProcessItemConverter))]
            [DefaultProperty("Header")]
            public sealed class ProcessItem
            {
                private Label lblMark;

                private Label lblHeader;

                private Label lblDescription;

                private ControlExtension ceContent;


                internal ControlExtension Owner { get; set; }

                internal ObservableCollection<object> Items { get; set; }


                [Browsable(true)]
                [Category("Function")]
                [Description("Header displays on left steps panel.")]
                public string Header
                {
                    get { return lblHeader.Text; }
                    set
                    {
                        lblHeader.Text = value;
                        RefreshItems(Owner, Items);
                    }
                }

                [Browsable(true)]
                [Category("Function")]
                [Description("Description displays on top of content panel.")]
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
                public ControlCollection Controls
                {
                    get { return ceContent.Controls; }
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


                internal void AddToOwner()
                {
                    if (Owner == null || Items == null) { return; }

                    Owner.SuspendLayout();
                    // 
                    // lblDescription
                    // 
                    lblDescription.Enabled = false;
                    lblDescription.Visible = false;
                    Owner.Controls.Add(lblDescription);
                    // 
                    // ceContent
                    // 
                    int ceContentLocationY = Items.Max(i => (i as ProcessItem).lblDescription.Bounds.Bottom) + 10;
                    ceContent.Location = new Point(185, ceContentLocationY);
                    ceContent.Size = new Size(Owner.Width - 195, Owner.Height - ceContentLocationY);
                    ceContent.Enabled = false;
                    ceContent.Visible = false;
                    Owner.Controls.Add(ceContent);
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
                        ceContent.Enabled = true;
                        ceContent.Visible = true;
                    }
                    else
                    {
                        lblMark.Text = "□";
                        lblDescription.Enabled = false;
                        lblDescription.Visible = false;
                        ceContent.Enabled = false;
                        ceContent.Visible = false;
                    }
                }

                internal void SetTabStop(bool tabStop)
                {
                    foreach (Control control in Controls)
                    {
                        control.TabStop = tabStop;
                    }
                }

                internal void SetTabIndex(int tabIndex)
                {
                    foreach (Control control in Controls)
                    {
                        control.TabIndex = tabIndex;
                    }
                }


                private void InitializeComponent()
                {
                    lblMark = new Label();
                    lblHeader = new Label();
                    lblDescription = new Label();
                    ceContent = new ControlExtension();
                    // 
                    // lblMark
                    // 
                    lblMark.Text = "□";
                    lblMark.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
                    lblMark.Location = new Point(10, 10);
                    lblMark.Size = new Size(15, 15);
                    lblMark.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    // 
                    // lblHeader
                    // 
                    lblHeader.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
                    lblHeader.Location = new Point(10, 10);
                    lblHeader.MaximumSize = new Size(150, 0);
                    lblHeader.AutoSize = true;
                    lblHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    // 
                    // lblDescription
                    // 
                    lblDescription.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
                    lblDescription.Location = new Point(185, 10);
                    lblDescription.MaximumSize = new Size(300, 0);
                    lblDescription.AutoSize = true;
                    lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    // 
                    // ceContent
                    // 
                    ceContent.Location = new Point(180, 10);
                    ceContent.Size = new Size(50, 50);
                    ceContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
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
        }
    }
}
