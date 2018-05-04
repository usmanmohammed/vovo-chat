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
using Android.Support.V4.App;
using Vovo.Models;
using Vovo.Android.Adapters;
using Vovo.Android.Activities;
using Newtonsoft.Json;

namespace Vovo.Android.Fragments
{
    public class ChatsFragment : Fragment
    {
        private MainActivity mainActivity;
        private ListView chatsListView;
        private List<Chat> chats;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mainActivity = this.Activity as MainActivity;
            View view = inflater.Inflate(Resource.Layout.chats_list, container, false);

            chatsListView = view.FindViewById<ListView>(Resource.Id.chats_list);
            chatsListView.ItemClick += ChatsListView_ItemClick;

            ViewGroup listViewHeader = (ViewGroup)inflater.Inflate(Resource.Layout.listview_header, chatsListView, false);
            (listViewHeader.FindViewById<TextView>(Resource.Id.listview_header)).Text = "Chats";

            chatsListView.AddHeaderView(listViewHeader, null, false);

            return view;
        }

        private void ChatsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (sender != null)
            {
                var selectedChat = chats[e.Position - 1];

                Intent chatIntent = new Intent(this.Context, typeof(ChatActivity));
                chatIntent.PutExtra("User", JsonConvert.SerializeObject(mainActivity.ActiveUser));

                StartActivity(chatIntent);
            }
        }

        public async override void OnResume()
        {
            base.OnResume();

            await mainActivity.GetChatsAsync();
            chats = mainActivity.ChatsList;
            chatsListView.Adapter = new ChatsAdapter(this.Context, chats, mainActivity.ActiveUser);
        }
    }
}