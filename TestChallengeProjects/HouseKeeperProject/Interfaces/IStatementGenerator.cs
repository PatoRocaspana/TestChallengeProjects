
namespace TestChallengeProjects.HouseKeeperHelperProject
{
    public interface IStatementGenerator
    {
        string SaveStatement(int housekeeperOid, string housekeeperName, DateTime statementDate);
    }
}