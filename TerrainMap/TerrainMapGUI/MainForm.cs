using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerrainMapGUI
{
    public class MainForm : Form
    {
        private Label label1;
        private Controls.TrackValueControl trackValueControl1;

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
            this.label1 = new System.Windows.Forms.Label();
            this.trackValueControl1 = new TerrainMapGUI.Controls.TrackValueControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(318, 464);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // trackValueControl1
            // 
            this.trackValueControl1.Location = new System.Drawing.Point(318, 366);
            this.trackValueControl1.Name = "trackValueControl1";
            this.trackValueControl1.Size = new System.Drawing.Size(353, 24);
            this.trackValueControl1.TrackValueChanged += new System.EventHandler(this.trackValueControl1_TrackValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackValueControl1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Terrain Map";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void trackValueControl1_TrackValueChanged(object sender, EventArgs e)
        {
            label1.Text = trackValueControl1.TrackValue.ToString();
        }
    }
}
