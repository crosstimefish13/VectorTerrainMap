using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Themes;

namespace TerrainMapGUILibrary.Extensions
{
    internal class PictureBoxExtension : PictureBox
    {
        [DefaultValue(typeof(Font), FontTheme.NormalString)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }


        [DefaultValue(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get { return false; }
            set { base.TabStop = false; }
        }

        [DefaultValue(0)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get { return 0; }
            set { base.TabIndex = 0; }
        }


        public PictureBoxExtension()
           : base()
        {
            Font = FontTheme.Normal();
            TabStop = false;
            TabIndex = 0;
        }
    }
}
