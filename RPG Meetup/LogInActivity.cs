using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using MySql.Data.MySqlClient;
using System.Data;

namespace RPG_Meetup
{
    [Activity(Label = "LogInActivity")]
    public class LogInActivity : Activity
    {
        Button button;
        Button forgot;
        EditText login, password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LogIn);
            toolbar();
            button = FindViewById<Button>(Resource.Id.button1);
            button.Click += button_Click;
            login = FindViewById<EditText>(Resource.Id.editText1);
            password = FindViewById<EditText>(Resource.Id.editText2);
            forgot = FindViewById<Button>(Resource.Id.button2);
            forgot.Click += forgot_Click;
            // Create your application here
        }

        private void forgot_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ForgotYourPasswordActivity));
        }

        private void button_Click(object sender, EventArgs args)
        {
            try
            {
                using (MySqlConnection sqlconn = new MySqlConnection(Resources.GetString(Resource.String.connection)))
                { 
                    sqlconn.Open();
                    string queryString = "select * from User where Username=@username";
                    MySqlCommand cmd = new MySqlCommand(queryString, sqlconn);
                    cmd.Parameters.AddWithValue("@username", login.Text);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            if (rdr.GetString(2) == password.Text)
                            {
                                Global.activeUser = new User (rdr.GetInt32(0),rdr.GetString(1),rdr.GetInt32(3),rdr.GetInt32(4),rdr.GetString(5));
                                Global.logIn = true;
                            }
                            else
                            {
                                AlertDialog.Builder msg = new AlertDialog.Builder(this);
                                msg.SetTitle("Incorrect password or username");
                                msg.SetMessage("Username or password is incorrect");
                                msg.SetNegativeButton("Try again", delegate { });
                                msg.Create();
                                msg.Show();
                            }
                        }
                        rdr.Close();
                    }
                    if(Global.logIn)
                    {
                        string query2 = "UPDATE User SET `Last Seen` = DATE_ADD(NOW(), INTERVAL 2 HOUR) WHERE Id=@id";
                        MySqlCommand cmd2 = new MySqlCommand(query2, sqlconn);
                        cmd2.Parameters.AddWithValue("@Id", Global.activeUser.Id);
                        cmd2.ExecuteNonQuery();
                    }
                    sqlconn.Close();
                }
                if(Global.logIn)
                {
                    StartActivity(typeof(BrowserActivity));
                    Finish();
                }
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
        private void toolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Log In:";
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }
    }
}