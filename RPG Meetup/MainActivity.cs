using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using Android.Support.V7.App;
using Android.Gms.Ads;

namespace RPG_Meetup
{
    [Activity(Label = "RPG_Meetup", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected AdView mAdView;

        private Button login, registration, guest;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_aplication_id));
            toolbar();

            mAdView = FindViewById<AdView>(Resource.Id.adView);
           // mAdView.AdSize = AdSize.SmartBanner;
           // mAdView.AdUnitId = GetString(Resource.String.banner_ad_unit_id);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            if (Global.logIn)
            {
                StartActivity(typeof(BrowserActivity));
                Finish();
            }
            else
            {
                Start();
            }
        }
        private void Guest_Click(object sender, EventArgs args)
        {
            guest.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff669900"));
            guest.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ff99cc00"));
            StartActivity(typeof(BrowserActivity));
            Finish();
        }
        private void LogIn_Click(object sender, EventArgs args)
        {
            StartActivity(typeof(LogInActivity));
            Finish();
        }
        private void SingUp_Click(object sender, EventArgs args)
        {
            StartActivity(typeof(SingUpActivity));
            Finish();
        }
        private void Start()
        {
            login = FindViewById<Button>(Resource.Id.button1);
            registration = FindViewById<Button>(Resource.Id.button2);
            guest = FindViewById<Button>(Resource.Id.button3);

            guest.Click += Guest_Click;
            login.Click += LogIn_Click;
            registration.Click += SingUp_Click;
        }
        private void toolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "RPG Meetup";
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

        protected override void OnPause()
        {
            if (mAdView != null)
            {
                mAdView.Pause();
            }
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (mAdView != null)
            {
                mAdView.Resume();
            }
        }

        protected override void OnDestroy()
        {
            if (mAdView != null)
            {
                mAdView.Destroy();
            }
            base.OnDestroy();
        }
    }
}

