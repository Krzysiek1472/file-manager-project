﻿using backend.Helpers;
using backend.Interfaces;
using backend.Models;
using backend.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly FileManagerDbContext _context;
        public AuthService(FileManagerDbContext context)
        {
            _context = context;
        }


        public async Task<UserViewModel> GetCurrentUser(Guid token)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Token == token && x.IsActive);
            if (user != null)
            {
                return UserConverter.ConvertDbModelToViewModel(user);
            }
            else
            {
                throw new NullReferenceException();
            }

        }


        public async Task<UserViewModel> Login(string email, string password)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower() && x.Password == password && x.IsActive);
            if (user != null)
            {
                if (user.IsAccountActivated == false)
                {
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("message", "ACCOUNT_NOT_ACTIVATED");
                    throw ex;
                }

                if (user.SystemAccess == false)
                {
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("message", "ACCOUNT_BLOCKED");
                    throw ex;
                }

                user.IsLoggedIn = true;

                HistoryModel historyRow = new HistoryModel()
                {
                    Description = "USER_LOGGED_INTO_SYSTEM",
                    UserData = user.Name + " " + user.Surname
                };

                _context.Users.Update(user);
                _context.History.Add(historyRow);

                await _context.SaveChangesAsync();
                return UserConverter.ConvertDbModelToViewModel(user);
            }
            else
            {
                InvalidOperationException ex = new InvalidOperationException();
                ex.Data.Add("message", "LOGIN_DATA_INCORRECT");
                throw ex;
            }
        }

        public async Task<bool> Register(UserViewModel userData, string emailActivationUrl, string mailSubject, string mailContent)
        {
            bool isAlreadyInSystem = await _context.Users.AnyAsync(x => x.Email.ToLower() == userData.email.ToLower() && x.IsActive);
            if (isAlreadyInSystem)
            {
                InvalidOperationException ex = new InvalidOperationException();
                ex.Data.Add("message", "ALREADY_IN_SYSTEM");
                throw ex;
            }

            if (userData.password != userData.passwordRepeat)
            {
                InvalidOperationException ex = new InvalidOperationException();
                ex.Data.Add("message", "PASSWORS_EQUAL_ERROR");
                throw ex;
            }

            UserModel user = new UserModel()
            {
                Email = userData.email,
                Name = userData.name,
                Surname = userData.surname,
                Password = userData.password,
                Role = userData.role
            };

            HistoryModel historyRow = new HistoryModel()
            {
                Description = "USER_REGISTERED_INTO_SYSTEM",
                UserData = user.Name + " " + user.Surname
            };

            _context.Users.Add(user);
            _context.History.Add(historyRow);
            await _context.SaveChangesAsync();

            string mailBody = mailContent +  (emailActivationUrl + user.Token) ;
            SendEmail(user, mailSubject, mailBody);
            return true;
        }

        private void SendEmail(UserModel user, string subject, string mailBody)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("File Manager Team", "filemanager321@gmail.com"));
            message.To.Add(new MailboxAddress("User", user.Email));

            message.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = mailBody;
            message.Body = bodyBuilder.ToMessageBody();

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com");
                smtpClient.Authenticate("filemanager321@gmail.com", "File!manager123");
                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }
        }

        public async Task<bool> Logout(Guid token)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Token == token && x.IsActive);
            if (user != null)
            {
                user.IsLoggedIn = false;

                HistoryModel historyRow = new HistoryModel()
                {
                    Description = "USER_LOGGED_OUT",
                    UserData = user.Name + " " + user.Surname
                };

                _context.Users.Update(user);
                _context.History.Add(historyRow);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                NullReferenceException ex = new NullReferenceException();
                ex.Data.Add("message", "USER_NOT_FOUND");
                throw ex;
            }

        }

        public async Task ActivateAccount(Guid token)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Token == token && x.IsActive);
            if (user != null)
            {
                user.IsAccountActivated = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                NullReferenceException ex = new NullReferenceException();
                ex.Data.Add("message", "USER_NOT_FOUND");
                throw ex;
            }

        }

        public async Task SetAccountUnactive(Guid token, string password)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Token == token && x.IsActive);

            if (user != null)
            {
                if (user.Password != password)
                {
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("message", "PASSWORD_INCORRECT");
                    throw ex;
                }
                user.IsActive = false;

                HistoryModel historyRow = new HistoryModel()
                {
                    Description = "USER_DELETED_ACCOUNT",
                    UserData = user.Name + " " + user.Surname
                };

                _context.Users.Update(user);
                _context.History.Add(historyRow);
                await _context.SaveChangesAsync();
            }
            else
            {
                NullReferenceException ex = new NullReferenceException();
                ex.Data.Add("message", "USER_NOT_FOUND");
                throw ex;
            }
        }

        public async Task ChangePassword(Guid token, PasswordChangeData passwordChangeData)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Token == token && x.IsActive);

            if (user != null)
            {
                if (passwordChangeData.newPassword != passwordChangeData.newPasswordRepeat)
                {
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("message", "PASSWORS_EQUAL_ERROR");
                    throw ex;
                }

                if (user.Password != passwordChangeData.oldPassword)
                {
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("message", "PASSWORD_INCORRECT");
                    throw ex;
                }

                user.Password = passwordChangeData.newPassword;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                NullReferenceException ex = new NullReferenceException();
                ex.Data.Add("message", "USER_NOT_FOUND");
                throw ex;
            }
        }

        public async Task RemindPassword(string email, string subject, string mailContent)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.IsActive);

            if (user != null)
            {
                string newPassword = Guid.NewGuid().ToString().Substring(0, 8);
                user.Password = newPassword;

                _context.Update(user);
                await _context.SaveChangesAsync();

                string mailBody = mailContent + newPassword;
                SendEmail(user, subject, mailBody);
            }
            else
            {
                NullReferenceException ex = new NullReferenceException();
                ex.Data.Add("message", "USER_NOT_FOUND_BY_EMAIL");
                throw ex;
            }
        }

        public async Task<List<UserViewModel>> GetAllUsers()
        {
            List<UserModel> users = await _context.Users.OrderBy(x => x.Role).Where(x => x.IsActive).ToListAsync();
            return UserConverter.ConvertDbListToViewList(users);
        }

        public async Task DisableUsersSystemAccess(int[] ids)
        {
            foreach (int id in ids)
            {
                UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
                if (user != null)
                {
                    user.SystemAccess = false;
                    _context.Users.Update(user);
                }
                else
                {
                    NullReferenceException ex = new NullReferenceException();
                    ex.Data.Add("message", "USER_NOT_FOUND_BY_ID");
                    ex.Data.Add("id", id);
                    throw ex;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DisableUsersSystemEditing(int[] ids)
        {
            foreach (int id in ids)
            {
                UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
                if (user != null)
                {
                    user.SystemEditingEnabled = false;
                    _context.Users.Update(user);
                }
                else
                {
                    NullReferenceException ex = new NullReferenceException();
                    ex.Data.Add("message", "USER_NOT_FOUND_BY_ID");
                    ex.Data.Add("id", id);
                    throw ex;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UnlockUser(int id)
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            if (user != null)
            {
                user.SystemAccess = true;
                user.SystemEditingEnabled = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                NullReferenceException ex = new NullReferenceException();
                ex.Data.Add("message", "USER_NOT_FOUND_BY_ID");
                ex.Data.Add("id", id);
                throw ex;
            }
        }

        public async Task<List<UserViewModel>> SearchForUsers(string searchString)
        {
            string searchStringNormalized = searchString.Trim().ToLower().Replace(" ", "");
            List<UserModel> users = await _context.Users
                .Where(x => x.IsActive && (x.Name + x.Surname + x.Email).ToLower().Contains(searchStringNormalized))
                .ToListAsync();

            return UserConverter.ConvertDbListToViewList(users);
        }

        public async Task<List<UserViewModel>> GetBlockedUsers()
        {
            List<UserModel> users = await _context.Users.Where(x => x.IsActive && (x.SystemAccess == false || x.SystemEditingEnabled == false)).ToListAsync();
            return UserConverter.ConvertDbListToViewList(users);
        }
    }
}
