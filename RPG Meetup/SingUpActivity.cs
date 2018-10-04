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

namespace RPG_Meetup
{
    [Activity(Label = "SingUpActivity")]
    public class SingUpActivity : Activity
    {
        EditText name, password, password2, email;
        CheckBox terms;
        Button button;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUp);
            toolbar();
            name = FindViewById<EditText>(Resource.Id.txtUser);
            password = FindViewById<EditText>(Resource.Id.txtPassword);
            password2 = FindViewById<EditText>(Resource.Id.txtPassword2);
            email = FindViewById<EditText>(Resource.Id.txtEmail);
            terms = FindViewById<CheckBox>(Resource.Id.checkBox1);
            button = FindViewById<Button>(Resource.Id.button1);
            button.Click += signUpButtonClick;
        }
        private void signUpButtonClick(object sender, EventArgs args)
        {
            if(terms.Checked)
            { 
                try
                {
                    string connsqlstring = Resources.GetString(Resource.String.connection);
                    MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                    sqlconn.Open();
                    // string sql = "Insert into Room(Name, Room.System, NOplayers, Location, Room.Data, Room.Time, forFriends, mustAccept, Description)"
                    //    + "Values('test2','Młotek','4','Poznań','2018-06-13','16:35:00','0','1','Opis')";

                    string sql = "Insert into User(Username, Password, email,`Master reputation`, `Player reputation`, `Last seen`)"
                        + " Values(@name,@password,@email,'0','0',@now)";
                    MySqlCommand cmd = new MySqlCommand(sql, sqlconn);

                    cmd.Parameters.AddWithValue("@name", name.Text);
                    cmd.Parameters.AddWithValue("@password", password.Text);
                    cmd.Parameters.AddWithValue("@email", email.Text);
                    cmd.Parameters.AddWithValue("@now", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    sqlconn.Close();

                    StartActivity(typeof(LogInActivity));
                    Finish();
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
                AlertDialog.Builder msg = new AlertDialog.Builder(this);
                msg.SetTitle("Accept the Terms!");
                msg.SetMessage("Please accept the Terms and Conditions and the Privacy Policy.");
                msg.SetNegativeButton("Return", delegate { });
                msg.Create();
                msg.Show();
            }
        }
        private void toolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Sign Up:";
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