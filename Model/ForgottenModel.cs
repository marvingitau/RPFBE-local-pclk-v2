﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class ForgottenModel
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
