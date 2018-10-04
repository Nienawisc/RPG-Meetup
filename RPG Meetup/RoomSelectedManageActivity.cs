using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MySql.Data.MySqlClient;

namespace RPG_Meetup
{
    [Activity(Label = "RoomSelectedManageActivity")]
    public class RoomSelectedManageActivity : Activity
    {
        Room thisRoom;
        List <User> usersInRoom = new List<User>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ManageRoom);
            toolbar();
            
            try
            {
                using (MySqlConnection sqlconn = new MySqlConnection(Resources.GetString(Resource.String.connection)))
                {
                    sqlconn.Open();
                    string queryString = "select * from Room join User on Room.MasterID = User.id" +
                            " where Room.Id=@id";
                    MySqlCommand cmd = new MySqlCommand(queryString, sqlconn);
                    cmd.Parameters.AddWithValue("@id", Global.selectedRoomId);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            User master = new User(rdr.GetInt32(11), rdr.GetString(12), rdr.GetInt32(14), rdr.GetInt32(15), rdr.GetString(16));
                            thisRoom = new Room(rdr.GetInt32(0), rdr.GetInt32(3), rdr.GetString(1), rdr.GetString(2), rdr.GetString(5).Split(' ')[0], rdr.GetString(4), rdr.GetString(6), rdr.GetString(9), rdr.GetBoolean(7), rdr.GetBoolean(8), "img", master);
                        }
                        rdr.Close();
                    }

                    string queryString2 = "select User.id,Username,`Master reputation`,`Player reputation`,`Last seen`,accept from UserInRoom join User on UserId=User.id where RoomId = @id";
                    MySqlCommand cmd2 = new MySqlCommand(queryString2, sqlconn);
                    cmd2.Parameters.AddWithValue("@id", Global.selectedRoomId);

