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
    [Activity(Label = "RPG_Meetup")]
    public class BrowserActivity : Activity
    {
        protected AdView mAdView;
        List<Room> rooms = new List<Room>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Browser);
            toolbar();
            BrowserStart();
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
                string queryString = "select * from Room join User on Room.MasterID=User.id";
                MySqlDataAdapter adapter = new MySqlDataAdapter(queryString, sqlconn);
                
                adapter.Fill(tickets, "Item");

                foreach (DataRow row in tickets.Tables["Item"].Rows)
                {
                    rooms.Add(new Room(Int32.Parse(row[0].ToString()), Int32.Parse(row[3].ToString()),
                        row[1].ToString(), row[2].ToString(), row[5].ToString(), row[4].ToString(), row[6].ToString(),
                        row[9].ToString(),Boolean.Parse(row[7].ToString()),Boolean.Parse(row[8].ToString()),row[18].ToString(),null));
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

                MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_aplication_id));
                mAdView = FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                mAdView.LoadAd(adRequest);
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
            if (Global.logIn) checkFriendInv();
        }

        private void RoomSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            Global.selectedRoomId = rooms[e.Position].Id;
            StartActivity(typeof(RoomSelectedActivity));
        }

        private void checkFriendInv()
        {
            User invider=null;
            try
            {
                using (MySqlConnection sqlconn = new MySqlConnection(Resources.GetString(Resource.String.connection)))
                {
                    sqlconn.Open();

                    string queryString3 = "select * from Friends join User on UserId1=User.id where UserId2=@id AND accepted='0'";
                    MySqlCommand cmd3 = new MySqlCommand(queryString3, sqlconn);
                    cmd3.Parameters.AddWithValue("@id", Global.activeUser.Id);

                    using (MySqlDataReader rdr = cmd3.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            invider = new User(rdr.GetInt32(4), rdr.GetString(5), rdr.GetInt32(7), rdr.GetInt32(8), rdr.GetString(9));
                        }
                        rdr.Close();
                    }
                    sqlconn.Close();
                    if (invider!=null)
                    { 
                        Android.App.AlertDialog.Builder msg = new Android.App.AlertDialog.Builder(this);
                        msg.SetTitle("New invite");
                        msg.SetMessage(string.Format("{0} invide you to friendlist",invider.Username));
                        msg.SetPositiveButton("Accept", delegate {
                            acceptInv(invider);
                        });
                        msg.SetNegativeButton("Refuse", delegate {
                            refuseInv(invider);
                        });
                        msg.Create();
                        msg.Show();
                    }
                }
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
        }
        private void acceptInv(User invider)
        {
            using (MySqlConnection sqlconn2 = new MySqlConnection(Resources.GetString(Resource.String.connection)))
            {
                sqlconn2.Open();
                string sql = "UPDATE Friends set accepted='1' WHERE UserId1=@u1 AND UserId2=@u2";
                MySqlCommand cmd = new MySqlCommand(sql, sqlconn2);
                cmd.Parameters.AddWithValue("@u2", Global.activeUser.Id);
                cmd.Parameters.AddWithValue("@u1", invider.Id);
                cmd.ExecuteNonQuery();
                sqlconn2.Close();
            }
        }
        private void refuseInv(User invider)
        {
            using (MySqlConnection sqlconn2 = new MySqlConnection(Resources.GetString(Resource.String.connection)))
            {
                sqlconn2.Open();
                string sql = "DELETE FROM Friends WHERE UserId1=@u1 AND UserId2=@u2";
                MySqlCommand cmd = new MySqlCommand(sql, sqlconn2);
                cmd.Parameters.AddWithValue("@u2", Global.activeUser.Id);
                cmd.Parameters.AddWithValue("@u1", invider.Id);
                cmd.ExecuteNonQuery();
                sqlconn2.Close();
            }
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