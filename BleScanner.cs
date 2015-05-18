using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;

namespace IoTBLEScannerHeaded
{
    /// <summary>
    /// Scans advertisements from BLE devices.
    /// </summary>
    public class BleScanner
    {
        /// <summary>
        /// An information about an advetisement from a BLE device.
        /// </summary>
        public class BleDeviceAdvertisement
        {
            /// <summary>
            /// Advertisement data
            /// </summary>
            public BluetoothLEAdvertisement Advertisement { get; private set; }
            /// <summary>
            /// Address of the BLE device which sends this advertisement.
            /// </summary>
            public ulong Address { get; private set; }
            /// <summary>
            /// RSSI value which indicates the signal strength from the BLE device.
            /// </summary>
            public int Rssi { get; private set; }

            public BleDeviceAdvertisement(BluetoothLEAdvertisement advertisement, ulong address, int rssi)
            {
                this.Advertisement = advertisement;
                this.Address = address;
                this.Rssi = rssi;
            }
        }
        private readonly BluetoothLEAdvertisementWatcher watcher;
        private readonly Subject<Unit> resetSubject = new Subject<Unit>();
        
        public ReactiveProperty<bool> IsStopped { get; private set; }
        public ReadOnlyReactiveCollection<BleDeviceAdvertisement> Advertisements { get; private set; }

        /// <summary>
        /// Start to watch advertisements.
        /// </summary>
        public void Start()
        {
            this.IsStopped.Value = false;
            this.watcher.Start();
        }
        /// <summary>
        /// Stop watching advertisements.
        /// </summary>
        public void Stop()
        {
            this.watcher.Stop();
        }
        /// <summary>
        /// Clear the advertisement collection.
        /// </summary>
        public void ClearAdvertisements()
        {
            this.resetSubject.OnNext(Unit.Default);
        }

        public BleScanner()
        {
            
            this.watcher = new BluetoothLEAdvertisementWatcher();
            
            this.Advertisements = Observable
                .FromEvent<TypedEventHandler<BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementReceivedEventArgs>, BluetoothLEAdvertisementReceivedEventArgs>(
                    handler => (o, e) => handler(e), handler => this.watcher.Received += handler, handler => this.watcher.Received -= handler)
                .Select(e => new BleDeviceAdvertisement(e.Advertisement, e.BluetoothAddress, e.RawSignalStrengthInDBm))
                .ToReadOnlyReactiveCollection(this.resetSubject);
            this.IsStopped = Observable
                .FromEvent<TypedEventHandler<BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementWatcherStoppedEventArgs>, BluetoothLEAdvertisementWatcherStoppedEventArgs>(
                    handler => (o, e) => handler(e), handler => this.watcher.Stopped += handler, handler => this.watcher.Stopped -= handler)
                .Select(_ => true)
                .ToReactiveProperty(true);
        }
    }
}
