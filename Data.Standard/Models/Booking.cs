using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Data.Models.Enums;

namespace Data.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the priestId.
        /// </summary>
        /// <value>
        /// The priestId.
        /// </value>
        [ForeignKey("PriestProfile")]
        public int PriestId { get; set; }

        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        /// <value>
        /// The service identifier.
        /// </value>
        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Navigation
        public required ApplicationUser User { get; set; }
        public PriestProfile? PriestProfile { get; set; }
        public required Services Service { get; set; }
        public Payment? Payment { get; set; }
    }

}
