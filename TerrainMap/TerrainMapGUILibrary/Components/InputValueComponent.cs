using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Extensions;

namespace TerrainMapGUILibrary.Components
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Components")]
    public sealed class InputValueComponent : ControlExtension
    {
        private TextBoxExtension txbValue;

        private CheckBoxExtension chbTrackValue;

        private TrackValueComponent tvcValue;

        [Category("Function")]
        [Description("Watermark text for value input.")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string WatermarkText
        {
            get { return txbValue.WatermarkText; }
            set { txbValue.WatermarkText = value; }
        }

        [Category("Function")]
        [Description("Max decimal length for value input.")]
        [DefaultValue(16)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength
        {
            get { return txbValue.MaxDecimalLength; }
            set
            {
                txbValue.MaxDecimalLength = value;
                tvcValue.MaxDecimalLength = value;
            }
        }

        [Category("Function")]
        [Description("Value input.")]
        [DefaultValue(double.NaN)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double Value
        {
            get
            {
                if (string.IsNullOrEmpty(txbValue.Text) == true) { return double.NaN; }
                else { return double.Parse(txbValue.Text); }
            }
            set { txbValue.Text = value.ToString(); }
        }

        [Category("Function")]
        [Description("Determines the index in the TAB order that the input control(s) will occupy. There are 3 controls index would be set.")]
        [DefaultValue(0)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int StartTabIndex
        {
            get { return txbValue.TabIndex; }
            set
            {
                txbValue.TabIndex = value;
                tvcValue.StartTabIndex = value + 1;
            }
        }

        [Category("Function")]
        [Description("Occurs when value changed.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ValueChanged;

        public InputValueComponent()
           : base()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            txbValue = new TextBoxExtension();
            chbTrackValue = new CheckBoxExtension();
            tvcValue = new TrackValueComponent();
            SuspendLayout();
            // 
            // txbValue
            // 
            txbValue.WatermarkText = "Value";
            txbValue.NumberInput = true;
            txbValue.MaxDecimalLength = 16;
            txbValue.Location = new Point(1, 1);
            txbValue.Size = new Size(100, 22);
            txbValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txbValue.TabIndex = 0;
            txbValue.TextChanged += (sender, e) =>
            { if (ValueChanged != null) { ValueChanged.Invoke(this, new EventArgs()); } };
            Controls.Add(txbValue);
            // 
            // chbTrackValue
            // 
            chbTrackValue.Text = "Use Track:";
            chbTrackValue.Location = new Point(110, 2);
            chbTrackValue.Size = new Size(90, 18);
            chbTrackValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            chbTrackValue.CheckedChanged += (sender, e) =>
            {
                txbValue.Enabled = !chbTrackValue.Checked;
                tvcValue.Enabled = chbTrackValue.Checked;
            };
            Controls.Add(chbTrackValue);
            // 
            // tvcValue
            // 
            tvcValue.MaxDecimalLength = 16;
            tvcValue.MinValue = 0;
            tvcValue.MaxValue = 0;
            tvcValue.TrackValue = 0;
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
            StartTabIndex = 0;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
