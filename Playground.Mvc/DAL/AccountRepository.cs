using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playground.Mvc.DAL
{
    public interface IAccountRepository : IDisposable
    {
        IEnumerable<APP_USER_ROLE> GetAllRoles();

        IEnumerable<UserViewModel> GetAllUsers();

        bool IsUserExist(string userName);

        UserViewModel GetUser(string userName, string password);

        bool AddNewUser(APP_USER user);

        bool UpdateUser(APP_USER user);

        bool DeleteUser(int userId);

        int GetRoleIdByRoleName(string roleName);

        UserViewModel GetUserById(int id);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly SeraphEntities _context;
        private bool _disposed;

        public AccountRepository(SeraphEntities context)
        {
            _context = context;
        }

        public IEnumerable<APP_USER_ROLE> GetAllRoles()
        {
            return _context.APP_USER_ROLE.ToList();
        }

        public IEnumerable<UserViewModel> GetAllUsers()
        {
            var allUsers = from u in _context.APP_USER
                            join r in _context.APP_USER_ROLE
                            on u.ROLE_ID equals r.ROLE_ID
                            select new UserViewModel
                            {
                                ID = u.ID,
                                UserName = u.USER_NAME,
                                DisplayName = u.DISPLAY_NAME,
                                RoleID = (int)u.ROLE_ID,
                                UserRole = r.ROLE_NAME
                            };

            return allUsers.ToList().OrderBy(u => u.UserRole);
        }

        public UserViewModel GetUser(string userName, string password)
        {
            var user = from u in _context.APP_USER
                        join r in _context.APP_USER_ROLE
                        on u.ROLE_ID equals r.ROLE_ID
                        where u.USER_NAME == userName && u.USER_PASSWORD == password && u.ROLE_ID == r.ROLE_ID
                        select new UserViewModel
                        {
                            ID = u.ID,
                            UserName = u.USER_NAME,
                            Password = u.USER_PASSWORD,
                            DisplayName = u.DISPLAY_NAME,
                            RoleID = (int)u.ROLE_ID,
                            UserRole = r.ROLE_NAME
                        };

            return user.FirstOrDefault();
        }

        public bool IsUserExist(string userName)
        {
            bool retVal = false;

            var user = (from u in _context.APP_USER
                         where u.USER_NAME.Trim() == userName.Trim()
                         select u.USER_NAME).FirstOrDefault();

            if (user != null)
            {
                if (user == userName)
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        public bool AddNewUser(APP_USER user)
        {
            _context.APP_USER.Add(user);
            _context.SaveChanges();

            return true;
        }

        public bool UpdateUser(APP_USER user)
        {
            bool retVal = false;
            var userToUpdate = _context.APP_USER.Find(user.ID);

            if (userToUpdate != null)
            {
                _context.Entry(userToUpdate).CurrentValues.SetValues(user);
                _context.SaveChanges();
                retVal = true;
            }

            return retVal;
        }

        public bool DeleteUser(int userId)
        {
            var userToDelete = (from u in _context.APP_USER
                         where u.ID == userId
                         select u).FirstOrDefault();

            if (userToDelete != null)
            {
                _context.APP_USER.Remove(userToDelete);
                _context.SaveChanges();
            }

            return true;
        }

        public int GetRoleIdByRoleName(string roleName)
        {
            var role = (from r in _context.APP_USER_ROLE
                      where r.ROLE_NAME == roleName
                      select r.ROLE_ID).FirstOrDefault();

            return Convert.ToInt32(role);
        }

        public UserViewModel GetUserById(int id)
        {
            var user = from u in _context.APP_USER
                        join r in _context.APP_USER_ROLE
                        on u.ROLE_ID equals r.ROLE_ID
                        where u.ID == id && u.ROLE_ID == r.ROLE_ID
                        select new UserViewModel
                        {
                            ID = u.ID,
                            UserName = u.USER_NAME,
                            Password = u.USER_PASSWORD,
                            DisplayName = u.DISPLAY_NAME,
                            RoleID = (int)u.ROLE_ID,
                            UserRole = r.ROLE_NAME
                        };

            return user.FirstOrDefault();
        }

        #region Disposing

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Disposing
    }
}