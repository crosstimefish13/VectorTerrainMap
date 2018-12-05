using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TerrainMapGUILibrary.Extensions;

namespace TerrainMapGUILibrary.Components
{
    [DefaultEvent("PathChanged")]
    [DefaultProperty("Path")]
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Components")]
    public sealed class InputPathComponent : ControlExtension
    {
        private TextBoxExtension txbPath;

        private ButtonExtension btnPath;

        [Category("Function")]
        [Description("Watermark text for value input.")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string WatermarkText
        {
            get { return txbPath.WatermarkText; }
            set { txbPath.WatermarkText = value; }
        }

        [Category("Function")]
        [Description("Path input.")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Path
        {
            get { return txbPath.Text; }
            set { txbPath.Text = value; }
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
            add { txbPath.TextChanged += value; }
            remove { txbPath.TextChanged -= value; }
        }

        [Category("Function")]
        [Description("Determines the index in the TAB order that the input control(s) will occupy. There is 1 control index would be set.")]
        [DefaultValue(0)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int StartTabIndex
        {
            get { return txbPath.TabIndex; }
            set { txbPath.TabIndex = value; }
        }

        public InputPathComponent()
           : base()
        {
            PathSelectType = PathType.FilePath;
            PathSelectFilter = "All File|*.*";

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            txbPath = new TextBoxExtension();
            btnPath = new ButtonExtension();
            SuspendLayout();
            // 
            // txbPath
            // 
            txbPath.Location = new Point(1, 1);
            txbPath.Size = new Size(100, 22);
            txbPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(txbPath);
            // 
            // btnPath
            // 
            btnPath.Text = "...";
            btnPath.Location = new Point(102, 0);
            btnPath.Size = new Size(24, 24);
            btnPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPath.Click += (sender, e) => { SelectPath(); };
            Controls.Add(btnPath);
            // 
            // this
            // 
            Size = new Size(127, 24);
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
                { txbPath.Text = dialog.FileName; }
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
                { txbPath.Text = dialog.SelectedPath; }
            }
        }

        public enum PathType
        {
            FilePath = 0,
            Directory = 1
        }
    }
}
