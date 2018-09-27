using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Extensions;
using TerrainMapGUILibrary.Themes;

namespace TerrainMapGUILibrary.Components
{
    [DefaultEvent("IsFoldedChanged")]
    [DefaultProperty("Title")]
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Components")]
    public sealed class FoldPanelComponent : ControlExtension
    {
        private Size minSize;

        private Size maxSize;

        private bool isFolded;

        private Label lblTitle;

        private Label lblArrow;

        private ControlExtension conTitle;


        [Category("Function")]
        [Description("Title for header bar.")]
        [DefaultValue("Title")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        [Category("Function")]
        [Description("Size if folded.")]
        [DefaultValue(typeof(Size), "100, 22")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size MinSize
        {
            get { return minSize; }
            set
            {
                // limit min size
                minSize = value;
                if (minSize.Width < 1) { minSize.Width = 1; }
                if (minSize.Height < 22) { minSize.Height = 22; }

                Invalidate();
            }
        }

        [Category("Function")]
        [Description("Size if not folded.")]
        [DefaultValue(typeof(Size), "100, 102")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size MaxSize
        {
            get { return maxSize; }
            set
            {
                // limit max size
                maxSize = value;
                if (maxSize.Width < 1) { maxSize.Width = 1; }
                if (maxSize.Height < 22) { maxSize.Height = 22; }

                Invalidate();
            }
        }

        [Category("Function")]
        [Description("Indicate if folded or not.")]
        [DefaultValue(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsFolded
        {
            get { return isFolded; }
            set
            {
                bool isFoldedChanged = false;
                if (isFolded != value)
                {
                    // show arrow indicate text
                    if (value == true)
                    {
                        Size = minSize;
                        lblArrow.Text = "▼";
                    }
                    else
                    {
                        Size = maxSize;
                        lblArrow.Text = "▲";
                    }

                    Invalidate();
                    isFoldedChanged = true;
                }

                isFolded = value;

                if (isFoldedChanged == true && IsFoldedChanged != null)
                { IsFoldedChanged.Invoke(this, new EventArgs()); }
            }
        }

        [Category("Function")]
        [Description("Occurs when is folded value changed.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler IsFoldedChanged;


        public FoldPanelComponent()
           : base()
        {
            minSize = new Size(100, 22);
            maxSize = new Size(100, 102);
            isFolded = false;

            InitializeComponent();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // draw border
            var pen = new Pen(SystemColors.ControlText, 1f);
            e.Graphics.DrawRectangle(pen,
                ClientRectangle.Left, ClientRectangle.Top,
                ClientRectangle.Width - 0.5f, ClientRectangle.Height - 0.5f);
            pen.Dispose();
        }


        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblArrow = new Label();
            conTitle = new ControlExtension();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Text = "Title";
            lblTitle.Font = FontTheme.Normal();
            lblTitle.Location = new Point(1, 1);
            lblTitle.AutoSize = true;
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lblTitle.Click += (sender, e) => { IsFolded = !IsFolded; };
            conTitle.Controls.Add(lblTitle);
            // 
            // lblArrow
            // 
            lblArrow.Text = "▲";
            lblArrow.Font = FontTheme.Normal();
            lblArrow.Location = new Point(80, 1);
            lblArrow.Size = new Size(20, 19);
            lblArrow.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblArrow.Click += (sender, e) => { IsFolded = !IsFolded; };
            conTitle.Controls.Add(lblArrow);
            // 
            // conTitle
            // 
            conTitle.Cursor = Cursors.Hand;
            conTitle.Location = new Point(1, 1);
            conTitle.Size = new Size(98, 20);
            conTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            conTitle.Click += (sender, e) => { IsFolded = !IsFolded; };
            Controls.Add(conTitle);
            // 
            // this
            // 
            Size = new Size(100, 102);
            IsFoldedChanged = null;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
