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
    [Activity(Label = "RoomSelectedActivity")]
    public class RoomSelectedActivity : Activity
    {
        Room thisRoom = null;
        bool activeUserInRoom = false;
        List <User> usersInRoom = new List<User>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SelectedRoom);
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
                            thisRoom = new Room(rdr.GetInt32(0),rdr.GetInt32(3),rdr.GetString(1),rdr.GetString(2),rdr.GetString(5).Split(' ')[0],rdr.GetString(4),rdr.GetString(6),rdr.GetString(9),rdr.GetBoolean(7),rdr.GetBoolean(8),"img",master);
                        }
                        rdr.Close();
                    }
                
                    string queryString2 = "select User.id,Username,`Master reputation`,`Player reputation`,`Last seen` from UserInRoom join User on UserId=User.id where accept='1' AND RoomId = @id";
                    MySqlCommand cmd2 = new MySqlCommand(queryString2, sqlconn);
                    cmd2.Parameters.AddWithValue("@id", Global.selectedRoomId);

                    using (MySqlDataReader rdr = cmd2.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            usersInRoom.Add(new User(rdr.GetInt32(0),rdr.GetString(1), rdr.GetInt32(2), rdr.GetInt32(3),rdr.GetString(4)));
                            if(Global.logIn)if (Global.activeUser.Id == usersInRoom.Last().Id) activeUserInRoom = true ;
                        }
                        rdr.Close();
                    }
                    thisRoom.CurrentPlayerNumber = usersInRoom.Count();
                    sqlconn.Close();
                }
                TextView txtName = FindViewById<TextView>(Resource.Id.txtName);
                TextView txtSystem = FindViewById<TextView>(Resource.Id.txtSystem);
                TextView txtCity = FindViewById<TextView>(Resource.Id.txtCity);
                TextView txtDate = FindViewById<TextView>(Resource.Id.txtDate);
                TextView txtDesc = FindViewById<TextView>(Resource.Id.txtDesc);
                TextView txtNOPlayers = FindViewById<TextView>(Resource.Id.txtNOPlayers);
                TextView txtMaster = FindViewById<TextView>(Resource.Id.txtMaster);

                txtName.Text = thisRoom.Name;
                txtSystem.Text = thisRoom.System;
                txtCity.Text = thisRoom.City1;
                txtDate.Text = string.Format("{0} {1}", thisRoom.Date1, thisRoom.Time1);
                txtDesc.Text = thisRoom.Desc1;
                txtNOPlayers.Text = string.Format("{0}/{1}",thisRoom.CurrentPlayerNumber ,thisRoom.NOPlayers1);
                txtMaster.Text = thisRoom.Master1.Username;
                txtMaster.Click += masterClick;

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
            Global.selectedUserId = usersInRoom[e.Position].Id;
            StartActivity(typeof(ProfileViewActivity));
        }
        private void masterClick(object sender, EventArgs args)
        {
            Global.selectedUserId = thisRoom.Master1.Id;
            StartActivity(typeof(ProfileViewActivity));
        }
        private void toolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Selected Room";
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (Global.logIn)
            {
                if(Global.activeUser.Id==thisRoom.Master1.Id)
                {
                    MenuInflater.Inflate(Resource.Menu.selectedRoom_menu_Master, menu);
                    return base.OnCreateOptionsMenu(menu);
                }
                else
                {
                    if (activeUserInRoom)
                    {
                        MenuInflater.Inflate(Resource.Menu.selectedRoom_menu_leave, menu);
                        return base.OnCreateOptionsMenu(menu);
                    }
                    else
                    {
                        MenuInflater.Inflate(Resource.Menu.selectedRoom_menu, menu);
                        return base.OnCreateOptionsMenu(menu);
                    }
                }
            }
            else
            {
                MenuInflater.Inflate(Resource.Menu.selectedRoom_menu_guest, menu);
                return base.OnCreateOptionsMenu(menu);
            }
        }
        private void leaveFromRoom()
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                string sql2 = "DELETE FROM UserInRoom WHERE RoomId=@roomId AND UserId=@userId";
                MySqlCommand cmd2 = new MySqlCommand(sql2, sqlconn);
                cmd2.Parameters.AddWithValue("@roomId", Global.selectedRoomId);
                cmd2.Parameters.AddWithValue("@userId", Global.activeUser.Id);
                cmd2.ExecuteNonQuery();

                sqlconn.Close();
                AlertDialog.Builder msg = new AlertDialog.Builder(this);
                msg.SetTitle("Operation done!");
                msg.SetMessage("You have successfully left the room");
                msg.SetPositiveButton("Ok", delegate 
                {
                    StartActivity(typeof(RoomSelectedActivity));
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
        private void joinToRoom()
        {
            if(thisRoom.Master1.Id != Global.activeUser.Id)
            {
                try
                {
                    string connsqlstring = Resources.GetString(Resource.String.connection);
                    MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                    sqlconn.Open();
                    string sql;
                    if (thisRoom.MustAccept) sql = "Insert into UserInRoom(UserId, RoomId,accept) Values(@userId,@roomId,0)";
                    else sql = "Insert into UserInRoom(UserId, RoomId,accept) Values(@userId,@roomId,1)";

                    MySqlCommand cmd = new MySqlCommand(sql, sqlconn);

                    cmd.Parameters.AddWithValue("@UserId", Global.activeUser.Id);
                    cmd.Parameters.AddWithValue("@RoomId", Global.selectedRoomId);

                    cmd.ExecuteNonQuery();
                    sqlconn.Close();
                    AlertDialog.Builder msg = new AlertDialog.Builder(this);
                    msg.SetTitle("Operation done!");
                    if(thisRoom.MustAccept) msg.SetMessage("You have successfully joined the room, but you have to wait for acceptance.");
                    else msg.SetMessage("You have successfully joined the room.");
                    msg.SetPositiveButton("Ok", delegate
                    {
                        StartActivity(typeof(RoomSelectedActivity));
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
            else
            {
                AlertDialog.Builder connectionException = new AlertDialog.Builder(this);
                connectionException.SetTitle("Join error!");
                connectionException.SetMessage("Cannot join to room as player when you are master.");
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
            }
        }
        private void rateMaster()
        {
            AlertDialog.Builder msg2 = new AlertDialog.Builder(this);
            msg2.SetTitle("Rating");
            if (IsRated(Global.activeUser.Id))
            {
                msg2.SetMessage("Cannot rate rated game master.");
                msg2.SetNegativeButton("return", delegate { });
            }
            else
            {
                msg2.SetMessage("As a player, how do you rate this game master?");
                msg2.SetPositiveButton("Well game master", delegate
                {
                    Rate(thisRoom.Master1.Id,1);
                });
                msg2.SetNegativeButton("Bad game master", delegate
                {
                    Rate(thisRoom.Master1.Id,-1);
                });
            }

            msg2.Create();
            msg2.Show();
        }
        private void Rate(int id, int rate)
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                string sql2 = "UPDATE User Set `Master reputation`=`Master reputation`+@rate WHERE Id=@userId";
                MySqlCommand cmd2 = new MySqlCommand(sql2, sqlconn);
                cmd2.Parameters.AddWithValue("@userId", id);
                cmd2.Parameters.AddWithValue("@rate", rate);
                cmd2.ExecuteNonQuery();

                string queryString = "update UserInRoom set GiveRate='1' where RoomId = @id AND UserId =@userId";
                MySqlCommand cmd = new MySqlCommand(queryString, sqlconn);
                cmd.Parameters.AddWithValue("@id", Global.selectedRoomId);
                cmd.Parameters.AddWithValue("@userId", Global.activeUser.Id);
                cmd.ExecuteNonQuery();

                sqlconn.Close();
                AlertDialog.Builder msg = new AlertDialog.Builder(this);
                msg.SetTitle("Operation done!");
                msg.SetMessage("You have successfully rated the game master");
                msg.SetPositiveButton("Ok", delegate
                {
                    StartActivity(typeof(RoomSelectedActivity));
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

                string queryString2 = "select GiveRate from UserInRoom where RoomId = @id AND UserId =@userId";
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
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            switch (item.TitleFormatted.ToString())
            {
                case "Rate master!":
                    rateMaster();
                    break;
                case "Joined Rooms":
                    StartActivity(typeof(JoinedRoomsActivity));
                    break;
                case "My Rooms":
                    StartActivity(typeof(MyRoomsActivity));
                    break;
                case "Browser":
                    StartActivity(typeof(BrowserActivity));
                    break;
                case "Join!":
                    joinToRoom();
                    break;
                case "Manage":
                    StartActivity(typeof(RoomSelectedManageActivity));
                    break;
                case "Leave!":
                    leaveFromRoom();
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