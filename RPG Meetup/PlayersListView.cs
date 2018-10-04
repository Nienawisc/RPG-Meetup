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

namespace RPG_Meetup
{
    class PlayersListView : BaseAdapter<User>
    {
        public List<User> userLists;
        private Context nContext;

        public PlayersListView(List<User> userLists, Context nContext)
        {
            this.userLists = userLists;
            this.nContext = nContext;
        }

        public override User this[int position]
        {
            get { return userLists[position]; }
        }

        public override int Count
        {
            get { return userLists.Count; }
        }

        public override long GetItemId(int position)
        {
            return userLists[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
                row = LayoutInflater.From(nContext).Inflate(Resource.Layout.User, null, false);
            LinearLayout background = row.FindViewById<LinearLayout>(Resource.Id.ll1);
            TextView name = row.FindViewById<TextView>(Resource.Id.txtName);
            if(userLists[position].AcceptInThisRoom)
                background.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ffc5e1a5"));

            name.Text = userLists[position].Username;
            return row;
        }
    }
}