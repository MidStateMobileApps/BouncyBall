using Android.App;
using Android.Widget;
using System.Collections.Generic;
using Android.OS;
using Android;
using CocosSharp;
using OpenTK;

namespace BouncyBall.Droid
{
    [Activity(Label = "BouncyBall", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
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
            CCGameView gameView = sender as CCGameView;
            if (gameView != null)
            {
                var contentSearchPath = new List<string>() {"Fonts", "Sounds" };
                CCSizeI viewSize = gameView.ViewSize;
                int w = 768;
                int h = 1024;

                gameView.DesignResolution = new CCSizeI(w, h);

                if (w < viewSize.Width)
                {
                    contentSearchPath.Add("images/Hd");
                    CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
                }
                else
                {
                    contentSearchPath.Add("images/Ld");
                    CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
                }

                gameView.ContentManager.SearchPaths = contentSearchPath;
                CCScene gameScene = new CCScene(gameView);

                gameScene.AddLayer(new GameLayer());
                gameView.RunWithScene(gameScene);
            }
        }
    }
}

