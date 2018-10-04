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
    [Activity(Label = "CreateRoomActivity")]
    public class CreateRoomActivity : Activity
    {
        Button createButton;
        Spinner spinner;
        EditText name, NOplayer, location, desc;
        DatePicker dp;
        TimePicker tp;
        string system = "All Flesh Must Be Eaten";
        Switch mustAccept, forFriends;
        string forFriendsString = "0";
        string mustAcceptString = "0";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CreateRoom);
            toolbar();
            CreateRoom();
        }
        private void createButton_Click(object sender, EventArgs args)
        {
            try
            {
                string connsqlstring = Resources.GetString(Resource.String.connection);
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();
                
                string sql = "Insert into Room(Name, Room.System, NOplayers, Location, Room.Data, Room.Time, forFriends, mustAccept, Description,MasterId)"
                    + "Values(@name,@system,@NOplayers,@location,@data,@time,@forFriends,@mustAccept,@desc,@masterid)";
                MySqlCommand cmd = new MySqlCommand(sql, sqlconn);
                
                cmd.Parameters.AddWithValue("@name", name.Text);
                cmd.Parameters.AddWithValue("@system", system);
                cmd.Parameters.AddWithValue("@NOplayers", Int32.Parse(NOplayer.Text));
                cmd.Parameters.AddWithValue("@location", location.Text);
                string data = string.Format("{0}-{1}-{2}", dp.Year.ToString(), (dp.Month+1).ToString(), dp.DayOfMonth.ToString());
                cmd.Parameters.AddWithValue("@data", data);
                string time = string.Format("{0}:{1}:00", tp.Hour.ToString(), tp.Minute.ToString());
                cmd.Parameters.AddWithValue("@time", time);
                cmd.Parameters.AddWithValue("@forFriends", forFriendsString);
                cmd.Parameters.AddWithValue("@mustAccept", mustAcceptString);
                cmd.Parameters.AddWithValue("@desc", desc.Text);
                cmd.Parameters.AddWithValue("@masterid", Global.activeUser.Id);

                cmd.ExecuteNonQuery();
                sqlconn.Close();

                StartActivity(typeof(BrowserActivity));
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
        private void CreateRoom()
        {
            createButton = FindViewById<Button>(Resource.Id.button1);
            createButton.Click += createButton_Click;
            name = FindViewById<EditText>(Resource.Id.editText1);
            NOplayer = FindViewById<EditText>(Resource.Id.editText2);
            location = FindViewById<EditText>(Resource.Id.locationText);
            desc = FindViewById<EditText>(Resource.Id.editText3);
            dp = FindViewById<DatePicker>(Resource.Id.datePicker1);
            tp = FindViewById<TimePicker>(Resource.Id.timePicker1);
            mustAccept = FindViewById<Switch>(Resource.Id.switch1);
            forFriends = FindViewById<Switch>(Resource.Id.switch2);
            spinner = FindViewById<Spinner>(Resource.Id.spinner1);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.systems_array, Resource.Layout.spinner_item);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            spinner.Adapter = adapter;
            forFriends.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) {
                forFriendsString = e.IsChecked ? "1" : "0";
            };
            mustAccept.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) {
                mustAcceptString = e.IsChecked ? "1" : "0";
            };
        }
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("The system is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
            system = spinner.GetItemAtPosition(e.Position).ToString();
        }
        private void toolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Create new game";
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