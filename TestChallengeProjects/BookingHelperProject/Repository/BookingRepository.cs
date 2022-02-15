namespace TestChallengeProjects.BookingHelperProject.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IQueryable<Booking> GetActiveBookings(Booking booking)
        {
            var bookings =
                    _unitOfWork
                    .Query<Booking>()
                    .Where(b => b.Id != booking.Id && b.Status != "Cancelled");

            return bookings;
        }
    }
}
