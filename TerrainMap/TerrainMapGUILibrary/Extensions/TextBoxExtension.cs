using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Resources.Fonts;
using TerrainMapLibrary.Mathematics;

namespace TerrainMapGUILibrary.Extensions
{
    internal class TextBoxExtension : TextBox
    {
        private int maxDecimalLength;

        private bool numberInput;

        private string watermarkText;

        private Font watermarkFont;

        private Color watermarkColor;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength
        {
            get
            {
                return maxDecimalLength;
            }
            set
            {
                // value must be more than or equal 0
                maxDecimalLength = value;
                if (maxDecimalLength < 0)
                {
                    maxDecimalLength = 0;
                }

                if (numberInput == true)
                {
                    Text = string.Empty;
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool NumberInput
        {
            get
            {
                return numberInput;
            }
            set
            {
                numberInput = value;

                if (numberInput == true)
                {
                    Text = string.Empty;
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string WatermarkText
        {
            get
            {
                return watermarkText;
            }
            set
            {
                watermarkText = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font WatermarkFont
        {
            get
            {
                return watermarkFont;
            }
            set
            {
                watermarkFont = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color WatermarkColor
        {
            get
            {
                return watermarkColor;
            }
            set
            {
                watermarkColor = value;
                Invalidate();
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                string fixedValue = value;
                if (numberInput == true)
                {
                    // need to format text as number
                    if (string.IsNullOrEmpty(value) == true ||
                        double.TryParse(value, out double number) == false ||
                        double.IsNaN(number) == true ||
                        double.IsInfinity(number) == true)
                    {
                        fixedValue = string.Empty;
                    }
                    else
                    {
                        fixedValue = Common.ToNumberString(number, maxDecimalLength);
                    }
                }

                base.Text = fixedValue;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return true;
            }
            set
            {
                base.TabStop = true;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

        public TextBoxExtension()
        {
            maxDecimalLength = 8;
            numberInput = false;
            watermarkText = "";
            watermarkFont = FontHelper.GetFont(FontStyle.Italic);
            watermarkColor = SystemColors.GrayText;
            Font = FontHelper.GetFont();
            TabStop = true;
            TabIndex = 0;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (NumberInput == false)
            {
                // do not check number input
                base.OnKeyPress(e);
            }
            else if (char.IsControl(e.KeyChar) == true)
            {
                // allow control keys
                base.OnKeyPress(e);
            }
            else if (Text.Length == 0 && (e.KeyChar == '+' || e.KeyChar == '-'))
            {
                // allow first + and -
                base.OnKeyPress(e);
            }
            else
            {
                int dotIndex = Text.IndexOf('.');
                if (dotIndex >= 0 && TextLength - dotIndex > MaxDecimalLength)
                {
                    // prevent if over max decimal length
                    e.Handled = true;
                }
                else
                {
                    string newText = Text + e.KeyChar;
                    if (double.TryParse(newText, out double result) == false)
                    {
                        // prevent if it is not a number
                        e.Handled = true;
                    }
                }

                base.OnKeyPress(e);
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0xf &&
                Focused == false &&
                string.IsNullOrEmpty(Text) == true &&
                string.IsNullOrEmpty(watermarkText) == false)
            {
                // draw watermark text if needed
                if (Enabled == true && Visible == true)
                {
                    var g = CreateGraphics();
                    TextRenderer.DrawText(
                        g,
                        watermarkText,
                        watermarkFont,
                        ClientRectangle,
                        watermarkColor,
                        BackColor,
                        TextFormatFlags.Top | TextFormatFlags.Left
                    );
                    g.Dispose();
                }
            }
        }
    }
}
