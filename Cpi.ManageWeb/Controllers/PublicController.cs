using Cpi.Application.BusinessObjects;
using Cpi.Application.DataModels;
using Cpi.Application.Helpers;
using Cpi.ManageWeb.Controllers.Base;
using Cpi.ManageWeb.Models;
using System;
using System.Web.Mvc;

namespace Cpi.ManageWeb.Controllers
{
    public class PublicController : BaseController
    {
        private UserBo UserBo;
        public PublicController(UserBo userBo)
        {
            UserBo = userBo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult Login(Login login)
        {
            UserDm trackedUser = UserBo.GetByUsername(login.Username);
            bool isLoggingIn = false;

            // no such username
            if (trackedUser == null)
            {
                ModelState.AddModelError("LoginError", "The specified username or password is incorrect.");
                return JsonModelState(ModelState);
            }

            // check if the user has temporary password: if so we redirect back to update password
            if (!string.IsNullOrEmpty(trackedUser.TempPassword))
            {
                if (login.Password == trackedUser.TempPassword)
                {
                    if (login.NewPassword.Length >= 8)
                    {
                        if (login.NewPassword == login.ConfirmNewPassword)
                        {
                            isLoggingIn = true;
                            trackedUser.TempPassword = null;
                            trackedUser.PasswordSalt = Guid.NewGuid().ToString();
                            trackedUser.Password = PasswordHelper.EncodePassword(login.NewPassword, trackedUser.PasswordSalt);
                        }
                        else
                        {
                            ModelState.AddModelError("ErrorMessage", "Your passwords do not match.");
                            return JsonModelState(ModelState);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ErrorMessage", "Your password must have at least 8 characters.");
                        return JsonModel(ModelState);
                    }

                    if (!isLoggingIn)
                    {
                        return JsonModel(new { IsUpdatingPassword = true });
                    }
                }
                else
                {
                    ModelState.AddModelError("LoginError", "The specified username or password is incorrect.");
                    return JsonModelState(ModelState);
                }
            }
            
            // wrong password
            if (PasswordHelper.EncodePassword(login.Password, trackedUser.PasswordSalt) != trackedUser.Password)
            {
                ModelState.AddModelError("LoginError", "The specified username or password is incorrect.");
                return JsonModelState(ModelState);
            }

            trackedUser.LastLoginDate = DateTime.Now;
            UserBo.Commit();

            UserHelper.Login(trackedUser);

            return JsonModel(new { LoginSuccess = true });
        }
    }
}
