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
    static class Global
    {
        public static User activeUser;
        public static bool logIn = false;
        public static int selectedRoomId = -1;
        public static int selectedUserId = -1;
    }
}