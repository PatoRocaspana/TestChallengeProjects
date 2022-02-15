
namespace TestChallengeProjects.BookingHelperProject.Repository
{
    public interface IBookingRepository
    {
        IQueryable<Booking> GetActiveBookings(Booking booking);
    }
}