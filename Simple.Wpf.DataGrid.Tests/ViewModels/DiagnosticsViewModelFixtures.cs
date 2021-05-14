using System;
using System.Reactive.Subjects;
using Moq;
using NUnit.Framework;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.Services;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Tests.ViewModels
{
    [TestFixture]
    public sealed class DiagnosticsViewModelFixtures : BaseViewModelFixtures
    {
        [SetUp]
        public void SetUp()
        {
            _diagnosticService = new Mock<IDiagnosticsService>();

            _cpuSubject = new Subject<int>();
            _diagnosticService.Setup(x => x.Cpu)
                .Returns(_cpuSubject);

            _memorySubject = new Subject<Memory>();
            _diagnosticService.Setup(x => x.Memory)
                .Returns(_memorySubject);
        }

        private Mock<IDiagnosticsService> _diagnosticService;
        private Subject<int> _cpuSubject;
        private Subject<Memory> _memorySubject;

        [Test]
        public void cpu_value_is_default_value_when_diagnostics_service_cpu_errors()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            _cpuSubject.OnError(new Exception("blah!"));

            TestScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo(Constants.UI.Diagnostics.DefaultCpuString));
        }

        [Test]
        public void cpu_value_is_formatted_when_diagnostics_service_pumps_cpu()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            _cpuSubject.OnNext(42);

            TestScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo("CPU: 42 %"));
        }

        [Test]
        public void disposing_unsubscribes_diagnostics_service_stream()
        {
            // ARRANGE
            const decimal managedMemory = 1024 * 1000 * 4;
            const decimal totalMemory = 1024 * 1000 * 42;

            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            viewModel.Dispose();

            _memorySubject.OnNext(new Memory(totalMemory, managedMemory));
            _cpuSubject.OnNext(42);

            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo(Constants.UI.Diagnostics.DefaultCpuString));
            Assert.That(viewModel.TotalMemory, Is.EqualTo(Constants.UI.Diagnostics.DefaultTotalMemoryString));
            Assert.That(viewModel.ManagedMemory, Is.EqualTo(Constants.UI.Diagnostics.DefaultManagedMemoryString));
        }

        [Test]
        public void managed_memory_value_is_default_value_when_diagnostics_service_memory_errors()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            _memorySubject.OnError(new Exception("blah!"));

            TestScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.ManagedMemory, Is.EqualTo(Constants.UI.Diagnostics.DefaultManagedMemoryString));
        }

        [Test]
        public void managed_memory_value_is_formatted_when_diagnostics_service_pumps_memory()
        {
            // ARRANGE
            const decimal managedMemory = 1024 * 1000 * 4;
            const decimal totalMemory = 1024 * 1000 * 42;

            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            _memorySubject.OnNext(new Memory(totalMemory, managedMemory));

            TestScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.ManagedMemory, Is.EqualTo("Managed Memory: 4.00 Mb"));
        }

        [Test]
        public void total_memory_value_is_default_value_when_diagnostics_service_memory_errors()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            _memorySubject.OnError(new Exception("blah!"));

            TestScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.TotalMemory, Is.EqualTo(Constants.UI.Diagnostics.DefaultTotalMemoryString));
        }

        [Test]
        public void total_memory_value_is_formatted_when_diagnostics_service_pumps_memory()
        {
            // ARRANGE
            const decimal managedMemory = 1024 * 1000 * 4;
            const decimal totalMemory = 1024 * 1000 * 42;

            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            _memorySubject.OnNext(new Memory(totalMemory, managedMemory));

            TestScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.TotalMemory, Is.EqualTo("Total Memory: 42.00 Mb"));
        }

        [Test]
        public void when_created_cpu_is_default_value()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo(Constants.UI.Diagnostics.DefaultCpuString));
        }

        [Test]
        public void when_created_managed_memory_is_default_value()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            // ASSERT
            Assert.That(viewModel.ManagedMemory, Is.EqualTo(Constants.UI.Diagnostics.DefaultManagedMemoryString));
        }

        [Test]
        public void when_created_total_memory_is_default_value()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, SchedulerService);

            // ACT
            // ASSERT
            Assert.That(viewModel.TotalMemory, Is.EqualTo(Constants.UI.Diagnostics.DefaultTotalMemoryString));
        }
    }
}