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
using System.Collections.Generic;
using System.Data;

namespace RPG_Meetup
{
    [Activity(Label = "RPG_Meetup")]
    public class BrowserActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Browser);
            BrowserStart();
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