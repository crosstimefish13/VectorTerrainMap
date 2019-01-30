using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TerrainMapGUILibrary.Extensions;

namespace TerrainMapGUILibrary.Components
{
    [DefaultEvent("PathChanged")]
    [DefaultProperty("PrefixText")]
    [DesignTimeVisible(true)]
    [ToolboxItem(true)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Components")]
    public sealed class InputPathComponent : ControlExtension
    {
        private LabelExtension lblPrefix;

        private TextBoxExtension txbPath;

        private ButtonExtension btnPath;

        [Category("Function")]
        [Description("Prefix text for value input.")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string PrefixText
        {
            get
            {
                return lblPrefix.Text;
            }
            set
            {
                lblPrefix.Text = value;
                RefreshControls();
            }
        }

        [Category("Function")]
        [Description("Watermark text for value input.")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string WatermarkText
        {
            get
            {
                return txbPath.WatermarkText;
            }
            set
            {
                txbPath.WatermarkText = value;
            }
        }

        [Category("Function")]
        [Description("Path input.")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Path
        {
            get
            {
                return txbPath.Text;
            }
            set
            {
                txbPath.Text = value;
            }
        }

        [Category("Function")]
        [Description("Path type when click select button.")]
        [DefaultValue(PathType.FilePath)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PathType PathSelectType { get; set; }

        [Category("Function")]
        [Description("Path filter when click select button.")]
        [DefaultValue("All File|*.*")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string PathSelectFilter { get; set; }

        [Category("Function")]
        [Description("Occurs when path changed.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler PathChanged
        {
            add
            {
                txbPath.TextChanged += value;
            }
            remove
            {
                txbPath.TextChanged -= value;
            }
        }

        [Category("Function")]
        [Description("Determines the index in the TAB order that the input control(s) will occupy. There is 1 control index would be set.")]
        [DefaultValue(0)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int StartTabIndex
        {
            get
            {
                return txbPath.TabIndex;
            }
            set
            {
                txbPath.TabIndex = value;
            }
        }

        public InputPathComponent()
        {
            PathSelectType = PathType.FilePath;
            PathSelectFilter = "All File|*.*";

            InitializeComponent();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            var newSize = new Size(Size.Width, Size.Height);

            // limit the width and height
            if (Size.Width < 28)
            {
                newSize.Width = 28;
            }

            if (Size.Height != 24)
            {
                newSize.Height = 24;
            }

            // 
            // btnPath
            // 
            if (btnPath != null)
            {
                btnPath.Location = new Point(ClientRectangle.Right - btnPath.Width, 0);
            }

            // 
            // txbPath
            // 
            if (txbPath != null)
            {
                txbPath.Size = new Size(btnPath.Bounds.Left - 1 - txbPath.Bounds.Left, 22);
            }

            if (newSize.Width != Size.Width || newSize.Height != Size.Height)
            {
                // update current Size if needed
                Size = newSize;
            }
        }

        private void InitializeComponent()
        {
            lblPrefix = new LabelExtension();
            txbPath = new TextBoxExtension();
            btnPath = new ButtonExtension();
            SuspendLayout();
            // 
            // txbPrefix
            // 
            lblPrefix.AutoSize = true;
            lblPrefix.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            // 
            // txbPath
            // 
            txbPath.Location = new Point(0, 1);
            txbPath.Size = new Size(71, 22);
            txbPath.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Controls.Add(txbPath);
            // 
            // btnPath
            // 
            btnPath.Text = "...";
            btnPath.Location = new Point(72, 0);
            btnPath.Size = new Size(28, 24);
            btnPath.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnPath.Click += (sender, e) =>
            {
                SelectPath();
            };
            Controls.Add(btnPath);
            // 
            // this
            // 
            Size = new Size(100, 24);
            StartTabIndex = 0;
            ResumeLayout(false);
            PerformLayout();
        }

        private void SelectPath()
        {
            string defaultDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (PathSelectType == PathType.FilePath)
            {
                var dialog = new OpenFileDialog()
                {
                    Title = "Select a File",
                    AddExtension = false,
                    InitialDirectory = defaultDir,
                    Filter = PathSelectFilter
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txbPath.Text = dialog.FileName;
                }
            }
            else if (PathSelectType == PathType.Directory)
            {
                var dialog = new FolderBrowserDialog()
                {
                    Description = "Select a Folder",
                    RootFolder = Environment.SpecialFolder.DesktopDirectory,
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txbPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void RefreshControls()
        {
            var fixedLocation = new Point(Location.X + txbPath.Location.X, Location.Y);

            if (string.IsNullOrEmpty(lblPrefix.Text))
            {
                // 
                // txbPrefix
                // 
                Controls.Remove(lblPrefix);
                // 
                // txbPath
                // 
                txbPath.Location = new Point(0, 1);
                // 
                // btnPath
                // 
                btnPath.Location = new Point(txbPath.Bounds.Right + 1, 0);
                // 
                // this
                // 
                Location = new Point(fixedLocation.X, fixedLocation.Y);
                Size = new Size(btnPath.Bounds.Right, 24);
            }
            else
            {
                // 
                // txbPrefix
                // 
                if (Controls.Contains(lblPrefix) == false)
                {
                    lblPrefix.Location = new Point(0, 3);
                    Controls.Add(lblPrefix);
                }

                // 
                // txbPath
                // 
                txbPath.Location = new Point(lblPrefix.Bounds.Right, 1);
                // 
                // btnPath
                // 
                btnPath.Location = new Point(txbPath.Bounds.Right + 1, 0);
                // 
                // this
                // 
                Location = new Point(fixedLocation.X - lblPrefix.Width, fixedLocation.Y);
                Size = new Size(btnPath.Bounds.Right, 24);
            }
        }

        public enum PathType
        {
            FilePath = 0,
            Directory = 1
        }
    }
}
