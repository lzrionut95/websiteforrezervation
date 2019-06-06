using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteForReservation.Classes
{
    public class UserTime
    {
      public DateTime dateOfReservation { get; set; }

      public DateTime startHour { get; set; }

        public DateTime endHour { get; set; }
        public User user { get; set; }

    }
}