using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IT_TASK3.Models;

namespace IT_TASK4.Models
{
    public class Conveyor
    {
        private readonly Random _random = new Random();
        private int _materials = 10;
        private bool _isBroken = false;
        private bool _isRunning = false;
        private CancellationTokenSource _cts;

        public event EventHandler<Detail> DetailProduced;
        public event EventHandler<string> ConveyorStatusChanged;
        public event EventHandler MaterialsEmpty;
        public event EventHandler ConveyorBroken;

        public int Materials => _materials;
        public bool IsBroken => _isBroken;
        public bool IsRunning => _isRunning;

        public void Start()
        {
            if (_isRunning) return;
            
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => ProductionCycle(_cts.Token));
            OnConveyorStatusChanged("Conveyor: Started");
        }

        public void Stop()
        {
            _isRunning = false;
            _cts?.Cancel();
            OnConveyorStatusChanged("Conveyor: Stopped");
        }

        public void AddMaterials(int quantity)
        {
            _materials += quantity;
            OnConveyorStatusChanged($"Conveyor: Added {quantity} materials. Total: {_materials}");
        }

        private async Task ProductionCycle(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested)
            {
                if (_isBroken)
                {
                    OnConveyorStatusChanged("Conveyor: Waiting for repair...");
                    await Task.Delay(1000);
                    continue;
                }

                if (_materials <= 0)
                {
                    OnMaterialsEmpty();
                    OnConveyorStatusChanged("Conveyor: Waiting for materials...");
                    await Task.Delay(1000);
                    continue;
                }

                try
                {
                    await ProduceDetail(token);
                    await Task.Delay(500, token); // Задержка между производством деталей
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        private async Task ProduceDetail(CancellationToken token)
        {
            _materials--;
            OnConveyorStatusChanged($"Conveyor: Producing detail. Materials left: {_materials}");
            
            // Имитация времени производства
            await Task.Delay(800, token);

            // 10% вероятность поломки
            if (_random.Next(0, 100) < 10)
            {
                _isBroken = true;
                OnConveyorBroken();
                return;
            }

            var detail = new Detail();
            OnDetailProduced(detail);
        }

        public void FixConveyor()
        {
            _isBroken = false;
            OnConveyorStatusChanged("Conveyor: Fixed and ready to work");
        }

        protected virtual void OnDetailProduced(Detail detail)
        {
            DetailProduced?.Invoke(this, detail);
        }

        protected virtual void OnConveyorStatusChanged(string status)
        {
            ConveyorStatusChanged?.Invoke(this, status);
        }

        protected virtual void OnMaterialsEmpty()
        {
            MaterialsEmpty?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnConveyorBroken()
        {
            ConveyorBroken?.Invoke(this, EventArgs.Empty);
        }
    }
}