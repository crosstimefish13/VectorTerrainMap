using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Resources.Fonts;

namespace TerrainMapGUILibrary.Extensions
{
    internal class PictureBoxExtension : PictureBox
    {
        public List<Image> PendingImages { get; private set; }

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

        [Browsable(false)]
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

        public PictureBoxExtension()
        {
            PendingImages = new List<Image>();
            Font = FontHelper.GetFont();
            TabStop = false;
            TabIndex = 0;
        }
    }
}
