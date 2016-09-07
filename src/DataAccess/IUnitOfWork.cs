using Domain;

namespace DataAccess
{
    public interface IUnitOfWork
    {
        void Commit();
        IRepository<Assignment> Assignments { get; set; }
        IRepository<Workplace> Workplaces { get; set; }
    }
}