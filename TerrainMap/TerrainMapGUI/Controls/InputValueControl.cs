using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUI.Controls.Extensions;

namespace TerrainMapGUI.Controls
{
    public sealed class InputValueControl : ExControl
    {
        private ExTextBox txbMinX;

        private CheckBox chbMinX;

        private TrackValueControl tvcMinX;


        [Browsable(true)]
        [Category("Function")]
        [Description("Watermark text for value input.")]
        [DefaultValue("Value")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string WatermarkText
        {
            get { return txbMinX.WatermarkText; }
            set { txbMinX.WatermarkText = value; }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Max decimal length for value input.")]
        [DefaultValue(16)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength
        {
            get { return txbMinX.MaxDecimalLength; }
            set
            {
                if (value >= 0)
                {
                    txbMinX.MaxDecimalLength = value;
                    tvcMinX.MaxDecimalLength = value;
                }
            }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Value input.")]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double Value
        {
            get
            {
                if (double.TryParse(txbMinX.Text, out double value) == true) { return value; }
                else { return double.NaN; }
            }
            set
            {
                if (chbMinX.Checked == true) { tvcMinX.TrackValue = value; }
                else
                {
                    // input decimal length should be less or equal than max decimal length
                    string valueString = value.ToString();
                    int dotIndex = valueString.IndexOf('.');
                    if ((dotIndex >= 0 && valueString.Substring(dotIndex, valueString.Length - dotIndex - 1).Length
                            <= txbMinX.MaxDecimalLength) || dotIndex < 0)
                    { txbMinX.Text = valueString; }
                }
            }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Occurs when value changed.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ValueChanged;


        public InputValueControl()
           : base()
        {
            InitializeComponent();
        }


        private void InitializeComponent()
        {
            txbMinX = new ExTextBox();
            chbMinX = new CheckBox();
            tvcMinX = new TrackValueControl();
            SuspendLayout();
            // 
            // txbMinX
            // 
            txbMinX.Text = "0";
            txbMinX.WatermarkText = "Value";
            txbMinX.NumberInput = true;
            txbMinX.MaxDecimalLength = 16;
            txbMinX.Location = new Point(1, 1);
            txbMinX.Size = new Size(100, 22);
            txbMinX.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txbMinX.TextChanged += (sender, e) =>
            { if (ValueChanged != null) { ValueChanged.Invoke(this, new EventArgs()); } };
            Controls.Add(txbMinX);
            // 
            // chbMinX
            // 
            chbMinX.Text = "Use Track:";
            chbMinX.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            chbMinX.Location = new Point(110, 2);
            chbMinX.Size = new Size(90, 18);
            chbMinX.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            chbMinX.TabStop = false;
            chbMinX.CheckedChanged += (sender, e) =>
            {
                txbMinX.Enabled = !chbMinX.Checked;
                tvcMinX.Enabled = chbMinX.Checked;
            };
            Controls.Add(chbMinX);
            // 
            // tvcMinX
            // 
            tvcMinX.Location = new Point(200, 0);
            tvcMinX.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            tvcMinX.Enabled = false;
            tvcMinX.TrackValueChanged += (sender, e) => { txbMinX.Text = tvcMinX.TrackValue.ToString(); };
            Controls.Add(tvcMinX);
            // 
            // this
            // 
            Size = new Size(553, 24);
            ValueChanged = null;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
