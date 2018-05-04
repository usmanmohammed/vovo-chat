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
using SupportFragment = Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Graphics.Drawables;
using Android.Content.PM;

using Vovo.Android.Models;
using Android.Util;
using Android.Graphics;

namespace Vovo.Android.Activities
{
    [Activity(Label = "Password", Theme = "@style/Theme.AppTheme")]
    public class PasswordActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(global::Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.password);

            var toolbar = FindViewById<SupportFragment.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.form_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == 16908332)
            {
                this.Finish();
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}