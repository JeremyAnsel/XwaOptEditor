using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace XwaHangarMapEditor
{
    public sealed class Visual3DConverter : IMultiValueConverter
    {
        public static readonly Visual3DConverter Default = new Visual3DConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: model
            // values[1]: IList<Visual3D>
            // values[2]: Visual3D

            if (values.Take(3).Any(t => t == DependencyProperty.UnsetValue))
            {
                return null;
            }

            if (values[0] == null)
            {
                return null;
            }

            if (values[1] is not IList<Visual3D> items)
            {
                return null;
            }

            if (values[2] is not Visual3D hangar)
            {
                return null;
            }

            var model = (ModelVisual3D)values[0];
            model.Children.Clear();

            foreach (var item in items)
            {
                model.Children.Add(item);
            }

            model.Children.Add(hangar);

            //var viewport = GetViewport(model);

            //if (viewport != null)
            //{
            //    viewport.ZoomExtents();
            //}

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //private static HelixViewport3D GetViewport(Visual3D model)
        //{
        //    DependencyObject obj = model;

        //    while (obj != null)
        //    {
        //        obj = VisualTreeHelper.GetParent(obj);

        //        if (obj is HelixViewport3D)
        //        {
        //            break;
        //        }
        //    }

        //    if (obj == null)
        //    {
        //        return null;
        //    }

        //    return (HelixViewport3D)obj;
        //}
    }
}
