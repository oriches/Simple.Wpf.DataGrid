using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Moq;
using NUnit.Framework;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Tests.Services
{
    [TestFixture]
    public sealed class ColumnsServiceFixtures : BaseServiceFixtures
    {
        [SetUp]
        public void SetUp()
        {
            _settingsService = new Mock<ISettingsService>();
        }

        private Mock<ISettingsService> _settingsService;

        [Test]
        public void does_not_hides_columns_for_invalid_identifier()
        {
            // ARRANGE
            var identifier1 = "Grid.1";
            var identifier2 = "Grid.2";
            var columns = new[] {"Col1", "Col2", "Col3", "Col4"};

            ISettings settings1 = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings1[Constants.UI.Settings.Names.Columns] = columns;
            settings1[Constants.UI.Settings.Names.VisibleColumns] = columns;

            ISettings settings2 = null;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifier1), out settings1))
                .Returns(true);

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifier2), out settings2))
                .Returns(false);

            var service = new ColumnsService(_settingsService.Object);

            string changedIdentifier = null;
            service.Changed
                .Subscribe(x => changedIdentifier = x);

            // ACT
            service.HideColumns(identifier2, new[] {"Col1", "Col2"});

            // ASSERT
            Assert.That(changedIdentifier, Is.Null);
            Assert.That(
                settings1.Get<IEnumerable<string>>(Constants.UI.Settings.Names.VisibleColumns).SequenceEqual(columns),
                Is.True);
        }


        [Test]
        public void does_not_show_columns_for_invalid_identifier()
        {
            // ARRANGE
            var identifier1 = "Grid.1";
            var identifier2 = "Grid.2";
            var columns = new[] {"Col1", "Col2", "Col3", "Col4"};

            ISettings settings1 = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings1[Constants.UI.Settings.Names.Columns] = columns;
            settings1[Constants.UI.Settings.Names.VisibleColumns] = Enumerable.Empty<string>();

            ISettings settings2 = null;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifier1), out settings1))
                .Returns(true);

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifier2), out settings2))
                .Returns(false);

            var service = new ColumnsService(_settingsService.Object);

            string changedIdentifier = null;
            service.Changed
                .Subscribe(x => changedIdentifier = x);

            // ACT
            service.ShowColumns(identifier2, new[] {"Col1", "Col2"});

            // ASSERT
            Assert.That(changedIdentifier, Is.Null);
            Assert.That(settings1.Get<IEnumerable<string>>(Constants.UI.Settings.Names.VisibleColumns), Is.Empty);
        }

        [Test]
        public void hides_columns_for_identifier()
        {
            // ARRANGE
            var identifier = "Grid.1";
            var columns = new[] {"Col1", "Col2", "Col3", "Col4"};

            ISettings settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings[Constants.UI.Settings.Names.Columns] = columns;
            settings[Constants.UI.Settings.Names.VisibleColumns] = columns;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifier), out settings))
                .Returns(true);

            var service = new ColumnsService(_settingsService.Object);

            string changedIdentifier = null;
            service.Changed
                .Subscribe(x => changedIdentifier = x);

            // ACT
            service.HideColumns(identifier, new[] {"Col1", "Col2"});

            // ASSERT
            Assert.That(changedIdentifier, Is.EqualTo(identifier));
            Assert.That(settings.Get<IEnumerable<string>>(Constants.UI.Settings.Names.Columns).SequenceEqual(columns),
                Is.True);
            Assert.That(
                settings.Get<IEnumerable<string>>(Constants.UI.Settings.Names.VisibleColumns).SequenceEqual(columns),
                Is.False);
            Assert.That(
                settings.Get<IEnumerable<string>>(Constants.UI.Settings.Names.VisibleColumns)
                    .SequenceEqual(new[] {"Col3", "Col4"}), Is.True);
        }

        [Test]
        public void initialises_with_columns()
        {
            // ARRANGE
            var identifer = "Grid.1";
            var columns = new[] {"Col1", "Col2", "Col3"};

            var settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());

            _settingsService.Setup(x => x.CreateOrUpdate(It.Is<string>(y => y == identifer)))
                .Returns(settings);

            var service = new ColumnsService(_settingsService.Object);

            string initialised = null;
            service.Initialised
                .Subscribe(x => initialised = x);

            // ACT
            service.InitialiseColumns(identifer, columns);

            // ASSERT
            Assert.That(initialised, Is.EqualTo(identifer));
            Assert.That(settings[Constants.UI.Settings.Names.Columns], Is.EqualTo(columns));
            Assert.That(settings[Constants.UI.Settings.Names.VisibleColumns], Is.EqualTo(columns));
        }

        [Test]
        public void returns_all_columns_for_identifier()
        {
            // ARRANGE
            var identifer = "Grid.1";
            var columns = new[] {"Col1", "Col2", "Col3", "Col4"};
            var visibleColumns = new[] {"Col2", "Col4"};

            ISettings settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings[Constants.UI.Settings.Names.Columns] = columns;
            settings[Constants.UI.Settings.Names.VisibleColumns] = visibleColumns;
            ;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifer), out settings))
                .Returns(true);

            var service = new ColumnsService(_settingsService.Object);

            // ACT
            var allColumns = service.GetAllColumns(identifer);

            // ASSERT
            Assert.That(allColumns.SequenceEqual(allColumns), Is.True);
        }

        [Test]
        public void returns_empty_hidden_columns_for_invalid_identifier()
        {
            // ARRANGE
            var identifer1 = "Grid.1";
            var identifer2 = "Grid.2";
            var allColumns = new[] {"Col1", "Col2", "Col3", "Col4"};
            var visibleColumns = new[] {"Col2", "Col4"};

            ISettings settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings[Constants.UI.Settings.Names.Columns] = allColumns;
            settings[Constants.UI.Settings.Names.VisibleColumns] = visibleColumns;
            ;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifer1), out settings))
                .Returns(true);

            var service = new ColumnsService(_settingsService.Object);

            // ACT
            var hiddenColumns = service.HiddenColumns(identifer2);

            // ASSERT
            Assert.That(hiddenColumns, Is.Empty);
        }

        [Test]
        public void returns_empty_visible_columns_for_invalid_identifier()
        {
            // ARRANGE
            var identifer1 = "Grid.1";
            var identifer2 = "Grid.2";
            var columns = new[] {"Col1", "Col2", "Col3"};

            ISettings settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings[Constants.UI.Settings.Names.Columns] = columns;
            settings[Constants.UI.Settings.Names.VisibleColumns] = columns;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifer1), out settings))
                .Returns(true);

            var service = new ColumnsService(_settingsService.Object);

            // ACT
            var visibleColumns = service.VisibleColumns(identifer2);

            // ASSERT
            Assert.That(visibleColumns, Is.Empty);
        }

        [Test]
        public void returns_hidden_columns_for_valid_identifier()
        {
            // ARRANGE
            var identifer = "Grid.1";
            var allColumns = new[] {"Col1", "Col2", "Col3", "Col4"};
            var visibleColumns = new[] {"Col2", "Col4"};

            ISettings settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings[Constants.UI.Settings.Names.Columns] = allColumns;
            settings[Constants.UI.Settings.Names.VisibleColumns] = visibleColumns;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifer), out settings))
                .Returns(true);

            var service = new ColumnsService(_settingsService.Object);

            // ACT
            var hiddenColumns = service.HiddenColumns(identifer);

            // ASSERT
            Assert.That(hiddenColumns.SequenceEqual(new[] {"Col1", "Col3"}), Is.True);
        }

        [Test]
        public void returns_no_columns_for_invalid_identifier()
        {
            // ARRANGE
            var identifier1 = "Grid.1";
            var identifier2 = "Grid.2";

            var columns = new[] {"Col1", "Col2", "Col3", "Col4"};
            var visibleColumns = new[] {"Col2", "Col4"};

            ISettings settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings[Constants.UI.Settings.Names.Columns] = columns;
            settings[Constants.UI.Settings.Names.VisibleColumns] = visibleColumns;
            ;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifier1), out settings))
                .Returns(true);

            var service = new ColumnsService(_settingsService.Object);

            // ACT
            var allColumns = service.GetAllColumns(identifier2);

            // ASSERT
            Assert.That(allColumns, Is.Empty);
        }

        [Test]
        public void returns_visible_columns_for_valid_identifier()
        {
            // ARRANGE
            var identifer = "Grid.1";
            var columns = new[] {"Col1", "Col2", "Col3"};

            ISettings settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings[Constants.UI.Settings.Names.Columns] = columns;
            settings[Constants.UI.Settings.Names.VisibleColumns] = columns;

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifer), out settings))
                .Returns(true);

            var service = new ColumnsService(_settingsService.Object);

            // ACT
            var visibleColumns = service.VisibleColumns(identifer);

            // ASSERT
            Assert.That(visibleColumns.SequenceEqual(columns), Is.True);
        }

        [Test]
        public void shows_columns_for_identifier()
        {
            // ARRANGE
            var identifier = "Grid.1";
            var columns = new[] {"Col1", "Col2", "Col3", "Col4"};

            ISettings settings = new Settings(Enumerable.Empty<Setting>(), new Subject<bool>());
            settings[Constants.UI.Settings.Names.Columns] = columns;
            settings[Constants.UI.Settings.Names.VisibleColumns] = Enumerable.Empty<string>();

            _settingsService.Setup(x => x.TryGet(It.Is<string>(y => y == identifier), out settings))
                .Returns(true);

            var service = new ColumnsService(_settingsService.Object);

            string changedIdentifier = null;
            service.Changed
                .Subscribe(x => changedIdentifier = x);

            // ACT
            service.ShowColumns(identifier, new[] {"Col1", "Col2"});

            // ASSERT
            Assert.That(changedIdentifier, Is.EqualTo(identifier));
            Assert.That(settings.Get<IEnumerable<string>>(Constants.UI.Settings.Names.Columns).SequenceEqual(columns),
                Is.True);
            Assert.That(
                settings.Get<IEnumerable<string>>(Constants.UI.Settings.Names.VisibleColumns).SequenceEqual(columns),
                Is.False);
            Assert.That(
                settings.Get<IEnumerable<string>>(Constants.UI.Settings.Names.VisibleColumns)
                    .SequenceEqual(new[] {"Col1", "Col2"}), Is.True);
        }
    }
}