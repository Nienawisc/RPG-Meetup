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

using MySql.Data.MySqlClient;
using System.Data;

using Android.Support.V7.App;
using Android.Gms.Ads;

namespace RPG_Meetup
{
    [Activity(Label = "JoinedRoomsActivity")]
    public class JoinedRoomsActivity : Activity
    {
        protected AdView mAdView;
        List<Room> rooms = new List<Room>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MyRooms);
            toolbar();
            BrowserStart();

            MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_aplication_id));
            mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);
        }
        private void BrowserStart()
        {
            ListView listOfRooms = FindViewById<ListView>(Resource.Id.listView1);
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                DataSet tickets = new DataSet();
                string queryString = string.Format("select * from Room join UserInRoom on RoomId=Room.Id where UserId='{0}'",Global.activeUser.Id);
                MySqlDataAdapter adapter = new MySqlDataAdapter(queryString, sqlconn);
                
                adapter.Fill(tickets, "Item");

                foreach (DataRow row in tickets.Tables["Item"].Rows)
                {
                    rooms.Add(new Room(Int32.Parse(row[0].ToString()), Int32.Parse(row[3].ToString()),
                        row[1].ToString(), row[2].ToString(), row[5].ToString(), row[4].ToString(), row[6].ToString(),
                        row[9].ToString(),Boolean.Parse(row[7].ToString()),Boolean.Parse(row[8].ToString()),"img",null));
                }
                foreach (var room in rooms)
                {
                    string queryString2 = "select Count(UserId) from UserInRoom where accept='1' AND RoomId = @id GROUP BY RoomId ";
                    MySqlCommand cmd2 = new MySqlCommand(queryString2, sqlconn);
                    cmd2.Parameters.AddWithValue("@id", room.Id);

                    using (MySqlDataReader rdr = cmd2.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            room.CurrentPlayerNumber = rdr.GetInt32(0);
                        }
                        rdr.Close();
                    }
                }
                sqlconn.Close();
            }
            catch (Exception ex)
            {
                Android.App.AlertDialog.Builder connectionException = new Android.App.AlertDialog.Builder(this);
                connectionException.SetTitle("Connection Error");
                connectionException.SetMessage(ex.ToString());
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
            }
            RoomsListView listAdapter = new RoomsListView(rooms,this);
            listOfRooms.Adapter = listAdapter;
            listOfRooms.ItemClick += RoomSelected;
        }

        private void RoomSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            Global.selectedRoomId = rooms[e.Position].Id;
            StartActivity(typeof(RoomSelectedActivity));
        }

        private void toolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Room Browser";
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (Global.logIn)
            {
                MenuInflater.Inflate(Resource.Menu.browser_menu, menu);
                return base.OnCreateOptionsMenu(menu);
            }
            else
            {
                MenuInflater.Inflate(Resource.Menu.browser_menu_guest, menu);
                return base.OnCreateOptionsMenu(menu);
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            switch(item.TitleFormatted.ToString())
            {
                case "Joined Rooms":
                    StartActivity(typeof(JoinedRoomsActivity));
                    break;
                case "My Rooms":
                    StartActivity(typeof(MyRoomsActivity));
                    break;
                case "Create Room":
                    StartActivity(typeof(CreateRoomActivity));
                    break;
                case "Profile":
                    Global.selectedUserId = Global.activeUser.Id;
                    StartActivity(typeof(ProfileViewActivity));
                    break;
                case "Log In":
                    StartActivity(typeof(LogInActivity));
                    break;
                case "Log Out":
                    Global.logIn = false;
                    StartActivity(typeof(MainActivity));
                    Finish();
                    break;
                case "Sign Up":
                    StartActivity(typeof(SingUpActivity));
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}