using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Collections;
using TerrainMapGUILibrary.Extensions;
using TerrainMapGUILibrary.Resources;
using TerrainMapGUILibrary.Resources.Images;
using TerrainMapGUILibrary.Themes;

namespace TerrainMapGUILibrary.Components
{
    [DefaultEvent("IsFoldedChanged")]
    [DefaultProperty("Contents")]
    [DesignTimeVisible(true)]
    [ToolboxItem(true)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Components")]
    public sealed class FoldPanelComponent : ControlExtension, PanelCollection.IOwner
    {
        private readonly Image upwardArrow;

        private readonly Image downwardArrow;

        private string title;

        private Size fullSize;

        private bool isFolded;

        private LabelExtension lblTitle;

        private PictureBoxExtension pcbTitleArrow;

        private ControlExtension conTitle;

        private ObservableCollection<PanelCollection.Panel> pcpInnerContents;

        private ControlExtension conContent;

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
                    // switch arrow indicate, size and contents display
                    pcbTitleArrow.Image = value ? downwardArrow : upwardArrow;
                    UpdateSize();
                    conContent.Enabled = !isFolded;
                    conContent.Visible = !isFolded;
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
        [Description("The content list.")]
        [MergableProperty(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PanelCollection Contents { get; private set; }

        public FoldPanelComponent()
        {
            upwardArrow = ImageHelper.GetArrowUpward(20);
            downwardArrow = ImageHelper.GetArrowDownward(20);
            title = "Title";
            fullSize = new Size(100, 100);
            isFolded = false;
            Contents = new PanelCollection(this);
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
            pcpInnerContents = new ObservableCollection<PanelCollection.Panel>();
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
            // pcpInnerContents
            // 
            pcpInnerContents.CollectionChanged += (sender, e) =>
            {
                RefreshContents();
            };
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

        private void RefreshContents()
        {
            // clear all old content panels, then add new panels to content
            conContent.SuspendLayout();
            conContent.Controls.Clear();
            foreach (var innerContents in pcpInnerContents)
            {
                conContent.Controls.Add(innerContents);
            }

            conContent.ResumeLayout(false);
            conContent.PerformLayout();
        }

        ObservableCollection<PanelCollection.Panel> PanelCollection.IOwner.GetItems()
        {
            return pcpInnerContents;
        }
    }
}
