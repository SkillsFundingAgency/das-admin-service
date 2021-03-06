﻿using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Extensions;
using System;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class CheckSessionAttribute : Attribute
    {
        public string Controller { get; private set; }
        public string Action { get; private set; }
        public string Key { get; private set; }
        public CheckSession CheckSession { get; private set; }

        public CheckSessionAttribute()
            : this(nameof(AccountController).RemoveController(), nameof(AccountController.SignIn), "OrganisationName")
        {
        }

        public CheckSessionAttribute(string key, CheckSession checkSession)
            : this(string.Empty, string.Empty, key, checkSession)
        {
            if (checkSession == CheckSession.Redirect)
                throw new ArgumentException("checkSession", "Must specify constructor with redirect arguments for redirect on CheckSession");
        }

        public CheckSessionAttribute(string controller, string action, string key, CheckSession checkSession = CheckSession.Redirect)
        {
            Controller = controller.RemoveController();
            Action = action;
            Key = key;
            CheckSession = checkSession;
        }
    }

    public enum CheckSession
    {
        Ignore,
        Redirect,
        Error
    }
}