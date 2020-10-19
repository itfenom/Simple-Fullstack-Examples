using Playground.Mvc.DAL;
using Playground.Mvc.Models;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace Playground.Mvc.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountRepository _repository;

        public AccountController()
        {
            _repository = new AccountRepository(new SeraphEntities());
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid Credentials. Try again.");
                return View(model);
            }

            if (_repository.IsUserExist(model.UserName))
            {
                var user = _repository.GetUser(model.UserName, model.Password);

                if (user != null)
                {
                    //populate static class here with credentials!
                    FormsAuthentication.RedirectFromLoginPage(model.UserName, model.RememberMe);

                    System.Web.HttpContext.Current.Session["ID"] = user.ID;
                    System.Web.HttpContext.Current.Session["USER_NAME"] = user.UserName;
                    System.Web.HttpContext.Current.Session["DISPLAY_NAME"] = user.DisplayName;
                    System.Web.HttpContext.Current.Session["ROLE_ID"] = user.RoleID;
                    System.Web.HttpContext.Current.Session["USER_ROLE"] = user.UserRole;

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1
                        && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//")
                        && !returnUrl.StartsWith("/\\"))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            //if we get this far, then credentials were not validated, so display error!
            ModelState.AddModelError("", "Invalid Credentials. Try again.");
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //check if user exist
                if (_repository.IsUserExist(model.UserName.Trim()))
                {
                    Danger($"<b>{model.UserName}</b> already exist. Choose a different user name.", true);
                    return View(model);
                }

                var newUser = new APP_USER
                {
                    USER_NAME = model.UserName,
                    DISPLAY_NAME = model.DisplayName,
                    USER_PASSWORD = model.ConfirmPassword,
                    ROLE_ID = 2
                };
                _repository.AddNewUser(newUser);

                Success($"<b>{model.UserName}</b> user created successfully. Please login.", true);

                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "Invalid/incomplete data provided. Try again.");
            return View(model);
        }

        [HttpGet]
        public ActionResult Manage()
        {
            if (System.Web.HttpContext.Current.Session["ID"] == null)
            {
                return RedirectToAction("LogOff");
            }
            var user = _repository.GetUserById(Convert.ToInt32(System.Web.HttpContext.Current.Session["ID"]));
            return View(user);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = _repository.GetUserById(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateUser = new APP_USER
                {
                    ID = model.ID,
                    USER_NAME = model.UserName,
                    DISPLAY_NAME = model.DisplayName,
                    USER_PASSWORD = model.ConfirmPassword,
                    ROLE_ID = model.RoleID
                };
                _repository.UpdateUser(updateUser);

                Success($"<b>{model.UserName}</b> updated successfully.", true);

                return RedirectToAction("Manage");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Users = "kmubarak")]
        public ActionResult ManageAdmin()
        {
            var allUsers = _repository.GetAllUsers();
            return View(allUsers);
        }

        [Authorize(Users = "kmubarak")]
        public ActionResult DeleteUser(int id)
        {
            var user = _repository.GetUserById(id);

            if (user.UserRole.ToUpper() == "ADMIN")
            {
                Danger("<b>You cannot delete an Admin user!</b>", true);
            }
            else
            {
                _repository.DeleteUser(id);
                Success($"User <b>'{user.DisplayName}'</b> deleted successfully.", true);
            }

            return RedirectToAction("ManageAdmin");
        }
    }
}