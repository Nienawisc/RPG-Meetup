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
    class RoomsListView : BaseAdapter<Room>
    {
        public List<Room> roomLists;
        private Context nContext;

        public RoomsListView(List<Room> roomLists, Context nContext)
        {
            this.roomLists = roomLists;
            this.nContext = nContext;
        }

        public override Room this[int position]
        {
            get { return roomLists[position]; }
        }

        public override int Count
        {
            get { return roomLists.Count; }
        }

        public override long GetItemId(int position)
        {
            return roomLists[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
                row = LayoutInflater.From(nContext).Inflate(Resource.Layout.Room, null, false);

            TextView name = row.FindViewById<TextView>(Resource.Id.txtName);
           // TextView system = row.FindViewById<TextView>(Resource.Id.txtSystem);
            TextView City = row.FindViewById<TextView>(Resource.Id.txtCity);
            TextView NOPlayer = row.FindViewById<TextView>(Resource.Id.txtNOPlayers);
            TextView Date = row.FindViewById<TextView>(Resource.Id.txtDate);
            ImageView img = row.FindViewById<ImageView>(Resource.Id.imgProfil);

            name.Text = roomLists[position].Name;
          //  system.Text = roomLists[position].System;
            City.Text = roomLists[position].City1;
            NOPlayer.Text = string.Format("Players: {0}/{1}", roomLists[position].CurrentPlayerNumber, roomLists[position].NOPlayers1);
            Date.Text = roomLists[position].Date1.Split(' ')[0];
          //  img.SetImageResource(Resource.Drawable.goblin);
            return row;
        }
    }
}