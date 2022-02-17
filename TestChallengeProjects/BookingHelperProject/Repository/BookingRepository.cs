namespace TestChallengeProjects.BookingHelperProject.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Booking> GetActiveBookings(int? excludedBookingId = null)
        {
            var bookings =
                    _unitOfWork.Query<Booking>()
                               .Where(b => b.Id != excludedBookingId && b.Status != "Cancelled");

            if (excludedBookingId.HasValue)
                bookings = bookings.Where(b => b.Id == excludedBookingId.Value);

            return bookings;
        }
    }
}
