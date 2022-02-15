using Moq;
using System.Collections.Generic;
using System.Linq;
using TestChallengeProjects;
using TestChallengeProjects.BookingHelperProject.Repository;
using Xunit;

namespace TestChallengeProjectsTests
{
    public class BookingHelperTests
    {
        private readonly Booking _existingBooking;
        private readonly List<Booking> _activeBookings;
        private readonly Mock<IBookingRepository> _mockBookingRepository;

        public BookingHelperTests()
        {
            _existingBooking = GetNewBooking();
            _activeBookings = new List<Booking>() { _existingBooking };
            _mockBookingRepository = new Mock<IBookingRepository>();
        }

        [Fact]
        public void OverlappingBookingsExist_CurrentBookingStatusIsCancelled_ReturnEmptyString()
        {
            //Arrange
            var currentBooking = new Booking()
            {
                Id = 2,
                ArrivalDate = _existingBooking.ArrivalDate,
                DepartureDate = _existingBooking.DepartureDate,
                Status = "Cancelled",
            };

            //Act
            var result = BookingHelper.OverlappingBookingsExist(currentBooking, _mockBookingRepository.Object);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(-6)]
        public void OverlappingBookingsExist_BookingArrivalAndDepartureDatesAreBothBeforeOrAfterThanBookingDates_ReturnEmptyString(int days)
        {
            //Arrange
            var currentBooking = new Booking()
            {
                Id = 2,
                ArrivalDate = _existingBooking.ArrivalDate.AddDays(days),
                DepartureDate = _existingBooking.DepartureDate.AddDays(days),
            };

            _mockBookingRepository.Setup(m => m.GetActiveBookings(currentBooking)).Returns(_activeBookings.AsQueryable);

            //Act
            var result = BookingHelper.OverlappingBookingsExist(currentBooking, _mockBookingRepository.Object);

            //Assert
            Assert.NotNull(result);
            _mockBookingRepository.Verify(m => m.GetActiveBookings(currentBooking), Times.Once);
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 1)]
        public void OverlappingBookingsExist_WhenOverlappingHapends_ReturnOverlappingBookingReference(int moveArrivalDays, int moveDepartureDays)
        {
            //Arrange
            var currentBooking = new Booking()
            {
                Id = 2,
                ArrivalDate = _existingBooking.ArrivalDate.AddDays(moveArrivalDays),
                DepartureDate = _existingBooking.DepartureDate.AddDays(moveDepartureDays),
            };

            _mockBookingRepository.Setup(m => m.GetActiveBookings(currentBooking)).Returns(_activeBookings.AsQueryable);

            //Act
            var result = BookingHelper.OverlappingBookingsExist(currentBooking, _mockBookingRepository.Object);

            //Assert
            Assert.NotNull(result);
            _mockBookingRepository.Verify(f => f.GetActiveBookings(currentBooking), Times.Once);
            Assert.Equal(_existingBooking.Reference, result);
        }

        private Booking GetNewBooking()
        {
            var existingBooking = new Booking()
            {
                Id = 1,
                ArrivalDate = new(2022, 2, 10, 14, 00, 00),
                DepartureDate = new(2022, 2, 15, 10, 00, 00),
                Reference = "Reference"
            };

            return existingBooking;
        }
    }
}