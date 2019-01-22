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
using TerrainMapGUILibrary.Resources;
using TerrainMapGUILibrary.Themes;

namespace TerrainMapGUILibrary.Components
{
    [DefaultEvent("IsFoldedChanged")]
    [DefaultProperty("IsFolded")]
    [DesignTimeVisible(true)]
    [ToolboxItem(true)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Components")]
    public sealed class FoldPanelComponent : ControlExtension
    {
        private Image upwardArrow;

        private Image downwardArrow;

        private string title;

        private Size fullSize;

        private bool isFolded;

        private LabelExtension lblTitle;

        private PictureBoxExtension pcbTitleArrow;

        private ControlExtension conTitle;

        private ControlExtension conContent;

        internal ObservableCollection<PanelContent> InnerItems { get; private set; }

        [Category("Function")]
        [Description("Title for header bar.")]
        [DefaultValue("Title")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                lblTitle.Text = FontTheme.Ellipsis(conTitle, title, lblTitle.Font, conTitle.Width - 20);
            }
        }

        [Category("Function")]
        [Description("Full Size if panel is not folded.")]
        [DefaultValue(typeof(Size), "100, 100")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size FullSize
        {
            get
            {
                return fullSize;
            }
            set
            {
                fullSize = value;
                UpdateSize();
            }
        }

        [Category("Function")]
        [Description("Indicate if folded or not.")]
        [DefaultValue(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsFolded
        {
            get
            {
                return isFolded;
            }
            set
            {
                bool isValueChanged = isFolded != value;
                isFolded = value;
                if (isValueChanged == true)
                {
                    // switch arrow indicate and size
                    pcbTitleArrow.Image = value ? downwardArrow : upwardArrow;
                    UpdateSize();
                }

                if (isValueChanged == true && IsFoldedChanged != null)
                {
                    IsFoldedChanged.Invoke(this, new EventArgs());
                }
            }
        }

        [Category("Function")]
        [Description("Occurs when is folded value changed.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler IsFoldedChanged;

        [Category("Function")]
        [Description("The process items.")]
        [MergableProperty(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PanelContentCollection Items { get; private set; }

        public FoldPanelComponent()
        {
            upwardArrow = ResourceHelper.GetArrowUpward20();
            downwardArrow = ResourceHelper.GetArrowDownward20();
            title = "Title";
            fullSize = new Size(100, 100);
            isFolded = false;
            InnerItems = new ObservableCollection<PanelContent>();
            InnerItems.CollectionChanged += (sender, e) =>
            {
                conContent.SuspendLayout();
                conContent.Controls.Clear();
                foreach (var item in InnerItems)
                {
                    conContent.Controls.Add(item);
                }

                conContent.ResumeLayout(false);
                conContent.PerformLayout();
            };
            Items = new PanelContentCollection()
            {
                Owner = this
            };

            InitializeComponent();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // need to reset the fullSize here
            fullSize.Width = Size.Width;
            if (isFolded == false)
            {
                fullSize.Height = Size.Height;
            }

            UpdateSize();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // draw border
            var pen = new Pen(SystemColors.ControlText, 1f);
            e.Graphics.DrawRectangle(
                pen,
                ClientRectangle.Left, ClientRectangle.Top,
                ClientRectangle.Width - 0.5f, ClientRectangle.Height - 0.5f
            );
            pen.Dispose();
        }

        private void InitializeComponent()
        {
            lblTitle = new LabelExtension();
            pcbTitleArrow = new PictureBoxExtension();
            conTitle = new ControlExtension();
            conContent = new ControlExtension();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Text = title;
            lblTitle.Location = new Point(0, 0);
            lblTitle.AutoSize = true;
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lblTitle.Click += (sender, e) =>
            {
                IsFolded = !IsFolded;
            };
            conTitle.Controls.Add(lblTitle);
            // 
            // pcbArrow
            // 
            pcbTitleArrow.Location = new Point(78, 0);
            pcbTitleArrow.Size = new Size(20, 20);
            pcbTitleArrow.Image = upwardArrow;
            pcbTitleArrow.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pcbTitleArrow.Click += (sender, e) =>
            {
                IsFolded = !IsFolded;
            };
            conTitle.Controls.Add(pcbTitleArrow);
            // 
            // conTitle
            // 
            conTitle.Cursor = Cursors.Hand;
            conTitle.Location = new Point(1, 1);
            conTitle.Size = new Size(98, 20);
            conTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            conTitle.Click += (sender, e) =>
            {
                IsFolded = !IsFolded;
            };
            Controls.Add(conTitle);
            // 
            // conContent
            // 
            conContent.Location = new Point(1, 21);
            conContent.Size = new Size(98, 77);
            conContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Controls.Add(conContent);
            // 
            // this
            // 
            Size = new Size(100, 100);
            IsFoldedChanged = null;
            ResumeLayout(false);
            PerformLayout();
        }

        private void UpdateSize()
        {
            if (lblTitle != null)
            {
                lblTitle.Text = FontTheme.Ellipsis(conTitle, title, lblTitle.Font, conTitle.Width - 20);
            }

            var newSize = new Size(Size.Width, Size.Height);
            if (isFolded == true)
            {
                // limit the width and fix the height if is folded
                if (Size.Width < 22)
                {
                    newSize.Width = 22;
                }

                newSize.Height = 22;
            }
            else
            {
                // limit the width and height if is not folded
                if (fullSize.Width < 22)
                {
                    fullSize.Width = 22;
                }

                if (fullSize.Height < 22)
                {
                    fullSize.Height = 22;
                }

                // the fullSize always be same with current size if is not folded
                newSize = fullSize;
            }

            if (newSize.Width != Size.Width || newSize.Height != Size.Height)
            {
                // update current Size if needed
                Size = newSize;
            }
        }

        [DefaultProperty("Name")]
        [DesignTimeVisible(false)]
        [ToolboxItem(false)]
        [TypeConverter(typeof(PanelContentConverter))]
        public sealed class PanelContent : PanelExtension
        {
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override bool AutoSize
            {
                get
                {
                    return base.AutoSize;
                }
                set
                {
                    base.AutoSize = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override AnchorStyles Anchor
            {
                get
                {
                    return base.Anchor;
                }
                set
                {
                    base.Anchor = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override DockStyle Dock
            {
                get
                {
                    return base.Dock;
                }
                set
                {
                    base.Dock = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Padding Margin
            {
                get
                {
                    return base.Margin;
                }
                set
                {
                    base.Margin = value;
                }
            }

            [DefaultValue(typeof(Size), "0, 0")]
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override Size MaximumSize
            {
                get
                {
                    return base.MaximumSize;
                }
                set
                {
                    base.MaximumSize = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override Size MinimumSize
            {
                get
                {
                    return base.MinimumSize;
                }
                set
                {
                    base.MinimumSize = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point Location
            {
                get
                {
                    return base.Location;
                }
                set
                {
                    base.Location = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Padding Padding
            {
                get
                {
                    return base.Padding;
                }
                set
                {
                    base.Padding = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size PreferredSize
            {
                get
                {
                    return base.PreferredSize;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size Size
            {
                get
                {
                    return base.Size;
                }
                set
                {
                    base.Size = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler AutoSizeChanged
            {
                add
                {
                    base.AutoSizeChanged += value;
                }
                remove
                {
                    base.AutoSizeChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler DockChanged
            {
                add
                {
                    base.DockChanged += value;
                }
                remove
                {
                    base.DockChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler LocationChanged
            {
                add
                {
                    base.LocationChanged += value;
                }
                remove
                {
                    base.LocationChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler MarginChanged
            {
                add
                {
                    base.MarginChanged += value;
                }
                remove
                {
                    base.MarginChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler PaddingChanged
            {
                add
                {
                    base.PaddingChanged += value;
                }
                remove
                {
                    base.PaddingChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler Resize
            {
                add
                {
                    base.Resize += value;
                }
                remove
                {
                    base.Resize -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler SizeChanged
            {
                add
                {
                    base.SizeChanged += value;
                }
                remove
                {
                    base.SizeChanged -= value;
                }
            }

            public PanelContent()
            {
                Dock = DockStyle.Fill;
            }

            public override string ToString()
            {
                return GetType().Name;
            }
        }

        public sealed class PanelContentConverter : ExpandableObjectConverter
        {
            public PanelContentConverter()
            { }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(InstanceDescriptor))
                {
                    return true;
                }

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (value != null && destinationType != null)
                {
                    if (value is PanelContent && destinationType == typeof(InstanceDescriptor))
                    {
                        // convert from instance PanelContent
                        var source = value as PanelContent;
                        var ci = typeof(PanelContent).GetConstructor(new Type[] { });
                        if (ci != null)
                        {
                            var instance = new InstanceDescriptor(ci, new object[] { });
                            return instance;
                        }
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        [ListBindable(false)]
        public sealed class PanelContentCollection : IList, ICollection, IEnumerable
        {
            internal FoldPanelComponent Owner { get; set; }

            public PanelContent this[int index]
            {
                get
                {
                    return Owner.InnerItems[index];
                }
                set
                {
                    Owner.InnerItems[index] = value;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public int Count
            {
                get
                {
                    return Owner.InnerItems.Count;
                }
            }

            public PanelContentCollection()
            {
                Owner = null;
            }

            public int Add(PanelContent value)
            {
                Owner.InnerItems.Add(value);
                return Owner.InnerItems.Count - 1;
            }

            public void Clear()
            {
                Owner.InnerItems.Clear();
            }

            public bool Contains(PanelContent value)
            {
                return Owner.InnerItems.Contains(value);
            }

            public void CopyTo(IEnumerable<PanelContent> array, int index)
            {
                Owner.InnerItems.CopyTo(array.ToArray(), index);
            }

            public IEnumerator GetEnumerator()
            {
                return Owner.InnerItems.Cast<PanelContent>().GetEnumerator();
            }

            public int IndexOf(PanelContent value)
            {
                return Owner.InnerItems.IndexOf(value);
            }

            public void Insert(int index, PanelContent value)
            {
                Owner.InnerItems.Insert(index, value);
            }

            public void Remove(PanelContent value)
            {
                Owner.InnerItems.Remove(value);
            }

            public void RemoveAt(int index)
            {
                Owner.InnerItems.RemoveAt(index);
            }

            object IList.this[int index]
            {
                get
                {
                    return Owner.InnerItems[index];
                }
                set
                {
                    Owner.InnerItems[index] = value as PanelContent;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return true;
                }
            }

            int IList.Add(object value)
            {
                Owner.InnerItems.Add(value as PanelContent);
                return Owner.InnerItems.Count - 1;
            }

            bool IList.Contains(object value)
            {
                return Owner.InnerItems.Contains(value);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                Owner.InnerItems.CopyTo(array.Cast<PanelContent>().ToArray(), index);
            }

            int IList.IndexOf(object value)
            {
                return Owner.InnerItems.IndexOf(value as PanelContent);
            }

            void IList.Insert(int index, object value)
            {
                Owner.InnerItems.Insert(index, value as PanelContent);
            }

            void IList.Remove(object value)
            {
                Owner.InnerItems.Remove(value as PanelContent);
            }
        }
    }
}
