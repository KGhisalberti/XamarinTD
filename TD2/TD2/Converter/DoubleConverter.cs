using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace TD2.Converter {
    class DoubleConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            double n = (double)value;
            return n.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            string n = (string)value;
            try {
                return double.Parse(n);
            } catch {
                return 0;
            }
        }

    }
}
