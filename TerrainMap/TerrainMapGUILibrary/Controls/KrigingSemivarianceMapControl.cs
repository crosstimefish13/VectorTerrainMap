using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TerrainMapGUILibrary.Components;
using TerrainMapGUILibrary.Extensions;
using TerrainMapLibrary.Interpolator.Kriging;

namespace TerrainMapGUILibrary.Controls
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    [ToolboxItemFilter("TerrainMapGUILibrary.Controls")]
    public sealed class KrigingSemivarianceMapControl : ControlExtension
    {
        private SemivarianceMap map;

        private SemivarianceMap.Chart chart;

        private Image dataImage;

        private Label lblMinX;

        private InputValueComponent ivcMinX;

        private Label lblMinY;

        private InputValueComponent ivcMinY;

        private Label lblMaxX;

        private InputValueComponent ivcMaxX;

        private Label lblMaxY;

        private InputValueComponent ivcMaxY;

        private Label lblModel;

        private ComboBox cmbModel;

        private FoldPanelComponent fpcContainer;

        private PictureBox pcbImage;


        [Browsable(true)]
        [Category("Function")]
        [Description("Max decimal length for value input.")]
        [DefaultValue(16)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxDecimalLength
        {
            get { return ivcMinX.MaxDecimalLength; }
            set
            {
                ivcMinX.MaxDecimalLength = value;
                ivcMinY.MaxDecimalLength = value;
                ivcMaxX.MaxDecimalLength = value;
                ivcMaxY.MaxDecimalLength = value;
            }
        }

        [Browsable(true)]
        [Category("Function")]
        [Description("Value for map.")]
        [DefaultValue(typeof(MapValue), "0, 0, 0, 0")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public MapValue Value
        {
            get
            {
                var value = new MapValue(ivcMinX.Value, ivcMinY.Value, ivcMaxX.Value, ivcMaxY.Value);
                return value;
            }
            set
            {
                if (value != null)
                {
                    ivcMinX.ValueChanged -= UpdateCurve;
                    ivcMinY.ValueChanged -= UpdateCurve;
                    ivcMaxX.ValueChanged -= UpdateCurve;
                    ivcMaxY.ValueChanged -= UpdateCurve;
                    ivcMinX.Value = value.MinX;
                    ivcMinY.Value = value.MinY;
                    ivcMaxX.Value = value.MaxX;
                    ivcMaxY.Value = value.MaxY;
                    ivcMinX.ValueChanged += UpdateCurve;
                    ivcMinY.ValueChanged += UpdateCurve;
                    ivcMaxX.ValueChanged += UpdateCurve;
                    ivcMaxY.ValueChanged += UpdateCurve;

                    UpdateCurve();
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
            get { return ivcMinX.TabStop; }
            set
            {
                // control self tab stop always be false
                ivcMinX.TabStop = value;
                ivcMinY.TabStop = value;
                ivcMaxX.TabStop = value;
                ivcMaxY.TabStop = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new int TabIndex
        {
            get { return ivcMinX.TabIndex; }
            set
            {
                ivcMinX.TabIndex = value;
                ivcMinY.TabIndex = value;
                ivcMaxX.TabIndex = value;
                ivcMaxY.TabIndex = value;
            }
        }


        public KrigingSemivarianceMapControl()
        {
            map = null;
            chart = null;
            dataImage = null;

            InitializeComponent();
        }


        public void LoadData(SemivarianceMap map)
        {
            if (map == null) { return; }
            if (dataImage != null) { dataImage.Dispose(); }

            this.map = map;
            chart = map.GetChart(Size.Width, Size.Height, 40f);
            dataImage = new Bitmap(Size.Width, Size.Height);

            // draw data
            var g = Graphics.FromImage(dataImage);
            map.DrawData(g, chart);
            g.Dispose();

            // update value, it would draw curve
            double minX = chart.MinVector.EuclidDistance;
            double minY = chart.MinVector.Semivariance;
            double maxX = chart.MaxVector.EuclidDistance;
            double maxY = chart.MaxVector.Semivariance;
            Value = new MapValue(minX, minY, maxX, maxY);
        }


        private void InitializeComponent()
        {
            lblMinX = new Label();
            ivcMinX = new InputValueComponent();
            lblMinY = new Label();
            ivcMinY = new InputValueComponent();
            lblMaxX = new Label();
            ivcMaxX = new InputValueComponent();
            lblMaxY = new Label();
            ivcMaxY = new InputValueComponent();
            lblModel = new Label();
            cmbModel = new ComboBox();
            fpcContainer = new FoldPanelComponent();
            pcbImage = new PictureBox();
            SuspendLayout();
            // 
            // lblMinX
            // 
            lblMinX.Text = "Min X:";
            lblMinX.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            lblMinX.Location = new Point(1, 30);
            lblMinX.Size = new Size(49, 19);
            lblMinX.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            fpcContainer.Controls.Add(lblMinX);
            // 
            // ivcMinX
            // 
            ivcMinX.Value = 0;
            ivcMinX.MaxDecimalLength = 16;
            ivcMinX.WatermarkText = "Min X";
            ivcMinX.Location = new Point(50, 26);
            ivcMinX.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ivcMinX.ValueChanged += UpdateCurve;
            fpcContainer.Controls.Add(ivcMinX);
            // 
            // lblMinY
            // 
            lblMinY.Text = "Min Y:";
            lblMinY.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            lblMinY.Location = new Point(1, 58);
            lblMinY.Size = new Size(49, 19);
            lblMinY.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            fpcContainer.Controls.Add(lblMinY);
            // 
            // ivcMinY
            // 
            ivcMinY.Value = 0;
            ivcMinY.MaxDecimalLength = 16;
            ivcMinY.WatermarkText = "Min Y";
            ivcMinY.Location = new Point(50, 54);
            ivcMinY.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ivcMinY.ValueChanged += UpdateCurve;
            fpcContainer.Controls.Add(ivcMinY);
            // 
            // lblMaxX
            // 
            lblMaxX.Text = "Max X:";
            lblMaxX.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            lblMaxX.Location = new Point(1, 86);
            lblMaxX.Size = new Size(49, 19);
            lblMaxX.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            fpcContainer.Controls.Add(lblMaxX);
            // 
            // ivcMaxX
            // 
            ivcMaxX.Value = 0;
            ivcMaxX.MaxDecimalLength = 16;
            ivcMaxX.WatermarkText = "Max X";
            ivcMaxX.Location = new Point(50, 82);
            ivcMaxX.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ivcMaxX.ValueChanged += UpdateCurve;
            fpcContainer.Controls.Add(ivcMaxX);
            // 
            // lblMaxY
            // 
            lblMaxY.Text = "Max Y:";
            lblMaxY.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            lblMaxY.Location = new Point(1, 114);
            lblMaxY.Size = new Size(49, 19);
            lblMaxY.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            fpcContainer.Controls.Add(lblMaxY);
            // 
            // ivcMaxY
            // 
            ivcMaxY.Value = 0;
            ivcMaxY.MaxDecimalLength = 16;
            ivcMaxY.WatermarkText = "Max Y";
            ivcMaxY.Location = new Point(50, 110);
            ivcMaxY.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ivcMaxY.ValueChanged += UpdateCurve;
            fpcContainer.Controls.Add(ivcMaxY);
            // 
            // lblModel
            // 
            lblModel.Text = "Model:";
            lblModel.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            lblModel.Location = new Point(1, 142);
            lblModel.Size = new Size(49, 19);
            lblModel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            fpcContainer.Controls.Add(lblModel);
            // 
            // cmbModel
            // 
            cmbModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModel.Items.AddRange(typeof(Model).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Model)) && t.IsAbstract == false)
                .Select(t => t.Name)
                .ToArray());
            cmbModel.SelectedIndex = 0;
            cmbModel.Font = new Font("Arial", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            cmbModel.Location = new Point(51, 138);
            cmbModel.Size = new Size(190, 24);
            cmbModel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            cmbModel.SelectedIndexChanged += UpdateCurve;
            fpcContainer.Controls.Add(cmbModel);
            // 
            // fpcContainer
            // 
            fpcContainer.Title = "Kriging Semivariance Map";
            fpcContainer.MinSize = new Size(180, 22);
            fpcContainer.MaxSize = new Size(613, 168);
            fpcContainer.Location = new Point(12, 12);
            fpcContainer.Size = new Size(613, 168);
            fpcContainer.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            fpcContainer.IsFoldedChanged += (sender, e) =>
            {
                ivcMinX.Enabled = !fpcContainer.IsFolded;
                ivcMinY.Enabled = !fpcContainer.IsFolded;
                ivcMaxX.Enabled = !fpcContainer.IsFolded;
                ivcMaxY.Enabled = !fpcContainer.IsFolded;
            };
            fpcContainer.IsFolded = true;
            Controls.Add(fpcContainer);
            // 
            // pcbImage
            // 
            pcbImage.Location = new Point(0, 0);
            pcbImage.Size = new Size(650, 200);
            pcbImage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            Controls.Add(pcbImage);
            // 
            // this
            // 
            Value = new MapValue(0, 0, 0, 0);
            Size = new Size(650, 200);
            ValueChanged = null;
            ResumeLayout(false);
            PerformLayout();
        }


        private void UpdateCurve(object sender = null, EventArgs e = null)
        {
            if (dataImage == null || chart == null || map == null || Value.IsValid() == false) { return; }

            // get drawing model
            var modelType = typeof(Model).Assembly.GetTypes()
                .FirstOrDefault(t => t.Name == cmbModel.Items[cmbModel.SelectedIndex].ToString());
            if (modelType == null) { return; }
            var model = Activator.CreateInstance(modelType, Value.MinX, Value.MinY, Value.MaxX, Value.MaxY) as Model;

            // draw curve
            var image = (Image)dataImage.Clone();
            var g = Graphics.FromImage(image);
            map.DrawModelCurve(g, chart, model);
            g.Dispose();

            // update image
            var oldImage = pcbImage.Image;
            pcbImage.Image = image;
            if (oldImage != null) { oldImage.Dispose(); }

            if (ValueChanged != null) { ValueChanged.Invoke(this, new EventArgs()); }
        }


        [TypeConverter(typeof(MapValueConverter))]
        public sealed class MapValue
        {
            public double MinX { get; set; }

            public double MinY { get; set; }

            public double MaxX { get; set; }

            public double MaxY { get; set; }


            public MapValue(double minX = 0, double minY = 0, double maxX = 0, double maxY = 0)
            {
                MinX = minX;
                MinY = minY;
                MaxX = maxX;
                MaxY = maxY;
            }


            public bool IsValid()
            {
                bool isInvalid = double.IsNaN(MinX) == true || double.IsInfinity(MinX) == true
                    || double.IsNaN(MinY) == true || double.IsInfinity(MinY) == true
                    || double.IsNaN(MaxX) == true || double.IsInfinity(MaxX) == true
                    || double.IsNaN(MaxY) == true || double.IsInfinity(MaxY) == true;

                return !isInvalid;
            }


            public static bool operator ==(MapValue left, MapValue right)
            {
                // compare object reference
                if (left is null && right is null) { return true; }
                else if (left is null || right is null) { return false; }
                // compare values
                else if (Math.Abs(left.MinX - right.MinX) > double.Epsilon) { return false; }
                else if (Math.Abs(left.MinY - right.MinY) > double.Epsilon) { return false; }
                else if (Math.Abs(left.MaxX - right.MaxX) > double.Epsilon) { return false; }
                else if (Math.Abs(left.MaxY - right.MaxY) > double.Epsilon) { return false; }
                else { return true; }
            }

            public static bool operator !=(MapValue left, MapValue right)
            {
                return !(left == right);
            }


            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is MapValue)) { return false; }

                return this == (obj as MapValue);
            }

            public override int GetHashCode()
            {
                return MinX.GetHashCode() + MinY.GetHashCode() + MaxX.GetHashCode() + MaxY.GetHashCode();
            }

            public override string ToString()
            {
                return $"MinX:{MinX}, MinY:{MinY}, MaxX:{MaxX}, MaxY:{MaxY}";
            }
        }


        public sealed class MapValueConverter : TypeConverter
        {
            public MapValueConverter()
            { }


            public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
            {
                if (propertyValues != null)
                {
                    // create MapValue instance by using properties from UI Designer
                    var propNames = typeof(MapValue).GetProperties().Select(p => p.Name).ToArray();
                    var minX = propertyValues[propNames[0]];
                    var minY = propertyValues[propNames[1]];
                    var maxX = propertyValues[propNames[2]];
                    var maxY = propertyValues[propNames[3]];

                    if (minX != null && minY != null && maxX != null && maxY != null
                        && minX is double && minY is double && maxX is double && maxY is double)
                    {
                        var result = new MapValue((double)minX, (double)minY, (double)maxX, (double)maxY);
                        return result;
                    }
                }

                return base.CreateInstance(context, propertyValues);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string)) { return true; }

                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value != null && value is string)
                {
                    // convert from string to MapValue
                    if (culture == null) { culture = CultureInfo.CurrentCulture; }
                    char sep = culture.TextInfo.ListSeparator[0];

                    var parts = value.ToString().Split(new char[] { sep }, StringSplitOptions.RemoveEmptyEntries);
                    var result = new MapValue();
                    if (parts.Length >= 4)
                    {
                        if (double.TryParse(parts[0].Trim(), out double minX) == true) { result.MinX = minX; }
                        if (double.TryParse(parts[1].Trim(), out double minY) == true) { result.MinY = minY; }
                        if (double.TryParse(parts[2].Trim(), out double maxX) == true) { result.MaxX = maxX; }
                        if (double.TryParse(parts[3].Trim(), out double maxY) == true) { result.MaxY = maxY; }
                    }

                    return result;
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(InstanceDescriptor)) { return true; }

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                Type destinationType)
            {
                if (value != null && destinationType != null)
                {
                    var source = value as MapValue;
                    if (destinationType == typeof(string))
                    {
                        // convert from MapValue to string
                        if (culture == null) { culture = CultureInfo.CurrentCulture; }
                        char sep = culture.TextInfo.ListSeparator[0];

                        string result = $"{source.MinX}{sep} {source.MinY}{sep} {source.MaxX}{sep} {source.MaxY}";
                        return result;
                    }
                    else if (destinationType == typeof(InstanceDescriptor))
                    {
                        // convert from instance to MapValue
                        var ci = typeof(MapValue).GetConstructor(new Type[]
                            { typeof(double), typeof(double), typeof(double), typeof(double) });
                        if (ci != null)
                        {
                            var instance = new InstanceDescriptor(ci,
                                new object[] { source.MinX, source.MinY, source.MaxX, source.MaxY });
                            return instance;
                        }
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
                Attribute[] attributes)
            {
                // get properties from MapValue
                var propNames = typeof(MapValue).GetProperties().Select(p => p.Name).ToArray();
                var propDescs = TypeDescriptor.GetProperties(typeof(MapValue), attributes);

                // TODO : sort is not work
                propDescs.Sort(propNames);
                return propDescs;
            }
        }
    }
}
