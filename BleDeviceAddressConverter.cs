using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using Windows.UI.Xaml.Data;
using Reactive.Bindings.Extensions;

namespace IoTBLEScannerHeaded
{
    /// <summary>
    /// Converts an address of a BLE device represented by ulong into "AA:BB:CC:DD:EE:FF" style string.
    /// </summary>
    public class BleDeviceAddressConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof (string))
            {
                throw new NotSupportedException();
            }
            if (value == null)
            {
                return "";
            }
            if (!(value is ulong))
            {
                throw new NotSupportedException();
            }

            var typedValue = (ulong) value;
            return String.Join(":", BitConverter.GetBytes(typedValue).Take(6).Reverse().Select(x => x.ToString("X02")));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
