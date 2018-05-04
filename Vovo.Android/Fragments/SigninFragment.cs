using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Vovo.Android.Activities;
using Vovo.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Vovo.Android.Models;

namespace Vovo.Android.Fragments
{
    public class SigninFragment : Fragment
    {
        private AlertDialog alertDialog;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Hookup Events
        }

        private void SignUpText_Click(object sender, EventArgs e)
        {      
            var fragmentManager = FragmentManager.BeginTransaction();
            fragmentManager.SetTransition(FragmentTransit.FragmentOpen);
            fragmentManager.Replace(Resource.Id.signin_signup_container, new SignupFragment(), "Sign_Up");
            fragmentManager.Commit();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.sign_in, container, false);

            view.FindViewById<Button>(Resource.Id.btnSignIn).Click += SigninFragment_Click;
            view.FindViewById<TextView>(Resource.Id.signupText).Click += SignUpText_Click;

            return view;
        }

        private async void SigninFragment_Click(object sender, EventArgs e)
        {
            //Validate UI

            //Launch Progress Bar Overlay
            ProgressDialog progressDialog = new ProgressDialog(this.Activity);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.SetMessage("Signing in...");
            progressDialog.SetCancelable(false);
            progressDialog.Show();

            //Get Values
            var userID = View.FindViewById<TextView>(Resource.Id.emailAddressText).Text;
            var password = View.FindViewById<EditText>(Resource.Id.passwordText).Text;

            HttpClient client = new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 5),
            };

            try
            {
                var response = await client.GetAsync(new Uri($"{RestService.BaseAddress}/api/users/login?un={userID}&pw={password}"));
                if (response.IsSuccessStatusCode)
                {
                    progressDialog.Dismiss();

                    Intent intent = new Intent(this.Activity, typeof(MainActivity));
                    intent.PutExtra("User", await response.Content.ReadAsStringAsync());
                    StartActivity(intent);
                    this.Activity.Finish();
                }
                else
                {
                    progressDialog.Dismiss();

                    //Login Failed
                    alertDialog = new AlertDialog.Builder(this.Activity).Create();
                    alertDialog.SetTitle("Invalid Login");
                    alertDialog.SetMessage("Email Address / Password incorrect. Please check your login credentials and try again.");
                    alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                    alertDialog.SetButton("OK", (_sender, args) => { });
                    alertDialog.Show();
                }
            }
            catch (Exception)
            {
                progressDialog.Dismiss();

                alertDialog = new AlertDialog.Builder(this.Activity).Create();
                alertDialog.SetTitle("Network Error");
                alertDialog.SetMessage("Error connecting to the internet. Please check your internet settings and try again.");
                alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                alertDialog.SetButton("OK", (_sender, args) =>
                {
                    //Do Something
                });

                alertDialog.Show();
            }
        }
    }
}