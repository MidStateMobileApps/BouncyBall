using Android.App;
using Android.Widget;
using Android.OS;
using Android;
using CocosSharp;
using System.Collections.Generic;

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
            // Button button = FindViewById<Button>(Resource.Id.myButton);

            // button.Click += delegate { button.Text = $"{count++} clicks!"; };
        }

        private void LoadGame(object sender, System.EventArgs e)
        {
            CCGameView gameView = sender as CCGameView;
            if ( gameView != null)
            {
                var contentSearchPaths = new List<string>() { "Fonts", "Sounds" };
                CCSizeI viewSize = gameView.ViewSize;
                int w = 768;
                int h = 1024;

                gameView.DesignResolution = new CCSizeI(w, h);
                if ( w < viewSize.Width)
                {
                    contentSearchPaths.Add("Images/Hd");
                    CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
                }
                else
                {
                    contentSearchPaths.Add("Images/Hd");
                    CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
                }

                gameView.ContentManager.SearchPaths = contentSearchPaths;
                CCScene gameScene = new CCScene(gameView);

                gameScene.AddLayer(new GameLayer());
                gameView.RunWithScene(gameScene);
            }
        }
    }
}

