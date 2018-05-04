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
using Vovo.Android.Models;
using Vovo.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Vovo.Android.Fragments
{
    public class SignupFragment : Fragment
    {
        private AlertDialog alertDialog;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.sign_up, container, false);

            var signInText = view.FindViewById<TextView>(Resource.Id.signInText);
            signInText.Click += SignInText_Click;

            var btnSignUp = view.FindViewById<Button>(Resource.Id.btnSignUp);
            btnSignUp.Click += BtnSignUp_Click;

            return view;
        }

        private async void BtnSignUp_Click(object sender, EventArgs e)
        {
            //Launch Progress Bar Overlay
            ProgressDialog progressDialog = new ProgressDialog(this.Activity)
            {
                Indeterminate = true
            };

            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.SetMessage("Creating account...");
            progressDialog.SetCancelable(false);

            //Get Values
            var userID = View.FindViewById<TextView>(Resource.Id.emailAddressText).Text;
            var fullName = View.FindViewById<TextView>(Resource.Id.fullNameText).Text;
            var password = View.FindViewById<EditText>(Resource.Id.passwordText).Text;
            var passwordConfirmation = View.FindViewById<EditText>(Resource.Id.confirmPasswordText).Text;


            if (string.IsNullOrEmpty(fullName))
            {
                alertDialog = new AlertDialog.Builder(this.Activity).Create();
                alertDialog.SetTitle("Invalid Full name");
                alertDialog.SetMessage("Full Name required.");
                alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                alertDialog.SetButton("OK", (_sender, args) => { });
                alertDialog.Show();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                alertDialog = new AlertDialog.Builder(this.Activity).Create();
                alertDialog.SetMessage("Password is required");
                alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                alertDialog.SetButton("OK", (_sender, args) => { });
                alertDialog.Show();
                return;
            }

            if (!userID.Contains('@') || string.IsNullOrEmpty(userID))
            {
                alertDialog = new AlertDialog.Builder(this.Activity).Create();
                alertDialog.SetTitle("Invalid Email");
                alertDialog.SetMessage("Invalid Email Addres. Please review.");
                alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                alertDialog.SetButton("OK", (_sender, args) => { });
                alertDialog.Show();
                return;
            }


            if (password != passwordConfirmation)
            {
                alertDialog = new AlertDialog.Builder(this.Activity).Create();
                alertDialog.SetTitle("Password Mismatch");
                alertDialog.SetMessage("Passwords do not match. Please review.");
                alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                alertDialog.SetButton("OK", (_sender, args) => { });
                alertDialog.Show();
                return;
            }     

            var user = new User
            {
                UserID = userID,
                FullName = fullName,
                PasswordHash = password,
                Location = "9.042301:7.984054",
                DateCreated = DateTime.UtcNow
            };

            var serializedUser = JsonConvert.SerializeObject(user);
            HttpContent content = new StringContent(serializedUser, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 10)
            };

            try
            {
                progressDialog.Show();

                var response = await client.GetAsync(new Uri($"{RestService.BaseAddress}/api/users/accounttaken?id={user.UserID}"));
                if (response.IsSuccessStatusCode)
                {
                    var status = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
                    if (!status)
                    {
                        var postResponse = await client.PostAsync(new Uri($"{RestService.BaseAddress}/api/users"), content);
                        if (postResponse.IsSuccessStatusCode)
                        {
                            progressDialog.Dismiss();

                            Intent intent = new Intent(this.Activity, typeof(MainActivity));
                            intent.PutExtra("User", serializedUser);
                            StartActivity(intent);
                            this.Activity.Finish();
                        }
                        else
                        {
                            progressDialog.Dismiss();

                            alertDialog = new AlertDialog.Builder(this.Activity).Create();
                            alertDialog.SetTitle("Registration Failed");
                            alertDialog.SetMessage("An error has occured. Please try again later.");
                            alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                            alertDialog.SetButton("OK", (_sender, args) => { });
                            alertDialog.Show();
                            return;
                        }
                    }
                    else
                    {
                        progressDialog.Dismiss();

                        alertDialog = new AlertDialog.Builder(this.Activity).Create();
                        alertDialog.SetMessage("Username Exists. Please use another email address");
                        alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                        alertDialog.SetButton("OK", (_sender, args) => { });
                        alertDialog.Show();
                        return;
                    }
                }
                else
                {
                    progressDialog.Dismiss();

                    alertDialog = new AlertDialog.Builder(this.Activity).Create();
                    alertDialog.SetTitle("Network Error");
                    alertDialog.SetMessage("Error connecting to the internet. Please check your internet settings and try again.");
                    alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                    alertDialog.SetButton("OK", (_sender, args) => { });
                    alertDialog.Show();
                }
            }
            catch (Exception)
            {
                progressDialog.Dismiss();

                alertDialog = new AlertDialog.Builder(this.Activity).Create();
                alertDialog.SetTitle("Error");
                alertDialog.SetMessage("An error has occured. Please try again.");
                alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                alertDialog.SetButton("OK", (_sender, args) => { });
                alertDialog.Show();
            }
        }

        private void SignInText_Click(object sender, EventArgs e)
        {
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.SetTransition(FragmentTransit.FragmentOpen);
            fragmentTransaction.Replace(Resource.Id.signin_signup_container, new SigninFragment());
            fragmentTransaction.Commit();
        }
    }
}