﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KQStore.Models
{
    public class CheckoutViewModel
    {
        public KQStore.Models.User User { get; set; }
        public IEnumerable<KQStore.Models.CartItem> CartItems { get; set; }
    }
}