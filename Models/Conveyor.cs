using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;

namespace MyApp.Models
{
    public class Conveyor
    {
        public event Action<BicyclePart>? PartAdded;
        public event Action<BicyclePart>? PartRemoved;
        public event Action? Broken;
        public event Action? Fixed;
        public event Action? MaterialsEmpty;
        public event Action<Bicycle>? BicycleAssembled;

        private readonly List<BicyclePart> _parts = new();
        private readonly Random _random = new();
        private bool _isRunning;
        private bool _isBroken;
        private int _wheelCount;
        private int _frameCount;
        private int _handlebarCount;
        private readonly object _lock = new();

        public IReadOnlyList<BicyclePart> Parts => _parts;
        public bool IsBroken => _isBroken;
        public bool IsRunning => _isRunning;
        public Rect Bounds { get; set; }
        public Rect CollectionBox { get; set; }

        public void Start()
        {
            if (_isRunning) return;
            
            _isRunning = true;
            Task.Run(() => RunConveyor());
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private async Task RunConveyor()
        {
            while (_isRunning)
            {
                if (_isBroken)
                {
                    await Task.Delay(1000);
                    continue;
                }

                if (_random.Next(100) == 0)
                {
                    _isBroken = true;
                    Broken?.Invoke();
                    continue;
                }

                if (_parts.Count == 0)
                {
                    MaterialsEmpty?.Invoke();
                    await Task.Delay(1000);
                    continue;
                }

                lock (_lock)
                {
                    foreach (var part in _parts)
                    {
                        var angle = Math.Atan2(part.Position.Y - Bounds.Center.Y, part.Position.X - Bounds.Center.X);
                        angle += 0.05;
                        
                        var radius = Math.Min(Bounds.Width, Bounds.Height) / 2 * 0.8;
                        part.Position = new Point(
                            Bounds.Center.X + radius * Math.Cos(angle),
                            Bounds.Center.Y + radius * Math.Sin(angle));
                        
                        if (CollectionBox.Contains(part.Position))
                        {
                            TryCollectPart(part);
                        }
                    }
                }

                await Task.Delay(50);
            }
        }

        private void TryCollectPart(BicyclePart part)
        {
            bool shouldRemove = false;

            switch (part.Type)
            {
                case PartType.Wheel when _wheelCount < 2:
                    _wheelCount++;
                    shouldRemove = true;
                    break;
                case PartType.Frame when _frameCount == 0:
                    _frameCount++;
                    shouldRemove = true;
                    break;
                case PartType.Handlebar when _handlebarCount == 0:
                    _handlebarCount++;
                    shouldRemove = true;
                    break;
            }

            if (shouldRemove)
            {
                RemovePart(part);
            }

            if (_wheelCount == 2 && _frameCount == 1 && _handlebarCount == 1)
            {
                var bicycle = new Bicycle();
                bicycle.AddPart(new BicyclePart(PartType.Wheel));
                bicycle.AddPart(new BicyclePart(PartType.Wheel));
                bicycle.AddPart(new BicyclePart(PartType.Frame));
                bicycle.AddPart(new BicyclePart(PartType.Handlebar));
                
                BicycleAssembled?.Invoke(bicycle);
                _wheelCount = 0;
                _frameCount = 0;
                _handlebarCount = 0;
            }
        }

        public void AddPart(BicyclePart part)
        {
            lock (_lock)
            {
                _parts.Add(part);
                PartAdded?.Invoke(part);
            }
        }

        public void RemovePart(BicyclePart part)
        {
            lock (_lock)
            {
                if (_parts.Remove(part))
                {
                    PartRemoved?.Invoke(part);
                }
            }
        }

        public void Fix()
        {
            _isBroken = false;
            Fixed?.Invoke();
        }
    }
}