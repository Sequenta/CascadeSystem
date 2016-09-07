namespace Core
{
    public interface ISmsCascadeNotifier
    {
        void Notify(string text, string responsiblePhone);
    }
}