                    using (MySqlDataReader rdr = cmd2.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            usersInRoom.Add(new User(rdr.GetInt32(0),rdr.GetString(1), rdr.GetInt32(2), rdr.GetInt32(3),rdr.GetString(4)));
                            usersInRoom.Last().AcceptInThisRoom = rdr.GetBoolean(5);
                        }
                        rdr.Close();
                    }
                    thisRoom.CurrentPlayerNumber = usersInRoom.Count();
                    sqlconn.Close();
                }

                ListView playerListView = FindViewById<ListView>(Resource.Id.playerListView);

                PlayersListView listAdapter = new PlayersListView(usersInRoom, this);
                playerListView.Adapter = listAdapter;
                playerListView.ItemClick += PlayerSelected;
            }
            catch (Exception ex)
            {
                AlertDialog.Builder connectionException = new AlertDialog.Builder(this);
                connectionException.SetTitle("Connection Error");
                connectionException.SetMessage(ex.ToString());
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
            }
        }

        private void PlayerSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            AlertDialog.Builder msg = new AlertDialog.Builder(this);
            msg.SetTitle("What do you want to do?");
            
            if (!usersInRoom[e.Position].AcceptInThisRoom)
            {
                msg.SetMessage("This player want to join your room");
                msg.SetPositiveButton("Accept", delegate
                {
                    accept(usersInRoom[e.Position].Id);
                });
                msg.SetNegativeButton("Refuse", delegate
                {
                    refuseOrKick(usersInRoom[e.Position].Id);
                });
            }
            else
            {
                msg.SetMessage("This player is already in the room");
                msg.SetPositiveButton("Give rate", delegate
                {
                    AlertDialog.Builder msg2 = new AlertDialog.Builder(this);
                    msg2.SetTitle("Rating");
                    if (IsRated(usersInRoom[e.Position].Id))
                    {
                        msg2.SetMessage("Cannot rate rated player.");
                        msg2.SetNegativeButton("return", delegate{});
                    }
                    else
                    {

                        msg2.SetMessage("As a game master, how do you rate this player?");
                        msg2.SetPositiveButton("Well Player", delegate
                        {
                            PositiveRate(usersInRoom[e.Position].Id);
                        });
                        msg2.SetNegativeButton("Bad Player", delegate
                        {
                            NegativeRate(usersInRoom[e.Position].Id);
                        });
                    }

                    msg2.Create();
                    msg2.Show();
                });
                msg.SetNegativeButton("Kick", delegate
                {
                    refuseOrKick(usersInRoom[e.Position].Id);
                });
            }

            msg.Create();
            msg.Show();
            // Global.selectedUserId = usersInRoom[e.Position].Id;
            // StartActivity(typeof(ProfileViewActivity));
        }
        private void accept(int id)
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                string sql2 = "UPDATE UserInRoom Set accept='1' WHERE UserId=@userId AND RoomId=@roomId";
                MySqlCommand cmd2 = new MySqlCommand(sql2, sqlconn);
                cmd2.Parameters.AddWithValue("@userId", id);
                cmd2.Parameters.AddWithValue("@roomId", Global.selectedRoomId);
                cmd2.ExecuteNonQuery();
                sqlconn.Close();
                AlertDialog.Builder msg = new AlertDialog.Builder(this);
                msg.SetTitle("Operation done!");
                msg.SetMessage("You have successfully accepted the player");
                msg.SetPositiveButton("Ok", delegate
                {
                    StartActivity(typeof(RoomSelectedManageActivity));
                    Finish();
                });
                msg.Create();
                msg.Show();
            }
            catch (Exception ex)
            {
                AlertDialog.Builder connectionException = new AlertDialog.Builder(this);
                connectionException.SetTitle("Connection Error");
                connectionException.SetMessage(ex.ToString());
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
            }
        }
        private void refuseOrKick(int id)
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                string sql2 = "DELETE FROM UserInRoom WHERE RoomId=@roomId AND UserId=@userId";
                MySqlCommand cmd2 = new MySqlCommand(sql2, sqlconn);
                cmd2.Parameters.AddWithValue("@userId", id);
                cmd2.Parameters.AddWithValue("@roomId", Global.selectedRoomId);
                cmd2.ExecuteNonQuery();
                sqlconn.Close();
                AlertDialog.Builder msg = new AlertDialog.Builder(this);
                msg.SetTitle("Operation done!");
                msg.SetMessage("You have successfully kicked/refused the player");
                msg.SetPositiveButton("Ok", delegate
                {
                    StartActivity(typeof(RoomSelectedManageActivity));
                    Finish();
                });
                msg.Create();
                msg.Show();

            }
            catch (Exception ex)
            {
                AlertDialog.Builder connectionException = new AlertDialog.Builder(this);
                connectionException.SetTitle("Connection Error");
                connectionException.SetMessage(ex.ToString());
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
            }
        }
        private void PositiveRate(int id)
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                string sql2 = "UPDATE User Set `Player reputation`=`Player reputation`+1 WHERE Id=@userId";
                MySqlCommand cmd2 = new MySqlCommand(sql2, sqlconn);
                cmd2.Parameters.AddWithValue("@userId", id);
                cmd2.ExecuteNonQuery();

                string queryString = "update UserInRoom set rated='1' where RoomId = @id AND UserId =@userId";
                MySqlCommand cmd = new MySqlCommand(queryString, sqlconn);
                cmd.Parameters.AddWithValue("@id", Global.selectedRoomId);
                cmd.Parameters.AddWithValue("@userId", id);
                cmd.ExecuteNonQuery();

                sqlconn.Close();
                AlertDialog.Builder msg = new AlertDialog.Builder(this);
                msg.SetTitle("Operation done!");
                msg.SetMessage("You have successfully rated the player");
                msg.SetPositiveButton("Ok", delegate
                {
                    StartActivity(typeof(RoomSelectedManageActivity));
                    Finish();
                });
                msg.Create();
                msg.Show();
            }
            catch (Exception ex)
            {
                AlertDialog.Builder connectionException = new AlertDialog.Builder(this);
                connectionException.SetTitle("Connection Error");
                connectionException.SetMessage(ex.ToString());
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
            }
        }
        private void NegativeRate(int id)
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                string sql2 = "UPDATE User Set `Player reputation`=`Player reputation`-1 WHERE Id=@userId";
                MySqlCommand cmd2 = new MySqlCommand(sql2, sqlconn);
                cmd2.Parameters.AddWithValue("@userId", id);
                cmd2.ExecuteNonQuery();

                string queryString = "update UserInRoom set rated='1' where RoomId = @id AND UserId =@userId";
                MySqlCommand cmd = new MySqlCommand(queryString, sqlconn);
                cmd.Parameters.AddWithValue("@id", Global.selectedRoomId);
                cmd.Parameters.AddWithValue("@userId", id);
                cmd.ExecuteNonQuery();

                sqlconn.Close();
                AlertDialog.Builder msg = new AlertDialog.Builder(this);
                msg.SetTitle("Operation done!");
                msg.SetMessage("You have successfully rated the player");
                msg.SetPositiveButton("Ok", delegate
                {
                    StartActivity(typeof(RoomSelectedManageActivity));
                    Finish();
                });
                msg.Create();
                msg.Show();
            }
            catch (Exception ex)
            {
                AlertDialog.Builder connectionException = new AlertDialog.Builder(this);
                connectionException.SetTitle("Connection Error");
                connectionException.SetMessage(ex.ToString());
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
            }
        }
        private bool IsRated(int id)
        {
            try
            {
                bool ok = false;
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                string queryString2 = "select rated from UserInRoom where RoomId = @id AND UserId =@userId";
                MySqlCommand cmd2 = new MySqlCommand(queryString2, sqlconn);
                cmd2.Parameters.AddWithValue("@id", Global.selectedRoomId);
                cmd2.Parameters.AddWithValue("@userId", id);

                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (rdr.GetBoolean(0)) ok = true;
                    }
                    rdr.Close();
                }
                sqlconn.Close();
                return ok;
            }
            catch (Exception ex)
            {
                AlertDialog.Builder connectionException = new AlertDialog.Builder(this);
                connectionException.SetTitle("Connection Error");
                connectionException.SetMessage(ex.ToString());
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
                return true;
            }
        }
        private void toolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Selected Room";
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.selectedRoomManage_menu_Master, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            switch (item.TitleFormatted.ToString())
            {
                case "Joined Rooms":
                    StartActivity(typeof(JoinedRoomsActivity));
                    break;
                case "My Rooms":
                    StartActivity(typeof(MyRoomsActivity));
                    break;
                case "Browser":
                    StartActivity(typeof(BrowserActivity));
                    break;
                case "Edit Room":
                    if (thisRoom.Master1.Id == Global.activeUser.Id)
                        StartActivity(typeof(EditRoomActivity));
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