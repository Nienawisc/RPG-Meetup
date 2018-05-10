using Android.App;
using Android.Widget;
using Android.OS;
using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace RPG_Meetup
{
    [Activity(Label = "RPG_Meetup", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private int menu = 0;
        private Button login, registration, guest;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Browser);
            //CreateRoom();
            //Start();
            BrowserStart();

        }
        private void Click(object sender, EventArgs args)
        {
            guest.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff669900"));


            guest.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff99cc00"));
        }
        private void Start()
        {
            login = FindViewById<Button>(Resource.Id.button1);
            registration = FindViewById<Button>(Resource.Id.button2);
            guest = FindViewById<Button>(Resource.Id.button3);

            guest.Click += Click;
        }
        private void CreateRoom()
        {
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner1);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.systems_array, Resource.Layout.spinner_item );

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            spinner.Adapter = adapter;
        }
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("The system is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
        private void BrowserStart()
        {
            List<DataRow> rooms = new List<DataRow>();
            try
            {
                string connsqlstring = "Server=db4free.net;Port=3306;database=rpg_meetup;User Id=gamemaster001;Password=slonie01;charset=utf8";
                MySqlConnection sqlconn = new MySqlConnection(connsqlstring);
                sqlconn.Open();

                DataSet tickets = new DataSet();
                string queryString = "select * from Room";
                MySqlDataAdapter adapter = new MySqlDataAdapter(queryString, sqlconn);
                adapter.Fill(tickets, "Item");

                foreach (DataRow row in tickets.Tables["Item"].Rows)
                {
                    rooms.Add(row);
                }
                sqlconn.Close();
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
            TextView name = FindViewById<TextView>(Resource.Id.Name);
            TextView system = FindViewById<TextView>(Resource.Id.System);
            TextView NOplayer = FindViewById<TextView>(Resource.Id.NOplayers);

            name.Text = "Name:" + rooms[0][1].ToString();
            system.Text = "System:" + rooms[0][2].ToString();
            NOplayer.Text = "Players: 0/" + rooms[0][3].ToString();
        }
    }
}

