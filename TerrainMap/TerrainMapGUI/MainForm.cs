using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TerrainMapGUILibrary.Controls;
using TerrainMapLibrary.Interpolator.Kriging;

namespace TerrainMapGUI
{
    public class MainForm : Form
    {
        private ProcessControl processControl1;
        private ProcessControl.ProcessItem processItem1;
        private ProcessControl.ProcessItem processItem2;
        private KrigingSemivarianceMapControl krigingSemivarianceMapControl1;

        public MainForm()
        {
            InitializeComponent();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // unmanaged resource here
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.processControl1 = new TerrainMapGUILibrary.Controls.ProcessControl();
            this.processItem1 = new TerrainMapGUILibrary.Controls.ProcessControl.ProcessItem("Step 1", "Do Something");
            this.processItem2 = new TerrainMapGUILibrary.Controls.ProcessControl.ProcessItem("Step 2", "Kriging Map");
            this.krigingSemivarianceMapControl1 = new TerrainMapGUILibrary.Controls.KrigingSemivarianceMapControl();
            this.processItem2.SuspendLayout();
            this.SuspendLayout();
            // 
            // processControl1
            // 
            this.processControl1.BackColor = System.Drawing.SystemColors.Control;
            this.processControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processControl1.Items.Add(this.processItem1);
            this.processControl1.Items.Add(this.processItem2);
            this.processControl1.Location = new System.Drawing.Point(0, 0);
            this.processControl1.Name = "processControl1";
            this.processControl1.SelectedIndex = 0;
            this.processControl1.Size = new System.Drawing.Size(1264, 681);
            // 
            // processItem1
            // 
            this.processItem1.Description = "Do Something";
            this.processItem1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processItem1.Header = "Step 1";
            this.processItem1.Location = new System.Drawing.Point(0, 0);
            this.processItem1.Name = "processItem1";
            this.processItem1.Size = new System.Drawing.Size(1069, 602);
            // 
            // processItem2
            // 
            this.processItem2.Controls.Add(this.krigingSemivarianceMapControl1);
            this.processItem2.Description = "Kriging Map";
            this.processItem2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processItem2.Header = "Step 2";
            this.processItem2.Location = new System.Drawing.Point(0, 0);
            this.processItem2.Name = "processItem2";
            this.processItem2.Size = new System.Drawing.Size(1069, 602);
            // 
            // krigingSemivarianceMapControl1
            // 
            this.krigingSemivarianceMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.krigingSemivarianceMapControl1.Location = new System.Drawing.Point(0, 0);
            this.krigingSemivarianceMapControl1.Name = "krigingSemivarianceMapControl1";
            this.krigingSemivarianceMapControl1.Size = new System.Drawing.Size(1069, 602);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.processControl1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Terrain Map";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.processItem2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var map = SemivarianceMap.Load(SemivarianceMap.GetALlLagBins()[2]);
            krigingSemivarianceMapControl1.LoadData(map);
        }
    }
}
