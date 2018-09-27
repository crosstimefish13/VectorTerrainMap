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
        private KrigingSemivarianceMapControl krigingSemivarianceMapControl1;
        private KrigingSemivarianceMapControl ksmcTool;

        public MainForm()
        {
            InitializeComponent();

            ksmcTool = new KrigingSemivarianceMapControl();
            ksmcTool.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            ksmcTool.Dock = DockStyle.Fill;
            //processControl1.Items[1].Controls.Add(ksmcTool);
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
            this.krigingSemivarianceMapControl1 = new TerrainMapGUILibrary.Controls.KrigingSemivarianceMapControl();
            this.SuspendLayout();
            // 
            // krigingSemivarianceMapControl1
            // 
            this.krigingSemivarianceMapControl1.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.krigingSemivarianceMapControl1.Location = new System.Drawing.Point(322, 189);
            this.krigingSemivarianceMapControl1.Name = "krigingSemivarianceMapControl1";
            this.krigingSemivarianceMapControl1.Size = new System.Drawing.Size(650, 200);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.krigingSemivarianceMapControl1);
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
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var map = SemivarianceMap.Load(SemivarianceMap.GetALlLagBins()[2]);
            ksmcTool.LoadData(map);
        }
    }
}
