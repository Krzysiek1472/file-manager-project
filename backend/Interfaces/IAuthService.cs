﻿using backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<bool> Register(UserViewModel userData);

        Task<Guid> Login(string email, string password);
        Task<bool> Logout(Guid token);
        Task<UserViewModel> GetCurrentUser(Guid token);
    }
}