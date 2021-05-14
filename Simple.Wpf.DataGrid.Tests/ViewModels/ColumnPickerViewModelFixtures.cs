using System;
using System.Linq;
using System.Reactive.Linq;
using Moq;
using NUnit.Framework;
using Simple.Wpf.DataGrid.Services;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Tests.ViewModels
{
    [TestFixture]
    public sealed class ColumnPickerViewModelFixtures : BaseViewModelFixtures
    {
        [SetUp]
        public void SetUp()
        {
            _columnService = new Mock<IColumnsService>();

            _identifier = "Grid.1";
            _visibleColumns = new[] {"Col1", "Col2", "Col3", "Col4", "Col5"};
            _hiddenColumns = new[] {"Col6", "Col7", "Col8", "Col9", "Col10"};

            _columnService.Setup(x => x.Initialised)
                .Returns(Observable.Never<string>());

            _columnService.Setup(x => x.VisibleColumns(It.Is<string>(y => y == _identifier)))
                .Returns(_visibleColumns);

            _columnService.Setup(x => x.HiddenColumns(It.Is<string>(y => y == _identifier)))
                .Returns(_hiddenColumns);
        }

        private Mock<IColumnsService> _columnService;
        private string[] _visibleColumns;
        private string[] _hiddenColumns;
        private string _identifier;

        [Test]
        public void adds_hidden_column()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            var column = viewModel.Left.First();

            column.IsSelected = true;

            // ACT
            viewModel.AddCommand.Execute(null);

            // ASSERT
            Assert.That(viewModel.Left.Contains(column), Is.False);
            Assert.That(viewModel.Right.Contains(column), Is.True);
        }

        [Test]
        public void adds_multiple_hidden_columns()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            var columns = new[]
            {
                viewModel.Left.First(), viewModel.Left.Skip(1)
                    .First()
            };
            columns[0]
                .IsSelected = true;
            columns[1]
                .IsSelected = true;

            // ACT
            viewModel.AddCommand.Execute(null);

            // ASSERT
            Assert.That(viewModel.Left.Contains(columns[0]), Is.False);
            Assert.That(viewModel.Left.Contains(columns[1]), Is.False);
            Assert.That(viewModel.Right.Contains(columns[0]), Is.True);
            Assert.That(viewModel.Right.Contains(columns[1]), Is.True);
        }

        [Test]
        public void can_add_hidden_column()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // ACT

            viewModel.Left.First()
                .IsSelected = true;

            // ASSERT
            Assert.That(viewModel.AddCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void can_add_mulitple_hidden_columns()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // ACT

            viewModel.Left.First()
                .IsSelected = true;
            viewModel.Left.Skip(1)
                .First()
                .IsSelected = true;

            // ASSERT
            Assert.That(viewModel.AddCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void can_move_first_column_down()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // ACT

            viewModel.Right.First()
                .IsSelected = true;

            // ASSERT
            Assert.That(viewModel.MovedownCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void can_not_move_first_column_up()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // ACT

            viewModel.Right.First()
                .IsSelected = true;

            // ASSERT
            Assert.That(viewModel.MoveupCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void can_remove_multiple_visible_columns()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // ACT

            viewModel.Right.First()
                .IsSelected = true;
            viewModel.Right.Skip(1)
                .First()
                .IsSelected = true;

            // ASSERT
            Assert.That(viewModel.RemoveCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void can_remove_visible_column()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // ACT

            viewModel.Right.First()
                .IsSelected = true;

            // ASSERT
            Assert.That(viewModel.RemoveCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void moves_visible_column_down()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            var column = viewModel.Right.First();
            var columnIndex = viewModel.Right.ToList()
                .IndexOf(column);

            column.IsSelected = true;

            // ACT
            viewModel.MovedownCommand.Execute(null);

            // ASSERT
            Assert.That(viewModel.Right.ToList()
                .IndexOf(column), Is.EqualTo(++columnIndex));
        }


        [Test]
        public void moves_visible_column_up()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            var column = viewModel.Right.Last();
            var columnIndex = viewModel.Right.ToList()
                .IndexOf(column);

            column.IsSelected = true;

            // ACT
            viewModel.MoveupCommand.Execute(null);

            // ASSERT
            Assert.That(viewModel.Right.ToList()
                .IndexOf(column), Is.EqualTo(--columnIndex));
        }

        [Test]
        public void no_buttons_enabled_when_populated()
        {
            // ARRANGE
            // ACT
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // ASSERT
            Assert.That(viewModel.AddCommand.CanExecute(null), Is.False);
            Assert.That(viewModel.RemoveCommand.CanExecute(null), Is.False);
            Assert.That(viewModel.MoveupCommand.CanExecute(null), Is.False);
            Assert.That(viewModel.MovedownCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void populates_hidden_and_visible_columns_when_created()
        {
            // ARRANGE
            // ACT
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // ASSERT
            Assert.That(viewModel.Left, Is.Not.Empty);
            Assert.That(viewModel.Left.Select(x => x.Name)
                .SequenceEqual(_hiddenColumns), Is.True);

            Assert.That(viewModel.Right, Is.Not.Empty);
            Assert.That(viewModel.Right.Select(x => x.Name)
                .SequenceEqual(_visibleColumns), Is.True);
        }

        [Test]
        public void removes_multiple_visible_columns()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            var columns = new[]
            {
                viewModel.Right.First(), viewModel.Right.Skip(1)
                    .First()
            };
            columns[0]
                .IsSelected = true;
            columns[1]
                .IsSelected = true;

            // ACT
            viewModel.RemoveCommand.Execute(null);

            // ASSERT
            Assert.That(viewModel.Left.Contains(columns[0]), Is.True);
            Assert.That(viewModel.Left.Contains(columns[1]), Is.True);
            Assert.That(viewModel.Right.Contains(columns[0]), Is.False);
            Assert.That(viewModel.Right.Contains(columns[1]), Is.False);
        }

        [Test]
        public void removes_visible_column()
        {
            // ARRANGE
            var viewModel = new ColumnPickerViewModel(_identifier, _columnService.Object, SchedulerService);

            TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            var column = viewModel.Right.First();

            column.IsSelected = true;

            // ACT
            viewModel.RemoveCommand.Execute(null);

            // ASSERT
            Assert.That(viewModel.Left.Contains(column), Is.True);
            Assert.That(viewModel.Right.Contains(column), Is.False);
        }
    }
}