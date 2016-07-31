namespace Simple.Wpf.DataGrid.Tests.Services
{
    using DataGrid.Services;
    using Moq;
    using ObservableExtensions = Extensions.ObservableExtensions;

    public abstract class BaseServiceFixtures
    {
        protected BaseServiceFixtures()
        {
            GestureService = new Mock<IGestureService>();
            GestureService.Setup(x => x.SetBusy()).Verifiable();

            ObservableExtensions.GestureService = GestureService.Object;
        }

        public Mock<IGestureService> GestureService { get; }
    }
}