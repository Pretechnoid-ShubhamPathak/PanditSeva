using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Enums
{
    public enum UserRole
    {
        User,
        Priest,
        Admin
    }

    public enum BookingStatus
    {
        Pending,
        Accepted,
        Paid,
        Completed,
        Cancelled
    }
    
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed
    }
}
