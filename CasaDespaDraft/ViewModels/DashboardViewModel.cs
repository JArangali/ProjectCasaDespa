﻿using CasaDespaDraft.Models;

namespace CasaDespaDraft.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<Booking> Requested { get; set; }
        public IEnumerable<Booking> Accepted { get; set; }
        public IEnumerable<Booking> Archive { get; set; }
    }
}
