﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_bot.Models
{
    internal class UserResponse
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public DateTime DateTime { get; set; } 
    }
}
