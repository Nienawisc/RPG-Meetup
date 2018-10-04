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
    class User
    {
        private int id=-1;
        private string username;
        private int masterRep, playerRep;
        private string lastSeen;
        private bool acceptInThisRoom = false;
        private int friend = -1;

        public User(int id, string username, int masterRep, int playerRep, string lastSeen)
        {
            this.id = id;
            this.username = username;
            this.masterRep = masterRep;
            this.playerRep = playerRep;
            this.lastSeen = lastSeen;
        }

        public string Username { get => username; set => username = value; }
        public int MasterRep { get => masterRep; set => masterRep = value; }
        public int PlayerRep { get => playerRep; set => playerRep = value; }
        public string LastSeen { get => lastSeen; set => lastSeen = value; }
        public int Id { get => id; set => id = value; }
        public bool AcceptInThisRoom { get => acceptInThisRoom; set => acceptInThisRoom = value; }
        public int Friend { get => friend; set => friend = value; }
    }
}