namespace Core
{
    public interface IMailCascadeNotifier
    {
        void Notify(string text, string responsibleMail);
    }
}