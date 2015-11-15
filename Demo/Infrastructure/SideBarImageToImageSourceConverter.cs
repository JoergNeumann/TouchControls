using Demo.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Demo.Infrastructure
{
    public class SideBarImageToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            SideBarImage imageType;
            if (Enum.TryParse<SideBarImage>(value.ToString(), out imageType))
            {
                return Application.Current.Resources[value.ToString()] as string;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
