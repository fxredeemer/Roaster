using Roaster.ViewModels;
using Stylet;
using StyletIoC;
using System.Windows;
using System.Windows.Threading;

namespace Roaster
{
    internal class MainBootStrapper : Bootstrapper<MainViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);

            builder.Bind<IMainViewModel>().To<MainViewModel>();
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(e);

            MessageBox.Show(e.Exception.Message, "An error as occurred", MessageBoxButton.OK);
        }
    }
}
