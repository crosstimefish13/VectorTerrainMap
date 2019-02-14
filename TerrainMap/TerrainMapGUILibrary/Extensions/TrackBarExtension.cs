using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TerrainMapGUILibrary.Resources.Fonts;

namespace TerrainMapGUILibrary.Extensions
{
    internal class TrackBarExtension : TrackBar
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return false;
            }
            set
            {
                base.TabStop = false;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get
            {
                return 0;
            }
            set
            {
                base.TabIndex = 0;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new virtual bool ShowFocusCues { get; set; }

        public TrackBarExtension()
        {
            ShowFocusCues = false;
            Font = FontHelper.GetFont();
            TabStop = false;
            TabIndex = 0;
        }

        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            // hide forus cues ifneeded
            if (ShowFocusCues == false)
            {
                SendMessage(Handle, 0x0128, (1 << 16) | (0x1 & 0xffff), 0);
            }
        }
    }
}
