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
    public class ChatThreadAdapter : BaseAdapter<Vovo.Models.Message>
    {
        private Context context;
        private List<Vovo.Models.Message> messages;
        private User activeUser;

        public ChatThreadAdapter(Context context, List<Vovo.Models.Message> messages, User activeUser)
        {
            this.messages = messages;
            this.context = context;
            this.activeUser = activeUser;
        }

        public void AddMessage(Vovo.Models.Message message)
        {
            messages.Add(message);
        }

        public override int Count
        {
            get { return messages.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Vovo.Models.Message this[int position]
        {
            get { return messages[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (activeUser.UserID == messages[position].UserID)
            {
                if (row == null)
                    row = LayoutInflater.From(context).Inflate(Resource.Layout.chats_message_left_listview_row, null, false);

                row.FindViewById<TextView>(Resource.Id.message_user).Text = messages[position].UserID ?? "sdsdsdssd";
                row.FindViewById<TextView>(Resource.Id.message_content).Text = messages[position].Content ?? "sdsdsdssd";

            }
            else
            {
                if (row == null)
                    row = LayoutInflater.From(context).Inflate(Resource.Layout.chats_message_right_listview_row, null, false);

                row.FindViewById<TextView>(Resource.Id.message_user).Text = messages[position].UserID ?? "sdsdsdssd";
                row.FindViewById<TextView>(Resource.Id.message_content).Text = messages[position].Content ?? "sdsdsdssd";
            }

            return row;
        }
    }
}