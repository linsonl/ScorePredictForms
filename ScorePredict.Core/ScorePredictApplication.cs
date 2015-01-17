﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ScorePredict.Core
{
    public class ScorePredictApplication : Application
    {
        private readonly IReadUserSecurityService _readUserSecurityService;

        public ScorePredictApplication(IStartupPageHelper getPageHelper, params InjectionModule[] modules)
        {
            Resolver.CurrentResolver.Initialize(modules);

            _readUserSecurityService = Resolver.CurrentResolver.Get<IReadUserSecurityService>();
            SetMainPage(getPageHelper);

            Resolver.CurrentResolver.AddModule(new CoreInjectionModule(MainPage.Navigation));
        }

        private void SetMainPage(IStartupPageHelper getPageHelper)
        {
            var user = _readUserSecurityService.ReadUser();
            if (user == null)
            {
                MainPage = getPageHelper.GetLoginPage();
                return;
            }

            if (string.IsNullOrEmpty(user.Username))
            {
                Resolver.CurrentResolver.GetInstance<IClient>().AuthenticateUser(user);
                MainPage = getPageHelper.GetUsernamePage(user);
                return;
            }

            MainPage = getPageHelper.GetMainPage();
        }
    }
}
