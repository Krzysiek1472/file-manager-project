﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.ViewModels
{
    public class UserViewModel
    {
        public int id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string surname { get; set; }

        public string password { get; set; }

        public bool isLoggedIn { get; set; }

        public Guid token { get; set; }
        public string role { get; set; }
        public bool systemAccess { get; set; }
        public bool systemEditingEnabled { get; set; }
#nullable enable
        public string? passwordRepeat { get; set; }

    }
}
