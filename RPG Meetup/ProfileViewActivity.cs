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

using Android.Support.V7.App;
using Android.Gms.Ads;


namespace RPG_Meetup
{
    [Activity(Label = "ProfileViewActivity")]
    public class ProfileViewActivity : Activity
    {
        private User thisUser;
        protected AdView mAdView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProfileView);
            toolbar();
            TextView txtName = FindViewById<TextView>(Resource.Id.txtName);
            TextView txtMRep = FindViewById<TextView>(Resource.Id.txtMRep);
            TextView txtPRep = FindViewById<TextView>(Resource.Id.txtPRep);
            TextView txtSeen = FindViewById<TextView>(Resource.Id.txtSeen);
                try
                {
                    using (MySqlConnection sqlconn = new MySqlConnection(Resources.GetString(Resource.String.connection)))
                    {
                        sqlconn.Open();
                        string queryString = "select * from User where id=@id";
                        MySqlCommand cmd = new MySqlCommand(queryString, sqlconn);
                        cmd.Parameters.AddWithValue("@id", Global.selectedUserId);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                thisUser = new User(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(3),rdr.GetInt32(4),rdr.GetString(5));
                            }
                            rdr.Close();
                        }
                        if(Global.logIn)
                    { 
                        string queryString2 = "select accepted from Friends where UserId1=@id AND UserId2=@id2";
                        MySqlCommand cmd2 = new MySqlCommand(queryString2, sqlconn);
                        cmd2.Parameters.AddWithValue("@id", Global.selectedUserId);
                        cmd2.Parameters.AddWithValue("@id2", Global.activeUser.Id);

                        using (MySqlDataReader rdr = cmd2.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                            thisUser.Friend = Convert.ToInt32(rdr.GetBoolean(0));
                            }
                            rdr.Close();
                        }
                    string queryString3 = "select accepted from Friends where UserId1=@id2 AND UserId2=@id";
                    MySqlCommand cmd3 = new MySqlCommand(queryString3, sqlconn);
                    cmd3.Parameters.AddWithValue("@id", Global.selectedUserId);
                    cmd3.Parameters.AddWithValue("@id2", Global.activeUser.Id);

                    using (MySqlDataReader rdr = cmd3.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            thisUser.Friend = Convert.ToInt32(rdr.GetBoolean(0));
                        }
                        rdr.Close();
                    }
                    sqlconn.Close();
                    }
                }
                    txtName.Text = thisUser.Username;
                    txtMRep.Text = thisUser.MasterRep.ToString();
                    txtPRep.Text = thisUser.PlayerRep.ToString();
                    txtSeen.Text = thisUser.LastSeen.ToString();
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
            MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_aplication_id));
            mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);
            // Create your application here
        }
        private void toolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Profil";
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (Global.logIn)
            {
                if(Global.activeUser.Id == thisUser.Id)
                {
                    MenuInflater.Inflate(Resource.Menu.Profile_menuOwn, menu);
                    return base.OnCreateOptionsMenu(menu);
                }
                else
                {
                    if(thisUser.Friend==-1)
                    {
                        MenuInflater.Inflate(Resource.Menu.Profile_menu, menu);
                        return base.OnCreateOptionsMenu(menu);
                    }
                    else
                    {
                        if (thisUser.Friend == 0)
                        {
                            MenuInflater.Inflate(Resource.Menu.Profile_menuInvited, menu);
                            return base.OnCreateOptionsMenu(menu);
                        }
                        else
                        {
                            MenuInflater.Inflate(Resource.Menu.Profile_menuKick, menu);
                            return base.OnCreateOptionsMenu(menu);
                        }
                    }
                }
            }
            else
            {
                MenuInflater.Inflate(Resource.Menu.Profile_menu_guest, menu);
                return base.OnCreateOptionsMenu(menu);
            }
        }
        void kickFriend()
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();
                string sql = "DELETE FROM Friends WHERE UserId1=@u1 AND UserId2=@u2";
                MySqlCommand cmd = new MySqlCommand(sql, sqlconn);
                cmd.Parameters.AddWithValue("@u1", Global.activeUser.Id);
                cmd.Parameters.AddWithValue("@u2", thisUser.Id);
                cmd.ExecuteNonQuery();
                string sql2 = "DELETE FROM Friends WHERE UserId1=@u2 AND UserId2=@u1";
                MySqlCommand cmd2 = new MySqlCommand(sql2, sqlconn);
                cmd2.Parameters.AddWithValue("@u1", Global.activeUser.Id);
                cmd2.Parameters.AddWithValue("@u2", thisUser.Id);
                cmd2.ExecuteNonQuery();

                sqlconn.Close();
                Android.App.AlertDialog.Builder msg = new Android.App.AlertDialog.Builder(this);
                msg.SetTitle("Operation done!");
                msg.SetMessage("You have successfully delete friend.");
                msg.SetPositiveButton("Ok", delegate
                {
                    StartActivity(typeof(ProfileViewActivity));
                    Finish();
                });
                msg.Create();
                msg.Show();
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
        void inviteFriend()
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();
                string sql = "Insert into Friends(UserId1, UserId2) Values(@userId1,@userId2)";

                MySqlCommand cmd = new MySqlCommand(sql, sqlconn);

                cmd.Parameters.AddWithValue("@userId1", Global.activeUser.Id);
                cmd.Parameters.AddWithValue("@userId2", thisUser.Id);

                cmd.ExecuteNonQuery();
                sqlconn.Close();
                Android.App.AlertDialog.Builder msg = new Android.App.AlertDialog.Builder(this);
                msg.SetTitle("Operation done!");
                msg.SetMessage("You have successfully invited friend.");
                msg.SetPositiveButton("Ok", delegate
                {
                    StartActivity(typeof(ProfileViewActivity));
                    Finish();
                });
                msg.Create();
                msg.Show();
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
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            switch (item.TitleFormatted.ToString())
            {
                case "Kick friend":
                case "Cancel invite":
                    kickFriend();
                    break;
                case "Invite friend":
                    inviteFriend();
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
                case "Edit":
                    if (Global.selectedUserId == Global.activeUser.Id)
                        StartActivity(typeof(EditProfileActivity));
                    else
                    {
                        Android.App.AlertDialog.Builder msg = new Android.App.AlertDialog.Builder(this);
                        msg.SetTitle("Access Denied");
                        msg.SetMessage("You can't edit this profile");
                        msg.SetNegativeButton("Ok", delegate { });
                        msg.Create();
                        msg.Show();
                    }
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