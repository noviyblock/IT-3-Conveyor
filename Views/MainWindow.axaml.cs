using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using MyApp.Models;
using MyApp.ViewModels;

namespace MyApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _animationTimer;
        private MainViewModel ViewModel => (MainViewModel)DataContext!;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _animationTimer.Tick += OnAnimationTick;
            _animationTimer.Start();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            
            var conveyorCanvas = this.FindControl<Canvas>("ConveyorCanvas");
            var collectionBox = this.FindControl<Rectangle>("CollectionBox");
            var collectionInfo = this.FindControl<StackPanel>("CollectionInfo");
            
            collectionBox.SetValue(Canvas.LeftProperty, conveyorCanvas.Bounds.Width / 2 - collectionBox.Width / 2);
            collectionBox.SetValue(Canvas.TopProperty, conveyorCanvas.Bounds.Height / 2 - collectionBox.Height / 2);
            
            collectionInfo.SetValue(Canvas.LeftProperty, conveyorCanvas.Bounds.Width / 2 - collectionInfo.Bounds.Width / 2);
            collectionInfo.SetValue(Canvas.TopProperty, conveyorCanvas.Bounds.Height / 2 + collectionBox.Height + 10);
            
            var conveyorBounds = new Rect(0, 0, conveyorCanvas.Bounds.Width, conveyorCanvas.Bounds.Height);
            var collectionBounds = new Rect(
                (double)collectionBox.GetValue(Canvas.LeftProperty),
                (double)collectionBox.GetValue(Canvas.TopProperty),
                collectionBox.Width,
                collectionBox.Height);
            
            ViewModel.SetConveyorBounds(conveyorBounds, collectionBounds);
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            var conveyorCanvas = this.FindControl<Canvas>("ConveyorCanvas");
            conveyorCanvas.Children.Clear();
            
            var collectionBox = this.FindControl<Rectangle>("CollectionBox");
            conveyorCanvas.Children.Add(collectionBox);
            
            foreach (var part in ViewModel.Parts)
            {
                var border = new Border
                {
                    Width = 30,
                    Height = 30,
                    CornerRadius = new CornerRadius(15),
                    Background = part.Type switch
                    {
                        PartType.Wheel => Brushes.Black,
                        PartType.Frame => Brushes.Red,
                        PartType.Handlebar => Brushes.Blue,
                        _ => Brushes.Gray
                    }
                };
                
                Canvas.SetLeft(border, part.Position.X - 15);
                Canvas.SetTop(border, part.Position.Y - 15);
                conveyorCanvas.Children.Add(border);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}