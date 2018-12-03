using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TerrainMapGUILibrary.Components;
using TerrainMapGUILibrary.Extensions;
using TerrainMapGUILibrary.Themes;

namespace TerrainMapGUILibrary.Controls
{
    //[DefaultEvent("ValueChanged")]
    //[DefaultProperty("Value")]
    [DesignTimeVisible(true)]
    [ToolboxItem(true)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Controls")]
    public sealed class KrigingDataBuildControl : ControlExtension
    {
        private GroupBoxExtension grbFile;

        private Label lblFilePath;

        private InputPathComponent pcFilePath;

        private Label lblXColumn;

        private TextBoxExtension txbXColumn;

        private ToolTip tlpXColumn;

        private Label lblYColumn;

        private TextBoxExtension txbYColumn;

        private Label lblZColumn;

        private TextBoxExtension txbZColumn;

        private Label lblExclude;

        private TextBoxExtension txbExclude;

        private Label lblOutput;

        private InputPathComponent pcOutput;

        private Label lblStep;

        private ProgressBar pgbStep;

        private Button btnBuild;

        


        public KrigingDataBuildControl()
        {
            InitializeComponent();
        }


        private void InitializeComponent()
        {
            grbFile = new GroupBoxExtension();
            lblFilePath = new Label();
            pcFilePath = new InputPathComponent();
            lblXColumn = new Label();
            txbXColumn = new TextBoxExtension();
            tlpXColumn = new ToolTip();
            lblYColumn = new Label();
            txbYColumn = new TextBoxExtension();
            lblZColumn = new Label();
            txbZColumn = new TextBoxExtension();
            lblExclude = new Label();
            txbExclude = new TextBoxExtension();
            lblOutput = new Label();
            pcOutput = new InputPathComponent();
            lblStep = new Label();
            pgbStep = new ProgressBar();
            btnBuild = new Button();
            SuspendLayout();
            // 
            // grbFile
            // 
            grbFile.Text = "CSV Data Build Options";
            grbFile.Location = new Point(1, 1);
            grbFile.Size = new Size(423, 208);
            grbFile.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Controls.Add(grbFile);
            // 
            // lblFilePath
            // 
            lblFilePath.Text = "CSV File :";
            lblFilePath.Font = FontTheme.Normal();
            lblFilePath.Location = new Point(10, 34);
            lblFilePath.Size = new Size(69, 19);
            lblFilePath.BackColor = Color.AliceBlue;
            lblFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(lblFilePath);
            // 
            // pcFilePath
            // 
            pcFilePath.WatermarkText = "File Path";
            pcFilePath.PathSelectType = InputPathComponent.PathType.FilePath;
            pcFilePath.PathSelectFilter = "CSV File|*.csv";
            pcFilePath.Location = new Point(79, 30);
            pcFilePath.Size = new Size(333, 24);
            pcFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(pcFilePath);
            // 
            // lblXColumn
            // 
            lblXColumn.Text = "X :";
            lblXColumn.Font = FontTheme.Normal();
            lblXColumn.Location = new Point(55, 64);
            lblXColumn.Size = new Size(25, 19);
            lblXColumn.BackColor = Color.AliceBlue;
            lblXColumn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(lblXColumn);
            // 
            // txbXColumn
            // 
            txbXColumn.WatermarkText = "Column Index";
            txbXColumn.NumberInput = true;
            txbXColumn.Location = new Point(80, 60);
            txbXColumn.Size = new Size(90, 22);
            txbXColumn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(txbXColumn);
            // 
            // tlpXColumn
            // 
            tlpXColumn.ToolTipTitle = "The Value must be more than or equal with 0.";
            tlpXColumn.AutoPopDelay = 5000;
            tlpXColumn.InitialDelay = 1000;
            tlpXColumn.ReshowDelay = 500;
            tlpXColumn.ToolTipIcon = ToolTipIcon.Warning;
            tlpXColumn.IsBalloon = true;
            // 
            // lblYColumn
            // 
            lblYColumn.Text = "Y :";
            lblYColumn.Font = FontTheme.Normal();
            lblYColumn.Location = new Point(175, 64);
            lblYColumn.Size = new Size(25, 19);
            lblYColumn.BackColor = Color.AliceBlue;
            lblYColumn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(lblYColumn);
            // 
            // txbYColumn
            // 
            txbYColumn.WatermarkText = "Column Index";
            txbYColumn.NumberInput = true;
            txbYColumn.Location = new Point(200, 60);
            txbYColumn.Size = new Size(90, 22);
            txbYColumn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(txbYColumn);
            // 
            // lblZColumn
            // 
            lblZColumn.Text = "Z :";
            lblZColumn.Font = FontTheme.Normal();
            lblZColumn.Location = new Point(295, 64);
            lblZColumn.Size = new Size(25, 19);
            lblZColumn.BackColor = Color.AliceBlue;
            lblZColumn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(lblZColumn);
            // 
            // txbZColumn
            // 
            txbZColumn.WatermarkText = "Column Index";
            txbZColumn.NumberInput = true;
            txbZColumn.Location = new Point(320, 60);
            txbZColumn.Size = new Size(90, 22);
            txbZColumn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(txbZColumn);
            // 
            // lblExclude
            // 
            lblExclude.Text = "Exclude :";
            lblExclude.Font = FontTheme.Normal();
            lblExclude.Location = new Point(15, 94);
            lblExclude.Size = new Size(65, 19);
            lblExclude.BackColor = Color.AliceBlue;
            lblExclude.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(lblExclude);
            // 
            // txbExclude
            // 
            txbExclude.WatermarkText = "Rows Index ( split with , )";
            txbExclude.Location = new Point(80, 90);
            txbExclude.Size = new Size(330, 22);
            txbExclude.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(txbExclude);
            // 
            // lblRoot
            // 
            lblOutput.Text = "Build :";
            lblOutput.Font = FontTheme.Normal();
            lblOutput.Location = new Point(33, 124);
            lblOutput.Size = new Size(46, 19);
            lblOutput.BackColor = Color.AliceBlue;
            lblOutput.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(lblOutput);
            // 
            // pcOutput
            // 
            pcOutput.WatermarkText = "Output Directory";
            pcOutput.PathSelectType = InputPathComponent.PathType.Directory;
            pcOutput.Location = new Point(79, 120);
            pcOutput.Size = new Size(333, 24);
            pcOutput.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(pcOutput);
            // 
            // lblStep
            // 
            lblStep.Text = "Build ... (1000/1000)";
            lblStep.Font = FontTheme.Normal();
            lblStep.Location = new Point(80, 150);
            lblStep.AutoSize = true;
            lblStep.BackColor = Color.AliceBlue;
            lblStep.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(lblStep);
            // 
            // pgbStep
            // 
            pgbStep.Style = ProgressBarStyle.Continuous;
            pgbStep.Location = new Point(80, 171);
            pgbStep.Size = new Size(275, 22);
            pgbStep.Maximum = 100;
            pgbStep.Value = 50;
            pgbStep.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            grbFile.Controls.Add(pgbStep);
            // 
            // btnBuild
            // 
            btnBuild.Text = "Build";
            btnBuild.Font = FontTheme.Normal();
            btnBuild.Location = new Point(360, 170);
            btnBuild.Size = new Size(50, 24);
            btnBuild.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBuild.TabStop = false;
            btnBuild.Click += (sender, e) => { Build(); };
            grbFile.Controls.Add(btnBuild);
            // 
            // this
            // 
            Size = new Size(425, 210);
            ResumeLayout(false);
            PerformLayout();
        }

        private void Build()
        {
            tlpXColumn.Hide(txbXColumn);
            tlpXColumn.SetToolTip(txbXColumn, "xixi");
            tlpXColumn.Show("xixi", txbXColumn, 10000);
        }
    }
}
