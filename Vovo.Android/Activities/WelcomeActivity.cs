using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Vovo.Android.Fragments;
using Vovo.Models;

namespace Vovo.Android.Activities
{
    [Activity(Label = "Vovo", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppTheme") ]
    public class WelcomeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(global::Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.welcome);

            //Launch Sign In
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.signin_signup_container, new SigninFragment());
            fragmentTransaction.Commit();
        }
    }
}