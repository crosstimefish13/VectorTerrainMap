using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TerrainMapGUI.Controls.Extensions
{
    internal class ExTextBox : TextBox
    {
        private string watermarkText;

        private Font watermarkFont;

        private Color watermarkColor;


        [Browsable(true)]
        [DefaultValue(16)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength { get; set; }

        [Browsable(true)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool NumberInput { get; set; }

        [Browsable(true)]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string WatermarkText
        {
            get { return watermarkText; }
            set
            {
                watermarkText = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(Font), "Arial, 13px, style=Italic")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font WatermarkFont
        {
            get { return watermarkFont; }
            set
            {
                watermarkFont = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(Color), "GrayText")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color WatermarkColor
        {
            get { return watermarkColor; }
            set
            {
                watermarkColor = value;
                Invalidate();
            }
        }


        public ExTextBox()
            : base()
        {
            MaxDecimalLength = 16;
            NumberInput = true;
            watermarkText = "";
            watermarkFont = new Font("Arial", 13, FontStyle.Italic, GraphicsUnit.Pixel);
            watermarkColor = SystemColors.GrayText;
        }


        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // do not check number input
            if (NumberInput == false)
            { base.OnKeyPress(e); }
            // allow control keys
            else if (char.IsControl(e.KeyChar))
            { base.OnKeyPress(e); }
            // allow first + and -
            else if (Text.Length == 0 && (e.KeyChar == '+' || e.KeyChar == '-'))
            { base.OnKeyPress(e); }
            else
            {
                // prevent if over max decimal length
                int dotIndex = Text.IndexOf('.');
                if (dotIndex >= 0 && TextLength - dotIndex > MaxDecimalLength)
                { e.Handled = true; }
                else
                {
                    // prevent if it is not a number
                    string newText = Text + e.KeyChar;
                    if (double.TryParse(newText, out double result) == false)
                    { e.Handled = true; }
                }

                base.OnKeyPress(e);
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0xf && Focused == false && string.IsNullOrEmpty(Text) == true
                && string.IsNullOrEmpty(watermarkText) == false)
            {
                if (Enabled == true && Visible == true)
                {
                    var g = CreateGraphics();
                    TextRenderer.DrawText(g, watermarkText, watermarkFont, ClientRectangle,
                        watermarkColor, BackColor, TextFormatFlags.Top | TextFormatFlags.Left);
                    g.Dispose();
                }
            }
        }
    }
}
