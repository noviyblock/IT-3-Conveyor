using Avalonia;

namespace MyApp.Models
{
    public enum PartType { Wheel, Frame, Handlebar }

    public class BicyclePart
    {
        public PartType Type { get; }
        public int Id { get; } = System.Environment.TickCount;
        public Point Position { get; set; }

        public BicyclePart(PartType type)
        {
            Type = type;
        }

        public override string ToString() => $"{Type} #{Id}";
    }
}