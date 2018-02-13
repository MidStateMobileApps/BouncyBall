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
        CCLabel messageLabel;
        CCLabel levelLabel;

        float ballXVelocity;
        float ballYVelocity;

        const float gravity = 140;
        int levelMultiplier = 1;
        int levelScore;
        int loseCounter = 0;
        bool winner = false;

        int score = 0;

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

            scoreLabel = new CCLabel("Score: 0", "Arial", 70, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 1000;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            playLabel = new CCLabel("Play Again?", "Chalkduster", 70);
            playLabel.PositionX = 750;
            playLabel.PositionY = 100;
            playLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            messageLabel = new CCLabel("", "Arial", 150);
            messageLabel.PositionX = 400;
            messageLabel.PositionY = 600;
            messageLabel.AnchorPoint = CCPoint.AnchorMiddle;

            levelLabel = new CCLabel("Level:" + levelMultiplier, "Chalkduster", 70);
            levelLabel.PositionX = 750;
            levelLabel.PositionY = 1000;
            levelLabel.AnchorPoint = CCPoint.AnchorUpperRight;
            AddChild(levelLabel);

            Schedule(RunGameLogic);
            
        }

        private void RunGameLogic(float frameTimeInSeconds)
        {
            ballYVelocity += frameTimeInSeconds * (-gravity * levelMultiplier);
            ballSprite.PositionX += ballXVelocity * frameTimeInSeconds;
            ballSprite.PositionY += ballYVelocity * frameTimeInSeconds;

            bool doesBallOverlapPaddle = 
                ballSprite.BoundingBoxTransformedToParent.
                IntersectsRect(paddleSprite.BoundingBoxTransformedToParent);
            bool isMovingDownward = ballYVelocity < 0;
            bool isBallBelowPaddle = ballSprite.BoundingBoxTransformedToParent.MaxY <
                            paddleSprite.BoundingBoxTransformedToParent.MinY;
            if ( isBallBelowPaddle )
            {
                if(loseCounter == 3)
                {
                    messageLabel.Text = "Game Over";
                    loseCounter = 0;
                }
                else
                {
                    messageLabel.Text = "You lose!";
                    loseCounter++;
                    score = levelScore;
                }
                AddChild(messageLabel);
                ResetGame();
                return;
            }
            if (doesBallOverlapPaddle && isMovingDownward)
            {
                ballYVelocity *= -1;
                const float minXVelocity = -300;
                const float maxXVelocity = 300;
                ballXVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);
                score++;
                scoreLabel.Text = "Score: " + score;
                if ( score == 20 || (score - levelScore) == 20)
                {
                    levelMultiplier++;
                    messageLabel.Text = "You Win!";
                    levelLabel.Text = "Level: " + levelMultiplier;
                    levelScore = score;
                    loseCounter = 0;
                    winner = true;
                    ResetGame();
                }
            }
            float ballRight = ballSprite.BoundingBoxTransformedToParent.MaxX;
            float ballLeft = ballSprite.BoundingBoxTransformedToParent.MinX;

            float screenRight = VisibleBoundsWorldspace.MaxX;
            float screenLeft = VisibleBoundsWorldspace.MinX;

            bool shouldReflectXVelocity = (ballRight > screenRight && ballXVelocity > 0) ||
                                        (ballLeft < screenLeft && ballXVelocity < 0);
            if (shouldReflectXVelocity)
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

            if (loseCounter == 3)
            {
                levelMultiplier = 1;
                levelLabel.Text = "Level: " + levelMultiplier;
                score = 0;
                levelScore = 0;
            }

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
            foreach(var touch in touches)
            {
                if (playLabel.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    scoreLabel.Text = "Score: " + score.ToString();
                        Schedule(RunGameLogic);
                        RemoveChild(playLabel);
                        RemoveChild(messageLabel);
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
