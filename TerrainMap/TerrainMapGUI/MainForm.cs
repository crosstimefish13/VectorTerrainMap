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
        private Controls.Extensions.ExControl exControl1;

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
            this.exControl1 = new TerrainMapGUI.Controls.Extensions.ExControl();
            this.SuspendLayout();
            // 
            // exControl1
            // 
            this.exControl1.Location = new System.Drawing.Point(339, 285);
            this.exControl1.Name = "exControl1";
            this.exControl1.Size = new System.Drawing.Size(100, 100);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.exControl1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
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
        }
    }
}
