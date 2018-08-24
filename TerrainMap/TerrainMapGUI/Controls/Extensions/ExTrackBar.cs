using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TerrainMapGUI.Controls.Extensions
{
    internal class ExTrackBar : TrackBar
    {
        [Browsable(true)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new virtual bool ShowFocusCues { get; set; }

        public ExTrackBar()
            : base()
        {
            ShowFocusCues = false;
        }


        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (ShowFocusCues == false)
            { SendMessage(Handle, 0x0128, (1 << 16) | (0x1 & 0xffff), 0); }
        }
    }
}
