using System.Collections.Generic;
using System.Linq;

namespace MyApp.Models
{
    public class Bicycle
    {
        public int Id { get; } = System.Environment.TickCount;
        public List<BicyclePart> Parts { get; } = new();

        public bool IsComplete => 
            Parts.Count(p => p.Type == PartType.Wheel) == 2 &&
            Parts.Count(p => p.Type == PartType.Frame) == 1 &&
            Parts.Count(p => p.Type == PartType.Handlebar) == 1;

        public void AddPart(BicyclePart part)
        {
            if (!IsComplete)
            {
                Parts.Add(part);
            }
        }

        public override string ToString()
        {
            return $"Bicycle #{Id} (Wheels: {Parts.Count(p => p.Type == PartType.Wheel)}/2, " +
                   $"Frame: {Parts.Count(p => p.Type == PartType.Frame)}/1, " +
                   $"Handlebar: {Parts.Count(p => p.Type == PartType.Handlebar)}/1)";
        }
    }
}