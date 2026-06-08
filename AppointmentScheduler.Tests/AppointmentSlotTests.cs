using AppointmentScheduler.AppointmentService.Entities;
using Xunit;

namespace AppointmentScheduler.Tests;

public class AppointmentSlotTests
{
    [Fact]
    public void BookOne_IncrementsBookedCount()
    {
        var slot = new AppointmentSlot
        {
            Capacity = 2,
            BookedCount = 0
        };

        slot.BookOne();

        Assert.Equal(1, slot.BookedCount);
        Assert.False(slot.IsBooked);
    }

    [Fact]
    public void BookOne_ThrowsWhenSlotFullyBooked()
    {
        var slot = new AppointmentSlot
        {
            Capacity = 1,
            BookedCount = 1
        };

        var exception = Assert.Throws<InvalidOperationException>(() => slot.BookOne());
        Assert.Equal("The appointment slot is fully booked.", exception.Message);
    }

    [Fact]
    public void IsBooked_IsTrueWhenBookedCountReachesCapacity()
    {
        var slot = new AppointmentSlot
        {
            Capacity = 3,
            BookedCount = 3
        };

        Assert.True(slot.IsBooked);
    }
}
