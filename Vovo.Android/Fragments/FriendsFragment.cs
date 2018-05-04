using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SupportV4 = Android.Support.V4.App;

using Vovo.Models;
using Vovo.Android.Adapters;
using Vovo.Android.Activities;
using System.Threading.Tasks;
using Android.App;
using Vovo.Android.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace Vovo.Android.Fragments
{
    public class FriendsFragment : SupportV4.Fragment
    {
        private ListView friendsListView;
        private MainActivity mainActivity;
        private List<UserFriend> friends;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mainActivity = this.Activity as MainActivity;
            View view = inflater.Inflate(Resource.Layout.friends_list, container, false);

            friendsListView = view.FindViewById<ListView>(Resource.Id.friends_list);
            friendsListView.ItemClick += FriendsListView_ItemClick;

            ViewGroup listViewHeader = (ViewGroup)inflater.Inflate(Resource.Layout.listview_header, friendsListView, false);
            (listViewHeader.FindViewById<TextView>(Resource.Id.listview_header)).Text = "Friends";

            friendsListView.AddHeaderView(listViewHeader, null, false);

            return view;
        }

        private void FriendsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (sender != null)
            {
                var selectedUser = friends[e.Position - 1];

                Intent chatIntent = new Intent(this.Context, typeof(ChatActivity));
                chatIntent.PutExtra("User", JsonConvert.SerializeObject(mainActivity.ActiveUser));
                chatIntent.PutExtra("UserFriend", JsonConvert.SerializeObject(selectedUser));

                StartActivity(chatIntent);
            }
        }

        public async override void OnResume()
        {
            base.OnResume();

            await mainActivity.GetFriendsAsync();
            friends = mainActivity.FriendsList;
            friendsListView.Adapter = new FriendsListAdapter(this.Context, friends);
        }
    }
}