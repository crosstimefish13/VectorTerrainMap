using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Collections;
using TerrainMapGUILibrary.Extensions;
using TerrainMapGUILibrary.Resources.Fonts;
using TerrainMapGUILibrary.Resources.Images;
using TerrainMapGUILibrary.Themes;

namespace TerrainMapGUILibrary.Components
{
    [DefaultEvent("IsFoldedChanged")]
    [DefaultProperty("Contents")]
    [DesignTimeVisible(true)]
    [ToolboxItem(true)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Components")]
    public sealed class FoldPanel : ControlExtension, PanelCollection.IOwner
    {
        private readonly int borderWidth;

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
                    pcbTitleArrow.Image = value ? pcbTitleArrow.PendingImages[1] : pcbTitleArrow.PendingImages[0];
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

        public FoldPanel()
        {
            borderWidth = 1;
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
            float bofderWidthF = Convert.ToSingle(borderWidth);
            var pen = new Pen(ForeColor, bofderWidthF);
            if (borderWidth == 1)
            {
                e.Graphics.DrawRectangle(
                    pen,
                    ClientRectangle.Left,
                    ClientRectangle.Top,
                    ClientRectangle.Width - bofderWidthF / 2f,
                    ClientRectangle.Height - bofderWidthF / 2f
                );
            }
            else
            {
                e.Graphics.DrawRectangle(
                    pen,
                    ClientRectangle.Left + bofderWidthF / 2f,
                    ClientRectangle.Top + bofderWidthF / 2f,
                    ClientRectangle.Width - bofderWidthF,
                    ClientRectangle.Height - bofderWidthF
                );
            }
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
            pcbTitleArrow.Location = new Point(fullSize.Width - borderWidth * 2 - lblTitle.Height, 0);
            pcbTitleArrow.Size = new Size(lblTitle.Height, lblTitle.Height);
            pcbTitleArrow.PendingImages.Add(ImageHelper.GetArrowUpward(lblTitle.Height));
            pcbTitleArrow.PendingImages.Add(ImageHelper.GetArrowDownward(lblTitle.Height));
            pcbTitleArrow.Image = pcbTitleArrow.PendingImages[0];
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
            conTitle.Location = new Point(borderWidth, borderWidth);
            conTitle.Size = new Size(fullSize.Width - borderWidth * 2, lblTitle.Height);
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
            conContent.Location = new Point(borderWidth, borderWidth + lblTitle.Height);
            conContent.Size = new Size(
                fullSize.Width - borderWidth * 2,
                fullSize.Height - borderWidth * 2 - lblTitle.Height
            );
            conContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Controls.Add(conContent);
            // 
            // this
            // 
            Size = new Size(fullSize.Width, fullSize.Height);
            IsFoldedChanged = null;
            ResumeLayout(false);
            PerformLayout();
        }

        private void UpdateSize()
        {
            if (lblTitle == null)
            {
                return;
            }

            lblTitle.Text = FontHelper.Ellipsis(lblTitle, lblTitle.Font, conTitle.Width - pcbTitleArrow.Width, title);

            var newSize = new Size(Size.Width, Size.Height);
            if (isFolded == true)
            {
                // limit the width and fix the height if is folded
                if (Size.Width < pcbTitleArrow.Width + borderWidth * 2)
                {
                    newSize.Width = pcbTitleArrow.Width + borderWidth * 2;
                }

                newSize.Height = pcbTitleArrow.Height + borderWidth * 2;
            }
            else
            {
                // limit the width and height if is not folded
                if (fullSize.Width < pcbTitleArrow.Width + borderWidth * 2)
                {
                    fullSize.Width = pcbTitleArrow.Width + borderWidth * 2;
                }

                if (fullSize.Height < pcbTitleArrow.Height + borderWidth * 2)
                {
                    fullSize.Height = pcbTitleArrow.Height + borderWidth * 2;
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
