using System;
using System.Threading.Tasks;

namespace MyApp.Models
{
    public class Loader
    {
        public event Action<BicyclePart>? PartLoaded;

        private readonly Random _random = new();

        public async Task LoadPartsAsync(int count)
        {
            for (int i = 0; i < count; i++)
            {
                await Task.Delay(500);
                
                var partType = (PartType)_random.Next(3);
                var part = new BicyclePart(partType);
                
                PartLoaded?.Invoke(part);
            }
        }
    }
}