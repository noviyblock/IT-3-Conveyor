namespace MyApp.Models
{
    public interface IMechanic
    {
        void FixConveyor();
        bool IsBusy { get; }
    }
}