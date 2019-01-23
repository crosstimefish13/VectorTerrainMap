using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TerrainMapGUILibrary.Extensions;

namespace TerrainMapGUILibrary.Collections
{
    [ListBindable(false)]
    public sealed class PanelCollection : IList, ICollection, IEnumerable
    {
        public IOwner Owner { get; set; }

        public Panel this[int index]
        {
            get
            {
                return Owner.GetItems()[index];
            }
            set
            {
                Owner.GetItems()[index] = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public int Count
        {
            get
            {
                return Owner.GetItems().Count;
            }
        }

        public PanelCollection(IOwner owner)
        {
            Owner = owner;
        }

        public int Add(Panel value)
        {
            Owner.GetItems().Add(value);
            return Owner.GetItems().Count - 1;
        }

        public void Clear()
        {
            Owner.GetItems().Clear();
        }

        public bool Contains(Panel value)
        {
            return Owner.GetItems().Contains(value);
        }

        public void CopyTo(IEnumerable<Panel> array, int index)
        {
            Owner.GetItems().CopyTo(array.ToArray(), index);
        }

        public IEnumerator GetEnumerator()
        {
            return Owner.GetItems().Cast<Panel>().GetEnumerator();
        }

        public int IndexOf(Panel value)
        {
            return Owner.GetItems().IndexOf(value);
        }

        public void Insert(int index, Panel value)
        {
            Owner.GetItems().Insert(index, value);
        }

        public void Remove(Panel value)
        {
            Owner.GetItems().Remove(value);
        }

        public void RemoveAt(int index)
        {
            Owner.GetItems().RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return Owner.GetItems()[index];
            }
            set
            {
                Owner.GetItems()[index] = value as Panel;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

        int IList.Add(object value)
        {
            Owner.GetItems().Add(value as Panel);
            return Owner.GetItems().Count - 1;
        }

        bool IList.Contains(object value)
        {
            return Owner.GetItems().Contains(value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Owner.GetItems().CopyTo(array.Cast<Panel>().ToArray(), index);
        }

        int IList.IndexOf(object value)
        {
            return Owner.GetItems().IndexOf(value as Panel);
        }

        void IList.Insert(int index, object value)
        {
            Owner.GetItems().Insert(index, value as Panel);
        }

        void IList.Remove(object value)
        {
            Owner.GetItems().Remove(value as Panel);
        }

        [DefaultProperty("Name")]
        [DesignTimeVisible(false)]
        [ToolboxItem(false)]
        [TypeConverter(typeof(PanelConverter))]
        public class Panel : PanelExtension
        {
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override bool AutoSize
            {
                get
                {
                    return base.AutoSize;
                }
                set
                {
                    base.AutoSize = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override AnchorStyles Anchor
            {
                get
                {
                    return base.Anchor;
                }
                set
                {
                    base.Anchor = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override DockStyle Dock
            {
                get
                {
                    return base.Dock;
                }
                set
                {
                    base.Dock = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Padding Margin
            {
                get
                {
                    return base.Margin;
                }
                set
                {
                    base.Margin = value;
                }
            }

            [DefaultValue(typeof(Size), "0, 0")]
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override Size MaximumSize
            {
                get
                {
                    return base.MaximumSize;
                }
                set
                {
                    base.MaximumSize = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override Size MinimumSize
            {
                get
                {
                    return base.MinimumSize;
                }
                set
                {
                    base.MinimumSize = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point Location
            {
                get
                {
                    return base.Location;
                }
                set
                {
                    base.Location = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Padding Padding
            {
                get
                {
                    return base.Padding;
                }
                set
                {
                    base.Padding = value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size PreferredSize
            {
                get
                {
                    return base.PreferredSize;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
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
            public new event EventHandler AutoSizeChanged
            {
                add
                {
                    base.AutoSizeChanged += value;
                }
                remove
                {
                    base.AutoSizeChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler DockChanged
            {
                add
                {
                    base.DockChanged += value;
                }
                remove
                {
                    base.DockChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler LocationChanged
            {
                add
                {
                    base.LocationChanged += value;
                }
                remove
                {
                    base.LocationChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler MarginChanged
            {
                add
                {
                    base.MarginChanged += value;
                }
                remove
                {
                    base.MarginChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler PaddingChanged
            {
                add
                {
                    base.PaddingChanged += value;
                }
                remove
                {
                    base.PaddingChanged -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler Resize
            {
                add
                {
                    base.Resize += value;
                }
                remove
                {
                    base.Resize -= value;
                }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler SizeChanged
            {
                add
                {
                    base.SizeChanged += value;
                }
                remove
                {
                    base.SizeChanged -= value;
                }
            }

            public Panel()
            {
                Dock = DockStyle.Fill;
            }

            public override string ToString()
            {
                return GetType().Name;
            }
        }

        public class PanelConverter : ExpandableObjectConverter
        {
            public PanelConverter()
            { }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(InstanceDescriptor))
                {
                    return true;
                }

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (value != null && destinationType != null)
                {
                    if (value is Panel && destinationType == typeof(InstanceDescriptor))
                    {
                        // convert from instance PanelContent
                        var argumentTypes = GetConstructorArgumentTypes();
                        var constructorInfo = typeof(Panel).GetConstructor(argumentTypes);
                        if (constructorInfo != null)
                        {
                            var arguments = GetConstructorArguments();
                            var instanceDescriptor = new InstanceDescriptor(constructorInfo, arguments);
                            return instanceDescriptor;
                        }
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            protected Type[] GetConstructorArgumentTypes()
            {
                return new Type[] { };
            }

            protected object[] GetConstructorArguments()
            {
                return new object[] { };
            }
        }

        public interface IOwner
        {
            ObservableCollection<Panel> GetItems();
        }
    }
}
