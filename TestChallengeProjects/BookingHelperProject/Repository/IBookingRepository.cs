
namespace TestChallengeProjects.BookingHelperProject.Repository
{
    public interface IBookingRepository
    {
        IQueryable<Booking> GetActiveBookings(int? excludedBookingId = null);
    }
}
