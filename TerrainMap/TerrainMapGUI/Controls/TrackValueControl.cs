using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUI.Controls.Extensions;

namespace TerrainMapGUI.Controls
{
    [DefaultEvent("TrackValueChanged")]
    [DefaultProperty("TrackValue")]
    [ToolboxItemFilter("TerrainMapGUI.Controls")]
    public sealed class TrackValueControl : ExControl
    {
        private ExTextBox txbMinValue;

        private Button btnMinMove;

        private ExTrackBar trbValue;

        private Button btnMaxMove;

        private ExTextBox txbMaxValue;


        [Browsable(true)]
        [Category("Function")]
        [Description("Max decimal length for min max value input.")]
        [DefaultValue(16)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength
        {
            get { return txbMinValue.MaxDecimalLength; }
            set
            {
                if (value >= 0)
                {
                    txbMinValue.MaxDecimalLength = value;
                    txbMaxValue.MaxDecimalLength = value;
                }
            }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Min value to limit the track value.")]
        [DefaultValue(double.NaN)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double MinValue
        {
            get
            {
                if (double.TryParse(txbMinValue.Text, out double minValue) == true) { return minValue; }
                else { return double.NaN; }
            }
            set
            {
                // input decimal length should be less or equal than max decimal length
                string minValueString = value.ToString();
                int dotIndex = minValueString.IndexOf('.');
                if ((dotIndex >= 0 && minValueString.Substring(dotIndex, minValueString.Length - dotIndex - 1).Length
                        <= txbMinValue.MaxDecimalLength) || dotIndex < 0)
                {
                    txbMinValue.Text = minValueString;
                    SetTrackControls(txbMinValue);
                }
            }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Max value to limit the track value.")]
        [DefaultValue(double.NaN)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double MaxValue
        {
            get
            {
                if (double.TryParse(txbMaxValue.Text, out double maxValue) == true) { return maxValue; }
                else { return double.NaN; }
            }
            set
            {
                // input decimal length should be less or equal than max decimal length
                string maxValueString = value.ToString();
                int dotIndex = maxValueString.IndexOf('.');
                if ((dotIndex >= 0 && maxValueString.Substring(dotIndex, maxValueString.Length - dotIndex - 1).Length
                        <= txbMaxValue.MaxDecimalLength) || dotIndex < 0)
                {
                    txbMaxValue.Text = maxValueString;
                    SetTrackControls(txbMaxValue);
                }
            }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("The track value.")]
        [DefaultValue(double.NaN)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double TrackValue
        {
            get
            {
                if (trbValue.Enabled == true)
                {
                    // get value by trb value
                    double minValue = MinValue;
                    double maxValue = MaxValue;
                    double trackValue = minValue + (maxValue - minValue) / 99 * trbValue.Value;
                    if (trackValue < minValue) { trackValue = minValue; }
                    else if (trackValue > maxValue) { trackValue = maxValue; }

                    return trackValue;
                }
                else { return double.NaN; }
            }
            set
            {
                if (trbValue.Enabled == true)
                {
                    // get trb value by input value
                    double minValue = MinValue;
                    double maxValue = MaxValue;
                    int trackValue = 49;
                    if (value <= minValue) { trackValue = trbValue.Minimum; }
                    else if (value >= maxValue) { trackValue = trbValue.Maximum; }
                    else { trackValue = (int)((value - minValue) / (maxValue - minValue) * 99); }

                    trbValue.Value = trackValue;
                }
            }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Occurs when min value changed.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler MinValueChanged;

        [Browsable(true)]
        [Category("Function")]
        [Description("Occurs when max value changed.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler MaxValueChanged;

        [Browsable(true)]
        [Category("Function")]
        [Description("Occurs when track value changed.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler TrackValueChanged;


        [Browsable(true)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool TabStop
        {
            get { return txbMinValue.TabStop; }
            set
            {
                // control self tab stop always be false
                txbMinValue.TabStop = value;
                txbMaxValue.TabStop = value;
                base.TabStop = false;
            }
        }

        [Browsable(true)]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new int TabIndex
        {
            get { return txbMinValue.TabIndex; }
            set
            {
                txbMinValue.TabIndex = value;
                txbMaxValue.TabIndex = value;
                base.TabIndex = value;
            }
        }


        public TrackValueControl()
            : base()
        {
            InitializeComponent();
        }


        private void InitializeComponent()
        {
            txbMinValue = new ExTextBox();
            btnMinMove = new Button();
            trbValue = new ExTrackBar();
            btnMaxMove = new Button();
            txbMaxValue = new ExTextBox();
            SuspendLayout();
            // 
            // txbMinValue
            // 
            txbMinValue.WatermarkText = "Min Value";
            txbMinValue.NumberInput = true;
            txbMinValue.MaxDecimalLength = 16;
            txbMinValue.Location = new Point(1, 1);
            txbMinValue.Size = new Size(100, 22);
            txbMinValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txbMinValue.TextChanged += (sender, e) => { SetTrackControls(sender); };
            txbMinValue.TabStopChanged += (sender, e) => { OnTabStopChanged(e); };
            Controls.Add(txbMinValue);
            // 
            // btnMinMove
            // 
            btnMinMove.Text = "<";
            btnMinMove.Location = new Point(102, 0);
            btnMinMove.Size = new Size(24, 24);
            btnMinMove.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnMinMove.Enabled = false;
            btnMinMove.TabStop = false;
            btnMinMove.Click += (sender, e) =>
            { if (trbValue.Value - 1 >= trbValue.Minimum) { SetTrackControls(sender, trbValue.Value - 1); } };
            Controls.Add(btnMinMove);
            // 
            // trbValue
            // 
            trbValue.Minimum = 0;
            trbValue.Maximum = 99;
            trbValue.TickFrequency = 1;
            trbValue.Value = 49;
            trbValue.TickStyle = TickStyle.None;
            trbValue.Location = new Point(126, 1);
            trbValue.Size = new Size(100, 22);
            trbValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            trbValue.Enabled = false;
            trbValue.TabStop = false;
            trbValue.ValueChanged += (sender, e) =>
            { if (TrackValueChanged != null) { TrackValueChanged.Invoke(this, new EventArgs()); } };
            Controls.Add(trbValue);
            // 
            // btnMaxMove
            // 
            btnMaxMove.Text = ">";
            btnMaxMove.Location = new Point(226, 0);
            btnMaxMove.Size = new Size(24, 24);
            btnMaxMove.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnMaxMove.Enabled = false;
            btnMaxMove.TabStop = false;
            btnMaxMove.Click += (sender, e) =>
            { if (trbValue.Value + 1 <= trbValue.Maximum) { SetTrackControls(sender, trbValue.Value + 1); } };
            Controls.Add(btnMaxMove);
            // 
            // txbMaxValue
            // 
            txbMaxValue.WatermarkText = "Max Value";
            txbMaxValue.NumberInput = true;
            txbMaxValue.MaxDecimalLength = 16;
            txbMaxValue.Location = new Point(252, 1);
            txbMaxValue.Size = new Size(100, 22);
            txbMaxValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txbMaxValue.TextChanged += (sender, e) => { SetTrackControls(sender); };
            Controls.Add(txbMaxValue);
            // 
            // this
            // 
            Size = new Size(353, 24);
            MinValueChanged = null;
            MaxValueChanged = null;
            TrackValueChanged = null;
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetTrackControls(object sender, int value = 49)
        {
            bool validMinValue = double.TryParse(txbMinValue.Text, out double minValue);
            bool validMaxValue = double.TryParse(txbMaxValue.Text, out double maxValue);

            if (validMinValue == true && validMaxValue == true && minValue < maxValue)
            {
                // valid min max value, enable track controls
                btnMinMove.Enabled = true;
                trbValue.Enabled = true;
                btnMaxMove.Enabled = true;

                // need to reset trb value
                trbValue.Value = value;
                if (TrackValueChanged != null) { TrackValueChanged.Invoke(this, new EventArgs()); }
            }
            else
            {
                // disable track controls
                btnMinMove.Enabled = false;
                trbValue.Enabled = false;
                btnMaxMove.Enabled = false;
            }

            if (sender is ExTextBox)
            {
                // invoke min max value changed
                if (sender == txbMinValue && MinValueChanged != null)
                { MinValueChanged.Invoke(this, new EventArgs()); }
                else if (sender == txbMaxValue && MaxValueChanged != null)
                { MaxValueChanged.Invoke(this, new EventArgs()); }
            }
        }
    }
}
