namespace TestChallengeProjects.HouseKeeperHelperProject
{
    public interface IEmailSender
    {
        void EmailFile(string emailAddress, string emailBody, string filename, string subject);
    }
}
