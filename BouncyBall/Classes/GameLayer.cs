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
        CCSprite ballTwoSprite;
        CCLabel levelLabel;
        CCLabel scoreLabel;
        CCLabel winnerLabel;
        CCLabel loserLabel;
        CCLabel playLabel;

        float ballXVelocity;
        float ballYVelocity;

        float ballTwoXVelocity;
        float ballTwoYVelocity;

        const float gravity = 140;
        int levelMultiplier = 1;

        int level = 1;
        int score = 0;

        bool winner;

        public GameLayer(): base(CCColor4B.Black)
        {
            ballTwoSprite = new CCSprite("ball");

            paddleSprite = new CCSprite("paddle");
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;
            AddChild(paddleSprite);

            ballSprite = new CCSprite("ball");
            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;
            AddChild(ballSprite);

            levelLabel = new CCLabel("Level: 1", "Arial", 50, CCLabelFormat.SystemFont);
            levelLabel.PositionX = 50;
            levelLabel.PositionY = 1025;
            levelLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(levelLabel);

            scoreLabel = new CCLabel("Score: 0", "Arial", 50, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 1000;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            winnerLabel = new CCLabel("You Won!", "Chalkduster", 80);
            winnerLabel.PositionX = 750;
            winnerLabel.PositionY = 150;
            winnerLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            loserLabel = new CCLabel("You Lost", "Chalkduster", 80);
            loserLabel.PositionX = 750;
            loserLabel.PositionY = 150;
            loserLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            playLabel = new CCLabel("Play Again?", "Chalkduster", 60);
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
                winner = false;
                ResetGame();
                return;
            }

            if (level == 5)
            {
                ballTwoSprite.PositionX = 320;
                ballTwoSprite.PositionY = 600;
                AddChild(ballTwoSprite);
            }

            if (doesBallOverlapPaddle && isMovingDownwards)
            {
                ballYVelocity *= -1;
                const float minXVelocity = -300;
                const float maxXVelocity = 300;

                ballXVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);

                score++;
                
                scoreLabel.Text = "Score: " + score;
                if (score / level == 2 || score == 2)                    
                {
                    level++;
                    if (level < 4)
                    {
                        levelMultiplier++;
                    }
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

            if(winner == true)
            {
                AddChild(winnerLabel);
            }
            else
            {
                AddChild(loserLabel);
            }
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
                    if (winner)
                    {     
                        levelLabel.Text = "Level: " + level;
                        RemoveChild(winnerLabel);
                    }
                    if (!winner)
                    {
                        score = 0;
                    }
                    scoreLabel.Text = "Score: " + score;
                    RemoveChild(loserLabel);
                    RemoveChild(playLabel);
                    ballXVelocity = 0;
                    ballYVelocity = 0;
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
