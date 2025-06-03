using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class Booking
    {
        [Key] // Indicates that BookingID is the primary key
        public int BookingID { get; set; }

        [Required(ErrorMessage = "Guest name is required.")]
        [StringLength(255, ErrorMessage = "Guest name cannot exceed 255 characters.")]
        public string GuestName { get; set; }
        public string Status { get; set; }


        [Required(ErrorMessage = "Check-in date is required.")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required.")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        // Optional: Add other properties as needed
        // Example:
        // public int RoomNumber { get; set; }
        // public decimal TotalPrice { get; set; }
        // public string Notes {get; set;}
    }
}
