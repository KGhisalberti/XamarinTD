using System;
using System.Globalization;

using Xamarin.Forms;

namespace TD2.Converter {
    public class ImageIdConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int imageId = (int)value;
            return Urls.URI + Urls.IMAGE + "/" + imageId;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
