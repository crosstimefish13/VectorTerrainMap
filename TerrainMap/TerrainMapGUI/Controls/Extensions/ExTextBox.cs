using System.ComponentModel;
using System.Windows.Forms;

namespace TerrainMapGUI.Controls.Extensions
{
    internal class ExTextBox : TextBox
    {
        [Browsable(true)]
        [DefaultValue(16)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength { get; set; }

        [Browsable(true)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool NumberInput { get; set; }


        public ExTextBox()
            : base()
        {
            MaxDecimalLength = 16;
            NumberInput = true;
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
    }
}
