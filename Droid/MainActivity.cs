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
            
        }

        private void LoadGame(object sender, System.EventArgs e)
        {
            CCGameView gameview = sender as CCGameView;
            if(gameview != null)
            {
                var contentSearchPaths = new List<string>() { "Fonts", "Sounds" };
                CCSize viewsize = gameview.ViewSize;
                int w = 768;
                int h = 1024;

                gameview.DesignResolution = new CCSizeI(w, h);

                if(w < viewsize.Width)
                {
                    contentSearchPaths.Add("Images/Hd");
                    CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
                }
                else
                {
                    contentSearchPaths.Add("Images/ld");
                    CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
                }

                gameview.ContentManager.SearchPaths = contentSearchPaths;

                CCScene gameScene = new CCScene(gameview);

                gameScene.AddLayer(new GameLayer());
                gameview.RunWithScene(gameScene);
            }
        }
    }
}

