using Domain;

namespace Core
{
    public interface IAssignmentCascadeNotifier
    {
        void Notify(string text, Workplace responsible);
    }
}