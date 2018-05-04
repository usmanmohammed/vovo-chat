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
    public class FriendsListAdapter : BaseAdapter<UserFriend>
    {
        private Context context;
        private List<UserFriend> friends;

        public FriendsListAdapter(Context context, List<UserFriend> friends)
        {
            this.friends = friends;
            this.context = context;
        }

        public override int Count
        {
            get { return friends.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override UserFriend this[int position]
        {
            get { return friends[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
                row = LayoutInflater.From(context).Inflate(Resource.Layout.friends_listview_row, null, false);

            (row.FindViewById<TextView>(Resource.Id.user_fullname)).Text = friends[position].Friend.FullName;
            //(row.FindViewById<TextView>(Resource.Id.user_status)).Text = friends[position].UserID ?? "no status";
            return row;
        }
    }
}