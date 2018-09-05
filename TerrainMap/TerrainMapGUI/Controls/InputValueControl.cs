using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUI.Controls.Extensions;

namespace TerrainMapGUI.Controls
{
    public sealed class InputValueControl : ExControl
    {
        private ExTextBox txbValue;

        private CheckBox chbTrackValue;

        private TrackValueControl tvcValue;


        [Browsable(true)]
        [Category("Function")]
        [Description("Watermark text for value input.")]
        [DefaultValue("Value")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string WatermarkText
        {
            get { return txbValue.WatermarkText; }
            set { txbValue.WatermarkText = value; }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Max decimal length for value input.")]
        [DefaultValue(16)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength
        {
            get { return txbValue.MaxDecimalLength; }
            set
            {
                if (value >= 0)
                {
                    txbValue.MaxDecimalLength = value;
                    tvcValue.MaxDecimalLength = value;
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
                if (double.TryParse(txbValue.Text, out double value) == true) { return value; }
                else { return double.NaN; }
            }
            set
            {
                if (chbTrackValue.Checked == true) { tvcValue.TrackValue = value; }
                else
                {
                    // input decimal length should be less or equal than max decimal length
                    string valueString = value.ToString();
                    int dotIndex = valueString.IndexOf('.');
                    if ((dotIndex >= 0 && valueString.Substring(dotIndex, valueString.Length - dotIndex - 1).Length
                            <= txbValue.MaxDecimalLength) || dotIndex < 0)
                    { txbValue.Text = valueString; }
                }
            }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Occurs when value changed.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ValueChanged;


        [Browsable(true)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool TabStop
        {
            get { return txbValue.TabStop; }
            set
            {
                // control self tab stop always be false
                txbValue.TabStop = value;
                tvcValue.TabStop = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new int TabIndex
        {
            get { return txbValue.TabIndex; }
            set
            {
                txbValue.TabIndex = value;
                tvcValue.TabIndex = value;
            }
        }


        public InputValueControl()
           : base()
        {
            InitializeComponent();
        }


        private void InitializeComponent()
        {
            txbValue = new ExTextBox();
            chbTrackValue = new CheckBox();
            tvcValue = new TrackValueControl();
            SuspendLayout();
            // 
            // txbMinX
            // 
            txbValue.Text = "0";
            txbValue.WatermarkText = "Value";
            txbValue.NumberInput = true;
            txbValue.MaxDecimalLength = 16;
            txbValue.Location = new Point(1, 1);
            txbValue.Size = new Size(100, 22);
            txbValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txbValue.TextChanged += (sender, e) =>
            { if (ValueChanged != null) { ValueChanged.Invoke(this, new EventArgs()); } };
            Controls.Add(txbValue);
            // 
            // chbMinX
            // 
            chbTrackValue.Text = "Use Track:";
            chbTrackValue.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            chbTrackValue.Location = new Point(110, 2);
            chbTrackValue.Size = new Size(90, 18);
            chbTrackValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            chbTrackValue.TabStop = false;
            chbTrackValue.CheckedChanged += (sender, e) =>
            {
                txbValue.Enabled = !chbTrackValue.Checked;
                tvcValue.Enabled = chbTrackValue.Checked;
            };
            Controls.Add(chbTrackValue);
            // 
            // tvcMinX
            // 
            tvcValue.Location = new Point(200, 0);
            tvcValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            tvcValue.Enabled = false;
            tvcValue.TrackValueChanged += (sender, e) => { txbValue.Text = tvcValue.TrackValue.ToString(); };
            Controls.Add(tvcValue);
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
