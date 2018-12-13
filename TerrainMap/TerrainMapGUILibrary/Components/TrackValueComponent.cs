using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Extensions;

namespace TerrainMapGUILibrary.Components
{
    [DefaultEvent("TrackValueChanged")]
    [DefaultProperty("TrackValue")]
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Components")]
    public sealed class TrackValueComponent : ControlExtension
    {
        private TextBoxExtension txbMinValue;

        private ButtonExtension btnMinMove;

        private TrackBarExtension trbValue;

        private ButtonExtension btnMaxMove;

        private TextBoxExtension txbMaxValue;

        [Category("Function")]
        [Description("Max decimal length for min max value input.")]
        [DefaultValue(8)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength
        {
            get
            {
                return txbMinValue.MaxDecimalLength;
            }
            set
            {
                txbMinValue.MaxDecimalLength = value;
                txbMaxValue.MaxDecimalLength = value;
            }
        }

        [Category("Function")]
        [Description("Min value to limit the track value.")]
        [DefaultValue(double.NaN)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double MinValue
        {
            get
            {
                if (string.IsNullOrEmpty(txbMinValue.Text) == true)
                {
                    return double.NaN;
                }
                else
                {
                    return double.Parse(txbMinValue.Text);
                }
            }
            set
            {
                txbMinValue.Text = value.ToString();
            }
        }

        [Category("Function")]
        [Description("Max value to limit the track value.")]
        [DefaultValue(double.NaN)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double MaxValue
        {
            get
            {
                if (string.IsNullOrEmpty(txbMaxValue.Text) == true)
                {
                    return double.NaN;
                }
                else
                {
                    return double.Parse(txbMaxValue.Text);
                }
            }
            set
            {
                txbMaxValue.Text = value.ToString();
            }
        }

        [Category("Function")]
        [Description("The track value.")]
        [DefaultValue(double.NaN)]
        [Browsable(true)]
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
                    if (trackValue < minValue)
                    {
                        trackValue = minValue;
                    }
                    else if (trackValue > maxValue)
                    {
                        trackValue = maxValue;
                    }

                    return trackValue;
                }
                else
                {
                    return double.NaN;
                }
            }
            set
            {
                if (trbValue.Enabled == true)
                {
                    // get trb value by input value
                    double minValue = MinValue;
                    double maxValue = MaxValue;
                    int trackValue = 49;
                    if (value <= minValue)
                    {
                        trackValue = trbValue.Minimum;
                    }
                    else if (value >= maxValue)
                    {
                        trackValue = trbValue.Maximum;
                    }
                    else
                    {
                        trackValue = (int)((value - minValue) / (maxValue - minValue) * 99);
                    }

                    trbValue.Value = trackValue;
                }
            }
        }

        [Category("Function")]
        [Description("Determines the index in the TAB order that the input control(s) will occupy. There are 2 controls index would be set.")]
        [DefaultValue(0)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int StartTabIndex
        {
            get
            {
                return txbMinValue.TabIndex;
            }
            set
            {
                txbMinValue.TabIndex = value;
                txbMaxValue.TabIndex = value + 1;
            }
        }

        [Category("Function")]
        [Description("Occurs when min value changed.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler MinValueChanged;

        [Category("Function")]
        [Description("Occurs when max value changed.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler MaxValueChanged;

        [Category("Function")]
        [Description("Occurs when track value changed.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler TrackValueChanged;

        public TrackValueComponent()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            txbMinValue = new TextBoxExtension();
            btnMinMove = new ButtonExtension();
            trbValue = new TrackBarExtension();
            btnMaxMove = new ButtonExtension();
            txbMaxValue = new TextBoxExtension();
            SuspendLayout();
            // 
            // txbMinValue
            // 
            txbMinValue.WatermarkText = "Min Value";
            txbMinValue.NumberInput = true;
            txbMinValue.MaxDecimalLength = 8;
            txbMinValue.Location = new Point(1, 1);
            txbMinValue.Size = new Size(100, 22);
            txbMinValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txbMinValue.TabIndex = 0;
            txbMinValue.TextChanged += (sender, e) => 
            {
                SetTrackControls(sender);
            };
            Controls.Add(txbMinValue);
            // 
            // btnMinMove
            // 
            btnMinMove.Text = "<";
            btnMinMove.Location = new Point(102, 0);
            btnMinMove.Size = new Size(24, 24);
            btnMinMove.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnMinMove.Enabled = false;
            btnMinMove.Click += (sender, e) =>
            {
                if (trbValue.Value - 1 >= trbValue.Minimum)
                {
                    SetTrackControls(sender, trbValue.Value - 1);
                }
            };
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
            trbValue.ValueChanged += (sender, e) =>
            {
                if (TrackValueChanged != null)
                {
                    TrackValueChanged.Invoke(this, new EventArgs());
                }
            };
            Controls.Add(trbValue);
            // 
            // btnMaxMove
            // 
            btnMaxMove.Text = ">";
            btnMaxMove.Location = new Point(226, 0);
            btnMaxMove.Size = new Size(24, 24);
            btnMaxMove.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnMaxMove.Enabled = false;
            btnMaxMove.Click += (sender, e) =>
            {
                if (trbValue.Value + 1 <= trbValue.Maximum)
                {
                    SetTrackControls(sender, trbValue.Value + 1);
                }
            };
            Controls.Add(btnMaxMove);
            // 
            // txbMaxValue
            // 
            txbMaxValue.WatermarkText = "Max Value";
            txbMaxValue.NumberInput = true;
            txbMaxValue.MaxDecimalLength = 8;
            txbMaxValue.Location = new Point(252, 1);
            txbMaxValue.Size = new Size(100, 22);
            txbMaxValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txbMaxValue.TabIndex = 0;
            txbMaxValue.TextChanged += (sender, e) =>
            {
                SetTrackControls(sender);
            };
            Controls.Add(txbMaxValue);
            // 
            // this
            // 
            Size = new Size(353, 24);
            MinValueChanged = null;
            MaxValueChanged = null;
            TrackValueChanged = null;
            StartTabIndex = 0;
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
                if (TrackValueChanged != null)
                {
                    TrackValueChanged.Invoke(this, new EventArgs());
                }
            }
            else
            {
                // disable track controls
                btnMinMove.Enabled = false;
                trbValue.Enabled = false;
                btnMaxMove.Enabled = false;
            }

            if (sender is TextBoxExtension)
            {
                // invoke min max value changed
                if (sender == txbMinValue && MinValueChanged != null)
                {
                    MinValueChanged.Invoke(this, new EventArgs());
                }
                else if (sender == txbMaxValue && MaxValueChanged != null)
                {
                    MaxValueChanged.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
