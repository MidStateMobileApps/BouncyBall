using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CocosSharp;

namespace BouncyBall
{
    public class GameLayer : CCLayerColor
    {
        CCSprite paddleSprite;
        CCSprite ballSprite;
        CCLabel scoreLabel;
        CCLabel playLabel;

        float ballXVelocity;
        float ballYVelocity;

        const float gravity = 140;
        int levelMultiplier = 1;

        int score = 0;

        public GameLayer(): base(CCColor4B.Black)
        {
            paddleSprite = new CCSprite("paddle");
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;
            AddChild(paddleSprite);

            ballSprite = new CCSprite("ball");
            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;
            AddChild(ballSprite);
        }

        private void LoadGame(object sender, System.EventArgs e)
        {
            CCGameView gameView = sender as CCGameView;
            if (gameView != null)
            {
                var contentSearchPaths = new List<string>() { "Fonts", "Sounds" };
                CCSizeI viewSize = gameView.ViewSize;
                int w = 768;
                int h = 1024;

                gameView.DesignResolution = new CCSizeI(w, h);

                if (w < viewSize.Width)
                {
                    contentSearchPaths.Add("Images/Hd");
                    CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
                }
                else
                {
                    contentSearchPaths.Add("Images/Ld");
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
