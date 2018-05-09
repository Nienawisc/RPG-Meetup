using Android.App;
using Android.Widget;
using Android.OS;
using System;

namespace RPG_Meetup
{
    [Activity(Label = "RPG_Meetup", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Button login, registration, guest;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.CreateRoom);
            CreateRoom();

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
    }
}

