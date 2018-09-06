using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TerrainMapGUILibrary.Extensions
{
    internal class TrackBarExtension : TrackBar
    {
        [Browsable(true)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new virtual bool ShowFocusCues { get; set; }

        public TrackBarExtension()
            : base()
        {
            ShowFocusCues = false;
            Font = new Font("Arial", 13, FontStyle.Regular, GraphicsUnit.Pixel);
        }


        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            // hide forus cues ifneeded
            if (ShowFocusCues == false)
            { SendMessage(Handle, 0x0128, (1 << 16) | (0x1 & 0xffff), 0); }
        }
    }
}
