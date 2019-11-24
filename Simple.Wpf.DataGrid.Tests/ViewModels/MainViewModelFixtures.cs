using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Moq;
using NUnit.Framework;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.Services;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Tests.ViewModels
{
    [TestFixture]
    public sealed class MainViewModelFixtures : BaseViewModelFixtures
    {
        [SetUp]
        public void SetUp()
        {
            _diagnosticsViewModel = new Mock<IDiagnosticsViewModel>();

            _data = new Subject<IEnumerable<DynamicData>>();
            _tabularDataService = new Mock<ITabularDataService>();
            _tabularDataService.Setup(x => x.GetAsync(It.IsAny<IScheduler>())).Returns(_data);

            _columnsChanged = new Subject<string>();
            _columnsInitialised = new Subject<string>();

            _columnsService = new Mock<IColumnsService>();
            _columnsService.Setup(x => x.Changed).Returns(_columnsChanged);
            _columnsService.Setup(x => x.Initialised).Returns(_columnsInitialised);
            _columnsService.Setup(x => x.InitialiseColumns(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Callback<string, IEnumerable<string>>((y, z) =>
                {
                    _columnsService.Setup(x => x.VisibleColumns(It.IsAny<string>())).Returns(z);
                });

            _overlayService = new Mock<IOverlayService>();
            _dateTimeService = new MockDateTimeService(TestScheduler);

            TestScheduler.AdvanceTo(DateTime.Now.Ticks);
        }

        private Mock<IDiagnosticsViewModel> _diagnosticsViewModel;
        private Mock<ITabularDataService> _tabularDataService;
        private Mock<IColumnsService> _columnsService;
        private Mock<IOverlayService> _overlayService;
        private MockDateTimeService _dateTimeService;
        private Subject<string> _columnsChanged;
        private Subject<string> _columnsInitialised;
        private Subject<IEnumerable<DynamicData>> _data;

        private MainViewModel CreateViewModel()
        {
            return new MainViewModel(_diagnosticsViewModel.Object,
                _tabularDataService.Object,
                _columnsService.Object,
                _overlayService.Object,
                _dateTimeService,
                SchedulerService);
        }

        [Test]
        public void created_with_no_data()
        {
            // ARRANGE
            // ACT
            var viewModel = CreateViewModel();

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(viewModel.Data, Is.Empty);
            Assert.That(viewModel.HasData, Is.False);
            Assert.That(viewModel.TotalNumberOfRows, Is.EqualTo(0));
            Assert.That(viewModel.VisibleColumns, Is.Empty);
            Assert.That(viewModel.TotalNumberOfColumns, Is.EqualTo(0));
            Assert.That(viewModel.TotalNumberOfValues, Is.EqualTo(0));
            Assert.That(viewModel.UpdatesPerSecond, Is.EqualTo(0));
        }

        [Test]
        public void populated_with_data_when_data_service_pumps()
        {
            // ARRANGE
            var data = new[]
            {
                new DynamicData
                {
                    {"id", 1},
                    {"col1", 1},
                    {"col2", 2},
                    {"col3", 3}
                },
                new DynamicData
                {
                    {"id", 2},
                    {"col1", 1},
                    {"col2", 2},
                    {"col3", 3}
                }
            };

            var viewModel = CreateViewModel();

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            _data.OnNext(data);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(viewModel.Data, Is.Not.Empty);
            Assert.That(viewModel.HasData, Is.True);
            Assert.That(viewModel.TotalNumberOfRows, Is.EqualTo(2));
            Assert.That(viewModel.VisibleColumns, Is.Not.Empty);
            Assert.That(viewModel.TotalNumberOfColumns, Is.EqualTo(6));
            Assert.That(viewModel.TotalNumberOfValues, Is.EqualTo(12));
            Assert.That(viewModel.UpdatesPerSecond, Is.EqualTo(0));
        }
    }
}