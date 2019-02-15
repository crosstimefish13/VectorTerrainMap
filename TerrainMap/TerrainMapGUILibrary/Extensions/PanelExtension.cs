using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerrainMapGUILibrary.Resources.Fonts;

namespace TerrainMapGUILibrary.Extensions
{
    [DefaultEvent("Enter")]
    [DefaultProperty("Size")]
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    [ToolboxItemFilter("TerrainMapGUILibrary.Extensions")]
    public class PanelExtension : Panel
    {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [DefaultValue(typeof(Color), "White")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool CausesValidation
        {
            get
            {
                return base.CausesValidation;
            }
            set
            {
                base.CausesValidation = value;
            }
        }

        [DefaultValue(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return false;
            }
            set
            {
                base.TabStop = false;
            }
        }

        [DefaultValue(0)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DefaultValue(typeof(Size), "100, 100")]
        public new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler BackgroundImageChanged
        {
            add
            {
                base.BackgroundImageChanged += value;
            }
            remove
            {
                base.BackgroundImageChanged -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler BackgroundImageLayoutChanged
        {
            add
            {
                base.BackgroundImageLayoutChanged += value;
            }
            remove
            {
                base.BackgroundImageLayoutChanged -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler CausesValidationChanged
        {
            add
            {
                base.CausesValidationChanged += value;
            }
            remove
            {
                base.CausesValidationChanged -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler ContextMenuStripChanged
        {
            add
            {
                base.ContextMenuStripChanged += value;
            }
            remove
            {
                base.ContextMenuStripChanged -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event DragEventHandler DragDrop
        {
            add
            {
                base.DragDrop += value;
            }
            remove
            {
                base.DragDrop -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event DragEventHandler DragEnter
        {
            add
            {
                base.DragEnter += value;
            }
            remove
            {
                base.DragEnter -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler DragLeave
        {
            add
            {
                base.DragLeave += value;
            }
            remove
            {
                base.DragLeave -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event DragEventHandler DragOver
        {
            add
            {
                base.DragOver += value;
            }
            remove
            {
                base.DragOver -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler FontChanged
        {
            add
            {
                base.FontChanged += value;
            }
            remove
            {
                base.FontChanged -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler TabStopChanged
        {
            add
            {
                base.TabStopChanged += value;
            }
            remove
            {
                base.TabStopChanged -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler TabIndexChanged
        {
            add
            {
                base.TabIndexChanged += value;
            }
            remove
            {
                base.TabIndexChanged -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler TextChanged
        {
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }

        public PanelExtension()
        {
            Font = FontHelper.GetFont();
            Size = new Size(100, 100);
            BackColor = Color.White;
            TabStop = false;
            TabIndex = 0;
        }
    }
}
