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

using SupportV7 = Android.Support.V7.App;
using SupportFragment = Android.Support.V7.Widget;
using Android.Util;
using Android.Graphics.Drawables;
using Android.Graphics;
using Vovo.Models;
using Vovo.Android.Adapters;
using System.Net.Http;
using Vovo.Android.Models;
using Newtonsoft.Json;

namespace Vovo.Android.Activities
{
    [Activity(Label = "Find User", Theme = "@style/Theme.AppTheme")]
    public class SearchUserActivity : SupportV7.AppCompatActivity
    {
        private ListView searchListView;
        private AlertDialog alertDialog;
        private ViewGroup listViewHeader;
        private List<User> users;
        HttpClient client;
        private User activeUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(global::Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            activeUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));

            client = new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 30)
            };

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.search_users);

            var toolbar = FindViewById<SupportFragment.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);

            searchListView = FindViewById<ListView>(Resource.Id.users_list);
            searchListView.ItemClick += SearchListView_ItemClick;

            listViewHeader = (ViewGroup)LayoutInflater.Inflate(Resource.Layout.listview_header, searchListView, false);
            searchListView.AddHeaderView(listViewHeader, null, false);

            FindViewById<Button>(Resource.Id.btnSearch).Click += SearchUserActivity_Click;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
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

        private void SearchListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (sender != null)
            {
                var selectedUser = users[e.Position - 1];

                alertDialog = new AlertDialog.Builder(this).Create();
                alertDialog.SetTitle("Add Friend?");
                alertDialog.SetIcon(Resource.Drawable.ic_person_add_black_24dp);
                alertDialog.SetMessage($"Do you wish to add {selectedUser.FullName} as a Friend?");
                alertDialog.SetButton2("OK", async (_sender, args) =>
                {
                    try
                    {
                            //Add To Friends List
                            var serializedUserFriend = JsonConvert.SerializeObject(new UserFriend
                        {
                            UserUserID = activeUser.UserID,
                            FriendUserID = selectedUser.UserID,
                            DateCreated = DateTime.Now
                        });

                        var content = new StringContent(serializedUserFriend, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(new Uri($"{RestService.BaseAddress}/api/users/addfriend"), content);

                        if (response.IsSuccessStatusCode)
                        {
                            var addedFriend = JsonConvert.DeserializeObject<UserFriend>(await response.Content.ReadAsStringAsync());

                            var alertDialog = new AlertDialog.Builder(this).Create();
                            alertDialog.SetIcon(Resource.Drawable.ic_person_add_black_24dp);
                            alertDialog.SetMessage($"{selectedUser.FullName} added!");
                            alertDialog.SetButton("OK", (s, a) => { });
                            alertDialog.Show();

                        }
                        else
                        {
                                //Login Failed
                                alertDialog = new AlertDialog.Builder(this).Create();
                            alertDialog.SetMessage("An error has occured. Please try again.");
                            alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                            alertDialog.SetButton("OK", (s, a) => { });
                            alertDialog.Show();
                        }
                    }
                    catch (Exception)
                    {
                        alertDialog = new AlertDialog.Builder(this).Create();
                        alertDialog.SetTitle("Network Error");
                        alertDialog.SetMessage("Error connecting to the internet. Please check your internet settings and try again.");
                        alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                        alertDialog.SetButton("OK", (s, a) => { });
                        alertDialog.Show();
                    }

                        //Close Window
                        //Navigate to Friends List
                    });
                alertDialog.SetButton("Cancel", (_sender, args) =>
                {
                    alertDialog.Cancel();
                });
                alertDialog.Show();
            }
        }

        private async void SearchUserActivity_Click(object sender, EventArgs e)
        {
            string query = FindViewById<EditText>(Resource.Id.search_box).Text;
            //Launch Progress Bar Overlay
            ProgressDialog progressDialog = new ProgressDialog(this)
            {
                Indeterminate = true
            };
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.SetCancelable(false);
            progressDialog.Show();

            try
            {
                var response = await client.GetAsync(new Uri($"{RestService.BaseAddress}/api/users/searchusers?id={query}"));
                if (response.IsSuccessStatusCode)
                {
                    progressDialog.Dismiss();

                    users = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());

                    (listViewHeader.FindViewById<TextView>(Resource.Id.listview_header)).Text = "Search Results";
                    searchListView.Adapter = new UsersListAdapter(this, users);
                }
                else
                {
                    progressDialog.Dismiss();

                    //Login Failed
                    alertDialog = new AlertDialog.Builder(this).Create();
                    alertDialog.SetMessage("An error has occured. Please try again.");
                    alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                    alertDialog.SetButton("OK", (_sender, args) => { });
                    alertDialog.Show();
                }
            }
            catch (Exception)
            {
                progressDialog.Dismiss();

                alertDialog = new AlertDialog.Builder(this).Create();
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