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
    class MainPageViewModel : IDisposable
    {
        private HardwareButton hardwareButton;

        public BleScanner Scanner { get; private set; }

        public ReactiveCommand StartCommand { get; private set; }
        public ReactiveCommand StopCommand { get; private set; }
        public ReactiveCommand ClearCommand { get; private set; }

        private CompositeDisposable disposables = new CompositeDisposable();

        public MainPageViewModel()
        {
            this.Scanner = new BleScanner();
            this.hardwareButton = new HardwareButton(5);
            this.disposables.Add(this.hardwareButton);

            this.StartCommand = this.Scanner.IsStopped.ToReactiveCommand();
            this.StartCommand
                .Select(_ => Unit.Default)
                .Merge(this.hardwareButton.PressedObservable)
                .Select(_ => { this.Scanner.Start(); return Unit.Default; })
                .Subscribe()
                .AddTo(this.disposables);
            this.StopCommand = this.Scanner.IsStopped.Select(x => !x).ToReactiveCommand();
            this.StopCommand
                .Select(_ => { this.Scanner.Stop(); return Unit.Default; })
                .Subscribe()
                .AddTo(this.disposables);
            this.ClearCommand = new ReactiveCommand();
            this.ClearCommand
                .Select(_ => { this.Scanner.ClearAdvertisements(); return Unit.Default; })
                .Subscribe()
                .AddTo(this.disposables);
        }

        public void Dispose()
        {
            this.disposables.Dispose();
        }
    }
}
