// ReSharper disable ConvertClosureToMethodGroup
namespace Simple.Wpf.DataGrid.ViewModels
{
    using System;
    using System.Globalization;
    using System.Reactive.Linq;
    using Extensions;
    using Models;
    using Services;

    public sealed class DiagnosticsViewModel : BaseViewModel, IDiagnosticsViewModel
    {
        private string _cpu;
        private string _managedMemory;
        private string _totalMemory;

        public DiagnosticsViewModel(IDiagnosticsService diagnosticsService, ISchedulerService schedulerService)
        {
            Cpu = Constants.UI.Diagnostics.DefaultCpuString;
            ManagedMemory = Constants.UI.Diagnostics.DefaultManagedMemoryString;
            TotalMemory = Constants.UI.Diagnostics.DefaultTotalMemoryString;

            diagnosticsService.Cpu
                .Select(x => FormatCpu(x))
                .DistinctUntilChanged()
                .ObserveOn(schedulerService.Dispatcher)
                .Subscribe(x => Cpu = x,
                e =>
                {
                    Logger.Error(e);
                    Cpu = Constants.UI.Diagnostics.DefaultCpuString;
                })
                .DisposeWith(this);

            diagnosticsService.Memory
                .Select(x => FormatMemory(x))
                .DistinctUntilChanged()
                .ObserveOn(schedulerService.Dispatcher)
                .Subscribe(x =>
                {
                    ManagedMemory = x.ManagedMemory;
                    TotalMemory = x.TotalMemory;
                },
                e =>
                {
                    Logger.Error(e);
                    ManagedMemory = Constants.UI.Diagnostics.DefaultManagedMemoryString;
                    TotalMemory = Constants.UI.Diagnostics.DefaultTotalMemoryString;
                })
                .DisposeWith(this);
        }

        public string Cpu
        {
            get { return _cpu; }
            set { SetPropertyAndNotify(ref _cpu, value, () => Cpu); }
        }

        public string ManagedMemory
        {
            get { return _managedMemory; }
            set { SetPropertyAndNotify(ref _managedMemory, value, () => ManagedMemory); }
        }

        public string TotalMemory
        {
            get { return _totalMemory; }
            set { SetPropertyAndNotify(ref _totalMemory, value, () => TotalMemory); }
        }

        private static string FormatCpu(int cpu)
        {
            return cpu < 10
                ? $"CPU: 0{cpu.ToString(CultureInfo.InvariantCulture)} %"
                : $"CPU: {cpu.ToString(CultureInfo.InvariantCulture)} %";
        }

        private static FormattedMemory FormatMemory(Memory memory)
        {
            var managedMemory = $"Managed Memory: {memory.ManagedAsString()}";
            var totalMemory = $"Total Memory: {memory.WorkingSetPrivateAsString()}";

            return new FormattedMemory(managedMemory, totalMemory);
        }

        internal struct FormattedMemory
        {
            public string ManagedMemory { get; }
            public string TotalMemory { get; }

            public FormattedMemory(string managedMemory, string totalMemory)
            {
                ManagedMemory = managedMemory;
                TotalMemory = totalMemory;
            }
        }
    }
}