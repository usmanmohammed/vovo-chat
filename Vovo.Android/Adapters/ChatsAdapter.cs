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
using Vovo.Models;

namespace Vovo.Android.Adapters
{
    public class ChatsAdapter : BaseAdapter<Chat>
    {
        private Context context;
        private List<Chat> chats;
        private User activeUser;

        public ChatsAdapter(Context context, List<Chat> chats, User activeUser)
        {
            this.chats = chats;
            this.context = context;
            this.activeUser = activeUser;
        }

        public override int Count
        {
            get { return chats.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Chat this[int position]
        {
            get { return chats[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
                row = LayoutInflater.From(context).Inflate(Resource.Layout.chats_listview_row, null, false);

            if (activeUser.UserID == chats[position].Sender.UserID)
                (row.FindViewById<TextView>(Resource.Id.user_fullname)).Text = chats[position].Receiver.FullName;

            else
                (row.FindViewById<TextView>(Resource.Id.user_fullname)).Text = chats[position].Sender.FullName;

            (row.FindViewById<TextView>(Resource.Id.chat_title)).Text = chats[position].LastMessage ?? string.Empty;

            return row;
        }
    }
}