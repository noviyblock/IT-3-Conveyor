using System;

namespace IT_TASK3.Models
{
    public class Detail
    {
        public Guid DetailId { get; } = Guid.NewGuid();
        public DateTime ProductionTime { get; } = DateTime.Now;
    }
}