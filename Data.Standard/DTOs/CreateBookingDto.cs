using Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Standard.DTOs
{
    public class CreateBookingDto
    {
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int PriestProfileId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
    }

    public class UpdateBookingStatusDto
    {
        public BookingStatus Status { get; set; }
    }
}
