using BllEntities;
using Client.Misc;
using Client.ServerManager;
using Client.ServerManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.EventClasses
{
    public class Authorization
    {
        const string LOGIN_TAG = "login";
        const string PASSWORD_TAG = "password";

        public BllUser User { get; private set; }
        IAuthorizationManager authorizationManager;

        public Authorization()
        {
            User = CreateBlankUserAccordingToLoginAndPasswordFromConfig();
        }
        

        public void Authorize(ServerInstance serverInstance)
        {
            try
            {
                authorizationManager = new AuthorizationProxy(serverInstance.server);
                if (!HasUserLoginAndPassword())
                {
                    SetLoginAndPasswordUsingSignInForm();
                    WriteLoginAndPasswordToConfig();
                }
                else
                {                    
                    User = authorizationManager.SignIn(User);                    
                }
            }
            catch (ConnectionFailedException ex)
            {
                throw ex;
            }
            catch (UserIsNullException ex)
            {
                throw ex;
            }
        }

        private BllUser CreateBlankUserAccordingToLoginAndPasswordFromConfig()
        {
            string login = AppConfigManager.GetKeyValue(LOGIN_TAG);
            string password = AppConfigManager.GetKeyValue(PASSWORD_TAG);
            if ((login != null) && (password != null))
            {
                return new BllUser
                {
                    Login = login,
                    Password = password
                };
            }
            return new BllUser
            {
                Login = "",
                Password = ""
            };
        }

        private bool HasUserLoginAndPassword()
        {
            return User.Login != "";
        }

        private void SetLoginAndPasswordUsingSignInForm()
        {
            bool success = false;
            SignInForm signInForm = new SignInForm();
            while (success == false)
            {
                try
                {                   
                    signInForm.ShowDialog();
                    User = signInForm.User;
                    User = authorizationManager.SignIn(User);
                    success = true;
                }
                catch (UserIsNullException ex)
                {
                    success = false;
                    signInForm.ShowInvalidLoginMessage();
                }
            }
        }

        private void WriteLoginAndPasswordToConfig()
        {
            AppConfigManager.SetKeyValue(LOGIN_TAG, User.Login);
            AppConfigManager.SetKeyValue(PASSWORD_TAG, User.Password);
        }
    }
}
