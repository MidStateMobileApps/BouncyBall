using Android.App;
using Android.Widget;
using Android.OS;
using CocosSharp;
using System;

namespace BouncyBall.Droid
{
    [Activity(Label = "BouncyBall", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            CCGameView gameView = (CCGameView)FindViewById(Resource.Id.GameView);
            gameView.ViewCreated += LoadGame;

            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.myButton);

            //button.Click += delegate { button.Text = $"{count++} clicks!"; };
        }

        private void LoadGame(object sender, EventArgs e)
        {
            CCGameView gameView = sender as CCGameView;
        }
    }
}

