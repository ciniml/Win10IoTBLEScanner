using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Reactive.Bindings.Extensions;

namespace IoTBLEScannerHeaded
{
    class HardwareButton : IDisposable
    {
        public int PinNumber { get; private set; }
        public IObservable<Unit> PressedObservable { get; private set; }

        private GpioPin pin;
        
        
        public HardwareButton(int pinNumber)
        {
            this.PinNumber = pinNumber;

            if (ApiInformation.IsTypePresent(typeof(GpioPin).FullName))
            {
                var controller = GpioController.GetDefault();
                this.pin = controller.OpenPin(this.PinNumber);
                this.pin.SetDriveMode(GpioPinDriveMode.Input);
                this.pin.DebounceTimeout = TimeSpan.FromMilliseconds(10);

                this.PressedObservable = Observable
                    .FromEvent<TypedEventHandler<GpioPin, GpioPinValueChangedEventArgs>, GpioPinValueChangedEventArgs>(
                        handler => (o, e) => handler(e),
                        handler => this.pin.ValueChanged += handler,
                        handler => this.pin.ValueChanged -= handler)
                    .Where(e => e.Edge == GpioPinEdge.FallingEdge)
                    .Select(_ => Unit.Default);
            }
            else
            {
                this.PressedObservable = Observable.Empty<Unit>();
            }
        }

        public void Dispose()
        {
            if (this.pin != null)
            {
                this.pin.Dispose();
                this.pin = null;
            }
        }
    }
}
