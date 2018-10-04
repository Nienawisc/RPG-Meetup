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
    class Room
    {
        private int id, NOPlayers;
        private string name, system, Date, City, Time, Desc;
        private bool forFriends, mustAccept;
        private string imgSrc;
        private User Master;
        private int currentPlayerNumber = 0;

        public Room(int id, int nOPlayers, string name, string system, string date, string city, string time, string desc, bool forFriends, bool mustAccept, string imgSrc, User master)
        {
            this.id = id;
            NOPlayers = nOPlayers;
            this.name = name;
            this.system = system;
            Date = date;
            City = city;
            Time = time;
            Desc = desc;
            this.forFriends = forFriends;
            this.mustAccept = mustAccept;
            this.imgSrc = imgSrc;
            Master = master;
        }

        public int Id { get => id; set => id = value; }
        public string System { get => system; set => system = value; }
        public string Date1 { get => Date; set => Date = value; }
        public string City1 { get => City; set => City = value; }
        public int NOPlayers1 { get => NOPlayers; set => NOPlayers = value; }
        public string Time1 { get => Time; set => Time = value; }
        public string Desc1 { get => Desc; set => Desc = value; }
        public bool ForFriends { get => forFriends; set => forFriends = value; }
        public bool MustAccept { get => mustAccept; set => mustAccept = value; }
        public string ImgSrc { get => imgSrc; set => imgSrc = value; }
        public string Name { get => name; set => name = value; }
        public int CurrentPlayerNumber { get => currentPlayerNumber; set => currentPlayerNumber = value; }
        internal User Master1 { get => Master; set => Master = value; }
    }
}