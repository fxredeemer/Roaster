using Caliburn.Micro;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Roaster.ViewModels
{
    internal interface IMainViewModel
    {
        int TaskCount { get; set; }
        string ButtonText { get; }
        SeriesCollection CPULoad { get; }
        AxesCollection YAxis { get; }
        AxesCollection XAxis { get; }

        int TotalThreadCount { get; }
        void StartStop();
    }

    internal class MainViewModel : PropertyChangedBase, IMainViewModel
    {
        private readonly List<PerformanceCounter> cpuCounters = new List<PerformanceCounter>();
        private bool isRunning;
        private int taskCount = 1;
        private bool roast;

        public SeriesCollection CPULoad { get; set; }
        public AxesCollection YAxis { get; set; }
        public AxesCollection XAxis { get; set; }

        public MainViewModel()
        {
            InitializeCPUGraph();

            InitializeCPUCounters();

            var dispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (_, __) => Tick(), Application.Current.Dispatcher);

            dispatcherTimer.Start();
        }

        private void InitializeCPUCounters()
        {
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                cpuCounters.Add(new PerformanceCounter("Processor Information", "% Processor Time", $"0,{i}"));
            }
        }

        private void InitializeCPUGraph()
        {
            CPULoad = new SeriesCollection();
            YAxis = new AxesCollection()
            {
                new Axis()
                {
                    MinValue = 0,
                    MaxValue = 100,
                }
            };

            XAxis = new AxesCollection()
            {
                new Axis()
                {
                    Labels = Array.Empty<string>(),
                    Title = "Time"
                }
            };

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var series = new LineSeries
                {
                    AreaLimit = -10,
                    Values = new ChartValues<ObservableValue>
                    {
                        new ObservableValue(),
                        new ObservableValue(),
                        new ObservableValue(),
                        new ObservableValue(),
                        new ObservableValue(),
                        new ObservableValue(),
                        new ObservableValue(),
                        new ObservableValue(),
                        new ObservableValue(),
                        new ObservableValue()
                    },
                };

                CPULoad.Add(series);
            }
        }

        private void Tick()
        {
            for (int i = 0; i < cpuCounters.Count; i++)
            {
                var cpuLoad = cpuCounters[i].NextValue();
                CPULoad[i].Values.Add(new ObservableValue(cpuLoad));
                CPULoad[i].Values.RemoveAt(0);
            }
        }

        public int TaskCount
        {
            get => taskCount;
            set
            {
                taskCount = value;
                NotifyOfPropertyChange();
            }
        }

        private bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                NotifyOfPropertyChange(nameof(ButtonText));
            }
        }

        public string ButtonText => IsRunning ? "Stop Roasting" : "Roast";

        public int TotalThreadCount => Environment.ProcessorCount;

        public void StartStop()
        {
            if (!IsRunning)
            {
                roast = true;

                Task.Run(() =>
                {
                    Parallel.For(0, TaskCount, (_) =>
                    {
                        while (roast) { }
                    });
                });
            }
            else
            {
                roast = false;
            }

            IsRunning = !IsRunning;
        }
    }
}
