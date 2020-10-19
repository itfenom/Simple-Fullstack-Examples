using System;
using System.Web;

namespace Playground.Mvc.Helpers
{
    [Serializable]
    public class SessionSingleton
    {
        #region Singleton

        // ReSharper disable once InconsistentNaming
        private const string SESSION_SINGLETON_NAME = "Singleton_502E69E5-668B-E011-951F-00155DF26207";
        // ReSharper disable once RedundantDefaultMemberInitializer
        private int _cartItemCount = 0;

        private SessionSingleton()
        {
        }

        public static SessionSingleton Current
        {
            get
            {
                if (HttpContext.Current.Session[SESSION_SINGLETON_NAME] == null)
                {
                    HttpContext.Current.Session[SESSION_SINGLETON_NAME] = new SessionSingleton();
                }

                return HttpContext.Current.Session[SESSION_SINGLETON_NAME] as SessionSingleton;
            }
        }

        #endregion Singleton

        public int Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }

        public int CartItemCount
        {
            get => _cartItemCount;
            set => _cartItemCount = value;
        }
    }
}