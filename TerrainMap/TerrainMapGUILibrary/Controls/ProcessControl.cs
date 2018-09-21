using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TerrainMapGUILibrary.Extensions;

namespace TerrainMapGUILibrary.Controls
{
    [DefaultProperty("Items")]
    [DesignTimeVisible(true)]
    [ToolboxItem(true)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Controls")]
    public sealed class ProcessControl : ControlExtension
    {
        private ControlExtension cePanel;

        [Browsable(true)]
        [Category("Function")]
        [Description("The process items.")]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProcessItemCollection Items { get; private set; }


        public ProcessControl()
        {
            InitializeComponent();

            Items = new ProcessItemCollection() { Owner = cePanel };
        }


        private void InitializeComponent()
        {
            cePanel = new ControlExtension();
            SuspendLayout();
            // 
            // cePanel
            // 
            cePanel.BackColor = SystemColors.Control;
            cePanel.Dock = DockStyle.Fill;
            Controls.Add(cePanel);
            // 
            // this
            //
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
                items.CollectionChanged += (sender, e) => 
                {
                    Owner.Controls.Clear();
                    foreach (var item in items)
                    {
                        var processItem = item as ProcessItem;
                        processItem.Owner = Owner;
                        processItem.AddToOwner(IndexOf(processItem));
                    }

                    Owner.Invalidate();
                };

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
                return items.GetEnumerator();
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
                private Label lblHeader;

                private Label lblDescription;

                private ControlExtension ceContent;


                internal ControlExtension Owner { get; set; }


                [Browsable(true)]
                [Category("Function")]
                [Description("Header displays on left steps panel.")]
                public string Header
                {
                    get { return lblHeader.Text; }
                    set { lblHeader.Text = value; }
                }

                [Browsable(true)]
                [Category("Function")]
                [Description("Description displays on top of content panel.")]
                public string Description
                {
                    get { return lblDescription.Text; }
                    set { lblDescription.Text = value; }
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
                    Header = header;
                    Description = description;
                }


                public override string ToString()
                {
                    return $"ProcessItem: {{{Header}}}";
                }


                internal void AddToOwner(int index)
                {
                    Owner.SuspendLayout();
                    // 
                    // lblHeader
                    // 
                    Owner.Controls.Add(lblHeader);
                    // 
                    // Owner
                    //
                    Owner.ResumeLayout(false);
                    Owner.PerformLayout();
                }


                private void InitializeComponent()
                {
                    lblHeader = new Label();
                    lblDescription = new Label();
                    ceContent = new ControlExtension();
                    // 
                    // lblHeader
                    // 
                    lblHeader.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
                    lblHeader.Location = new Point(1, 1);
                    lblHeader.Size = new Size(49, 19);
                    lblHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    // 
                    // lblDescription
                    // 
                    lblDescription.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
                    lblDescription.Location = new Point(1, 1);
                    lblDescription.Size = new Size(49, 19);
                    lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    // 
                    // lblHeader
                    // 
                    ceContent.Location = new Point(1, 1);
                    ceContent.Size = new Size(50, 50);
                    ceContent.Anchor = AnchorStyles.Top | AnchorStyles.Left;
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
