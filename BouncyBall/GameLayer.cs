using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncyBall
{
    public class GameLayer : CCLayerColor
    {
        CCSprite paddleSprite;
        CCSprite ballSprite;
        CCLabel scoreLabel;
        CCLabel playLabel;
        CCLabel winnerLabel;
        CCLabel loseLabel;

        float ballXVelocity;
        float ballYVelocity;
        bool winner = false;


        const float GRAVITY = 140;
        int levelMultiplier = 1;

        int score = 0;
        int level;

        public GameLayer() : base(CCColor4B.Black)
        {
            paddleSprite = new CCSprite("paddle");
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;
            AddChild(paddleSprite);

            ballSprite = new CCSprite("ball");
            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;
            AddChild(ballSprite);

            scoreLabel = new CCLabel("Score: 0", "Ariel", 70, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 1000;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            loseLabel = new CCLabel("You lose!", "ChalkDuster", 100);
            loseLabel.AnchorPoint = CCPoint.AnchorLowerLeft;

            playLabel = new CCLabel("Play Again?", "Chalkduster", 70);
            playLabel.PositionX = 750;
            playLabel.PositionY = 100;
            playLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            winnerLabel = new CCLabel("WINNER!", "Chalkduster", 100);
            winnerLabel.AnchorPoint = CCPoint.AnchorLowerLeft;

            Schedule(RunGameLogic);
        }

        private void RunGameLogic(float frameTimeInSeconds)
        {
            ballYVelocity += frameTimeInSeconds * (-GRAVITY * levelMultiplier);
            ballSprite.PositionX += ballXVelocity * frameTimeInSeconds;
            ballSprite.PositionY += ballYVelocity * frameTimeInSeconds;

            bool doesBallOverlapPaddle = ballSprite.BoundingBoxTransformedToParent.IntersectsRect(paddleSprite.BoundingBoxTransformedToParent);
            bool isMovingDownward = ballYVelocity < 0;
            bool isBallBelowPaddle = ballSprite.BoundingBoxTransformedToParent.MaxY < paddleSprite.BoundingBoxTransformedToParent.MinY;

            if (isBallBelowPaddle)
            {
                AddChild(playLabel);
                AddChild(loseLabel);
                CreateTouchListener();
                return;
            }

            if (score > 20)
            {
                level++;
            }

            if (doesBallOverlapPaddle && isMovingDownward)
            {
                ballYVelocity *= -1;
                const float minXVelocity = -300;
                const float maxXVelocity = 300;
                ballXVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);
                score++;
                scoreLabel.Text = "Score: " + score;
                if (score > 2)
                {
                    winner = true;
                    AddChild(winnerLabel);
                    ResetGame();
                }
            }

            float ballRight = ballSprite.BoundingBoxTransformedToParent.MaxX;
            float ballLeft = ballSprite.BoundingBoxTransformedToParent.MinX;

            float screenRight = VisibleBoundsWorldspace.MaxX;
            float screenLeft = VisibleBoundsWorldspace.MinX;

            bool shouldReflectXVelocity = (ballRight > screenRight && ballXVelocity > 0) || (ballLeft < screenLeft && ballXVelocity < 0);
            if (shouldReflectXVelocity)
            {
                ballXVelocity *= -1;
            }
        }

        private void ResetGame()
        {
            StopAllActions();
            Unschedule(RunGameLogic);
            scoreLabel.Text = "Score: ";
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;

            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;

            RemoveChild(winnerLabel);
            RemoveChild(loseLabel);
            AddChild(playLabel);
            CreateTouchListener();
        }

        private void CreateTouchListener()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = TouchesBegan;
            AddEventListener(touchListener);
        }

        private void TouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            foreach (var touch in touches)
            {
                if (playLabel.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    score = 0;
                    levelMultiplier++;
                    Schedule(RunGameLogic);
                    RemoveChild(playLabel);
                    winner = false;
                }
            }
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();
            CCRect bounds = VisibleBoundsWorldspace;
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            touchListener.OnTouchesMoved = HandleTouchesMoved;
            AddEventListener(touchListener, this);
        }

        private void HandleTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            var locationOnScreen = touches[0].Location;
            paddleSprite.PositionX = locationOnScreen.X;
        }

        private void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {

        }
    }
}
