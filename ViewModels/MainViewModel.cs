using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp.Models;

namespace MyApp.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly Conveyor _conveyor;
        private readonly Loader _loader;
        private readonly Mechanic _mechanic;
        private readonly Random _random = new();

        [ObservableProperty]
        private int _wheelCount;

        [ObservableProperty]
        private int _frameCount;

        [ObservableProperty]
        private int _handlebarCount;

        [ObservableProperty]
        private int _assembledBicyclesCount;

        [ObservableProperty]
        private bool _isConveyorBroken;

        [ObservableProperty]
        private bool _isMechanicBusy;

        [ObservableProperty]
        private string _statusMessage = "Ready";

        public ObservableCollection<BicyclePart> Parts { get; } = new();
        public ObservableCollection<BicyclePart> AvailableParts { get; } = new();
        public ObservableCollection<Bicycle> AssembledBicycles { get; } = new();

        public MainViewModel()
        {
            _conveyor = new Conveyor();
            _loader = new Loader();
            _mechanic = new Mechanic();

            SetupEvents();
            InitializeAvailableParts();
        }

        private void SetupEvents()
        {
            _conveyor.PartAdded += part => Dispatcher.UIThread.Post(() => Parts.Add(part));
            _conveyor.PartRemoved += part => Dispatcher.UIThread.Post(() => Parts.Remove(part));
            _conveyor.Broken += () => Dispatcher.UIThread.Post(() =>
            {
                IsConveyorBroken = true;
                StatusMessage = "Conveyor broken! Waiting for mechanic...";
            });
            _conveyor.Fixed += () => Dispatcher.UIThread.Post(() =>
            {
                IsConveyorBroken = false;
                StatusMessage = "Conveyor fixed!";
            });
            _conveyor.MaterialsEmpty += () => Dispatcher.UIThread.Post(() =>
            {
                StatusMessage = "Materials empty! Loading more...";
                Task.Run(async () => await _loader.LoadPartsAsync(5));
            });
            _conveyor.BicycleAssembled += bicycle => Dispatcher.UIThread.Post(() =>
            {
                AssembledBicycles.Add(bicycle);
                AssembledBicyclesCount++;
                StatusMessage = $"Bicycle assembled! Total: {AssembledBicyclesCount}";
            });

            _loader.PartLoaded += part => Dispatcher.UIThread.Post(() =>
            {
                _conveyor.AddPart(part);
                StatusMessage = $"New {part.Type} loaded on conveyor";
            });

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(IsConveyorBroken) && IsConveyorBroken)
                {
                    _mechanic.FixConveyor();
                    IsMechanicBusy = true;
                }
            };

            var mechanicProperties = typeof(Mechanic).GetProperties();
            foreach (var prop in mechanicProperties)
            {
                if (prop.GetMethod != null && prop.GetMethod.IsPublic)
                {
                    var eventInfo = typeof(Mechanic).GetEvent("PropertyChanged");
                    if (eventInfo != null)
                    {
                        var addMethod = eventInfo.GetAddMethod();
                        var handler = new PropertyChangedEventHandler((s, e) =>
                        {
                            if (e.PropertyName == nameof(Mechanic.IsBusy))
                            {
                                Dispatcher.UIThread.Post(() => IsMechanicBusy = _mechanic.IsBusy);
                            }
                        });
                        
                        addMethod.Invoke(_mechanic, new object[] { handler });
                    }
                }
            }
        }

        private void InitializeAvailableParts()
        {
            for (int i = 0; i < 10; i++)
            {
                var partType = (PartType)_random.Next(3);
                AvailableParts.Add(new BicyclePart(partType));
            }
        }

        [RelayCommand]
        public void StartConveyor()
        {
            _conveyor.Start();
            StatusMessage = "Conveyor started";
        }

        [RelayCommand]
        public void StopConveyor()
        {
            _conveyor.Stop();
            StatusMessage = "Conveyor stopped";
        }

        [RelayCommand]
        public void AddRandomPart()
        {
            var partType = (PartType)_random.Next(3);
            var part = new BicyclePart(partType);
            _conveyor.AddPart(part);
            StatusMessage = $"Added {part.Type} to conveyor";
        }

        public void SetConveyorBounds(Rect bounds, Rect collectionBox)
        {
            _conveyor.Bounds = bounds;
            _conveyor.CollectionBox = collectionBox;
        }
    }
}