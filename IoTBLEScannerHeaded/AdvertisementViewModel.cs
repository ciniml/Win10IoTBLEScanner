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
using Reactive.Bindings.Extensions;

namespace IoTBLEScannerHeaded
{
    class AdvertisementViewModel : IDisposable
    {
        private readonly BleScanner.BleDeviceAdvertisement advertisement;
        private CompositeDisposable disposables = new CompositeDisposable();

        public string AddressString { get; private set; }
        public AdvertisementViewModel(BleScanner.BleDeviceAdvertisement advertisement)
        {
            this.advertisement = advertisement;

            this.AddressString = string.Join(":", BitConverter.GetBytes(this.advertisement.Address).Select(segment => segment.ToString("X02")));
        }

        public void Dispose()
        {
            this.disposables.Dispose();
        }
    }
}
