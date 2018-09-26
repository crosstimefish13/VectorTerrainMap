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
        private KrigingSemivarianceMapControl ksmcTool;

        private TerrainMapGUILibrary.Controls.ProcessControl processControl1;

        public MainForm()
        {
            InitializeComponent();

            ksmcTool = new KrigingSemivarianceMapControl();
            ksmcTool.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            ksmcTool.Dock = DockStyle.Fill;
            processControl1.Items[1].Controls.Add(ksmcTool);
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
            this.SuspendLayout();
            // 
            // processControl1
            // 
            this.processControl1.BackColor = System.Drawing.SystemColors.Control;
            this.processControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processControl1.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.processControl1.Items.Add(new TerrainMapGUILibrary.Controls.ProcessControl.ProcessItemCollection.ProcessItem("aaa", "111"));
            this.processControl1.Items.Add(new TerrainMapGUILibrary.Controls.ProcessControl.ProcessItemCollection.ProcessItem("bbb", "222"));
            this.processControl1.Location = new System.Drawing.Point(0, 0);
            this.processControl1.Name = "processControl1";
            this.processControl1.SelectedIndex = 0;
            this.processControl1.Size = new System.Drawing.Size(1264, 681);
            this.processControl1.TabIndex = 1;
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
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var map = SemivarianceMap.Load(SemivarianceMap.GetALlLagBins()[2]);
            ksmcTool.LoadData(map);
        }
    }
}
