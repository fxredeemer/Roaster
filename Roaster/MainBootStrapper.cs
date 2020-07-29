using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Roaster.ViewModels;

namespace Roaster
{
    internal class MainBootStrapper : BootstrapperBase
    {
        private IServiceProvider container;

        protected override void Configure()
        {
            var containerBuilder = new ServiceCollection();

            containerBuilder.AddSingleton<IEventAggregator, EventAggregator>();
            containerBuilder.AddSingleton<IWindowManager, WindowManager>();

            containerBuilder.AddTransient<IMainViewModel, MainViewModel>();

            container = containerBuilder.BuildServiceProvider();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
        }

        public MainBootStrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IMainViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            return container.GetService(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetServices(service);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            MessageBox.Show(e.Exception.Message, "An error as occurred", MessageBoxButton.OK);
        }
    }
}
