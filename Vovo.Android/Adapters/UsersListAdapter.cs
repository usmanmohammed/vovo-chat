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
    public class UsersListAdapter : BaseAdapter<User>
    {
        private Context context;
        private List<User> users;

        public UsersListAdapter(Context context, List<User> users)
        {
            this.users = users;
            this.context = context;
        }

        public override int Count
        {
            get { return users.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override User this[int position]
        {
            get { return users[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
                row = LayoutInflater.From(context).Inflate(Resource.Layout.users_listview_row, null, false);

            (row.FindViewById<TextView>(Resource.Id.user_fullname)).Text = users[position].FullName;
            row.FindViewById<TextView>(Resource.Id.user_id).Text = users[position].UserID ?? "no status";
            return row;
        }
    }
}