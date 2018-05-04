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
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget;
using Vovo.Android.Fragments;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportMain = Android.App;
using Android.Support.V4.View;
using Vovo.Android.Adapters;
using Vovo.Android.Models;
using Vovo.Models;
using Newtonsoft.Json;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Graphics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vovo.Android.Activities
{
    [Activity(Label = "Vovo", Theme = "@style/Theme.AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private User activeUser;

        public User ActiveUser
        {
            get
            {
                return activeUser;
            }
        }

        private List<UserFriend> friendsList;

        public List<UserFriend> FriendsList
        {
            get { return friendsList ?? new List<UserFriend>(); }
            set { friendsList = value; }
        }

        public async Task GetFriendsAsync()
        {
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
                var response = await client.GetAsync(new Uri($"{RestService.BaseAddress}/api/users/getfriends?id={activeUser.UserID}"));
                if (response.IsSuccessStatusCode)
                {
                    progressDialog.Dismiss();

                    friendsList = JsonConvert.DeserializeObject<List<UserFriend>>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    progressDialog.Dismiss();

                    //Login Failed
                    alertDialog = new SupportMain.AlertDialog.Builder(this).Create();
                    alertDialog.SetMessage("An error has occured. Please try again.");
                    alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                    alertDialog.SetButton("OK", (_sender, args) => { });
                    alertDialog.Show();
                }
            }
            catch (Exception)
            {
                progressDialog.Dismiss();

                alertDialog = new SupportMain.AlertDialog.Builder(this).Create();
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

        private List<Chat> chatsList;

        public List<Chat> ChatsList
        {
            get { return chatsList ?? new List<Chat>(); }
            set { chatsList = value; }
        }

        public async Task GetChatsAsync()
        {
            try
            {
                var response = await client.GetAsync(new Uri($"{RestService.BaseAddress}/api/users/getchats?id={activeUser.UserID}"));
                if (response.IsSuccessStatusCode)
                {
                    chatsList = JsonConvert.DeserializeObject<List<Chat>>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    //Login Failed
                    alertDialog = new SupportMain.AlertDialog.Builder(this).Create();
                    alertDialog.SetMessage("An error has occured. Please try again.");
                    alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                    alertDialog.SetButton("OK", (_sender, args) => { });
                    alertDialog.Show();
                }
            }
            catch (Exception)
            {
                alertDialog = new SupportMain.AlertDialog.Builder(this).Create();
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

        private DrawerLayout drawerLayout;
        private HttpClient client;
        private SupportMain.AlertDialog alertDialog;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(global::Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            activeUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));
            client = new HttpClient { Timeout = new TimeSpan(0, 0, 30) };

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main);

            var titles = CharSequence.ArrayFromStringArray(new[] { "Map", "Chats", "Friends" });
            var fragments = new SupportFragment[] { new MapFragment(), new ChatsFragment(), new FriendsFragment() };

            var viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = new TabsFragmentPagerAdapter(SupportFragmentManager, fragments, titles);

            var tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            tabLayout.SetupWithViewPager(viewPager);

            View locationTabView = LayoutInflater.Inflate(Resource.Layout.custom_tab, null);
            locationTabView.FindViewById(Resource.Id.icon).SetBackgroundResource(Resource.Drawable.ic_location_on_white_48dp);
            tabLayout.GetTabAt(0).SetCustomView(locationTabView);

            View messagesTabView = LayoutInflater.Inflate(Resource.Layout.custom_tab, null);
            messagesTabView.FindViewById(Resource.Id.icon).SetBackgroundResource(Resource.Drawable.ic_mail_outline_white_48dp);
            tabLayout.GetTabAt(1).SetCustomView(messagesTabView);

            View searchTabView = LayoutInflater.Inflate(Resource.Layout.custom_tab, null);
            searchTabView.FindViewById(Resource.Id.icon).SetBackgroundResource(Resource.Drawable.ic_group_white_48dp);
            tabLayout.GetTabAt(2).SetCustomView(searchTabView);

            var toolbar = FindViewById<SupportToolbar.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            var header = navigationView.GetHeaderView(0);
            header.FindViewById<TextView>(Resource.Id.nav_header_fullName).Text = activeUser.FullName;
            header.FindViewById<TextView>(Resource.Id.nav_header_emailAddress).Text = activeUser.UserID;

            await GetFriendsAsync();
            await GetChatsAsync();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.home_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case (Resource.Id.menu_add_person):
                    Intent searchUserIntent = new Intent(this, typeof(SearchUserActivity));
                    searchUserIntent.PutExtra("User", JsonConvert.SerializeObject(activeUser));
                    StartActivity(searchUserIntent);
                    break;

                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.profile_activity):
                    Intent profileIntent = new Intent(this, typeof(ProfileActivity));
                    profileIntent.PutExtra("User", JsonConvert.SerializeObject(activeUser));
                    StartActivity(profileIntent);
                    break;

                case (Resource.Id.changePassword_activity):
                    Intent passwordIntent = new Intent(this, typeof(PasswordActivity));
                    StartActivity(passwordIntent);
                    break;

                case (Resource.Id.settings_activity):
                    Intent settingsIntent = new Intent(this, typeof(SettingsActivity));
                    StartActivity(settingsIntent);
                    break;

                case (Resource.Id.logout_activity):
                    Intent intent = new Intent(this, typeof(WelcomeActivity));
                    StartActivity(intent);
                    Toast.MakeText(this, "Logout", ToastLength.Short).Show();
                    this.Finish();

                    break;

                default:
                    break;
            }
        }
    }
}