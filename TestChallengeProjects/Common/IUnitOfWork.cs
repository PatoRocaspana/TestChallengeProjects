
namespace TestChallengeProjects
{
    public interface IUnitOfWork
    {
        IQueryable<T> Query<T>();
    }
}