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

        bool winner;

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

            scoreLabel = new CCLabel("Score: 0", "Arial", 50, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 1000;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            playLabel = new CCLabel("Play Again?", "Chalkduster", 70);
            playLabel.PositionX = 750;
            playLabel.PositionY = 100;
            playLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            Schedule(RunGameLogic);
        }

        private void RunGameLogic(float frameTimeInSeconds)
        {
            ballYVelocity += frameTimeInSeconds * (-gravity * levelMultiplier);
            ballSprite.PositionX += ballXVelocity * frameTimeInSeconds;
            ballSprite.PositionY += ballYVelocity * frameTimeInSeconds;

            bool doesBallOverlapPaddle = ballSprite.BoundingBoxTransformedToParent.IntersectsRect(paddleSprite.BoundingBoxTransformedToParent);

            bool isMovingDownwards = ballYVelocity < 0;
            bool isBallBelowPaddle = ballSprite.BoundingBoxTransformedToParent.MaxY < paddleSprite.BoundingBoxTransformedToParent.MinY;

            if (isBallBelowPaddle)
            {
                ResetGame();
                return;
            }

            if (doesBallOverlapPaddle && isMovingDownwards)
            {
                ballYVelocity *= -1;
                const float minXVelocity = -300;
                const float maxXVelocity = 300;

                ballXVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);

                score++;
                
                scoreLabel.Text = "Score: " + score;
                if (score > 20)
                {
                    winner = true;
                    ResetGame();
                }
            }

            float ballRight = ballSprite.BoundingBoxTransformedToParent.MaxX;
            float ballLeft = ballSprite.BoundingBoxTransformedToParent.MinX;

            float screenRight = VisibleBoundsWorldspace.MaxX;
            float screenLeft = VisibleBoundsWorldspace.MinX;

            bool shouldReflexXVelocity = (ballRight > screenRight && ballXVelocity > 0)  || (ballLeft < screenLeft && ballXVelocity < 0);
            
            if (shouldReflexXVelocity)
            {
                ballXVelocity *= -1;
            }

            

        }

        private void ResetGame()
        {
            StopAllActions();
            Unschedule(RunGameLogic);
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;
            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;

            AddChild(playLabel);
            CreateTouchListener();
        }

        private void CreateTouchListener()
        {
            var TouchListener = new CCEventListenerTouchAllAtOnce();
            TouchListener.OnTouchesBegan = TouchesBegan;
            AddEventListener(TouchListener);
        }

        private void TouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            foreach (var touch in touches)
            {
                if (playLabel.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    score = 0;
                    if (winner)
                    {
                        levelMultiplier++;
                    }
                    RemoveChild(playLabel);
                    Schedule(RunGameLogic);
                }
            }
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();
            CCRect bounds = VisibleBoundsWorldspace;

            var touchListener = new CCEventListenerTouchAllAtOnce();

            touchListener.OnTouchesEnded = OnTouchesEnded;
            touchListener.OnTouchesMoved = HandlesTouchesMoved;

            AddEventListener(touchListener, this);
        }

        private void HandlesTouchesMoved(List<CCTouch> Touches, CCEvent TouchEvent)
        {
            var locationOnScreen = Touches[0].Location;
            paddleSprite.PositionX = locationOnScreen.X;
        }

        private void OnTouchesEnded(List<CCTouch> Touches, CCEvent TouchEvent)
        {

        }
    }
}
