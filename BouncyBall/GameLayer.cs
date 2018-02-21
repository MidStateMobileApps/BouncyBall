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
        CCLabel levelLabel;
        CCLabel playLabel;
        CCLabel winnerLabel;
        CCLabel loserLabel;
        CCLabel gameOverLabel;


        bool winner = false;
        bool lost = false;
        bool gameOver = false;

        float ballXVelocity;
        float ballYVelocity;

        const float Gravity = 100;
        int levelMultiplier = 1;

        int level = 1;
        int score = 0;
        int losesCount = 0;

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

            scoreLabel = new CCLabel("Score: 0", "Arial", 60, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 975;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            playLabel = new CCLabel("Play Again?", "Chalkduster", 70);
            playLabel.PositionX = 750;
            playLabel.PositionY = 100;
            playLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            levelLabel = new CCLabel("Level: 1", "Arial", 60, CCLabelFormat.SystemFont);
            levelLabel.PositionX = 50;
            levelLabel.PositionY = 1025;
            levelLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(levelLabel);

            winnerLabel = new CCLabel("Winner Winner!", "Chalkduster", 80);
            winnerLabel.PositionX = 750;
            winnerLabel.PositionY = 150;
            winnerLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            loserLabel = new CCLabel("You're Dead", "Chalkduster", 80);
            loserLabel.PositionX = 750;
            loserLabel.PositionY = 150;
            loserLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            gameOverLabel = new CCLabel("Game Over", "Chalkduster", 80);
            gameOverLabel.PositionX = 750;
            gameOverLabel.PositionY = 150;
            gameOverLabel.Color = CCColor3B.Red;
            gameOverLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            Schedule(RunGameLogic);
        }

        private void RunGameLogic(float frameTimeInSeconds)
        {
            ballYVelocity += frameTimeInSeconds * (-Gravity * levelMultiplier);
            ballSprite.PositionX += ballXVelocity * frameTimeInSeconds;
            ballSprite.PositionY += ballYVelocity * frameTimeInSeconds;

            bool doesBallOverlapPaddle = ballSprite.BoundingBoxTransformedToParent.IntersectsRect(paddleSprite.BoundingBoxTransformedToParent);
            bool isMovingDownwards = ballYVelocity < 0;
            bool isBallBelowPaddle = ballSprite.BoundingBoxTransformedToParent.MaxY < paddleSprite.BoundingBoxTransformedToParent.MidY;

            if ( isBallBelowPaddle)
            {
                winner = false;
                losesCount++;
                if(losesCount == 3)
                {
                    gameOver = true;
                }
                ResetGame();
                return;
            }
            if (doesBallOverlapPaddle && isMovingDownwards)
            {
                ballYVelocity *= -1;
                const float MinXVelocity = -300;
                const float MaxXVelocity = 300;

                ballXVelocity = CCRandom.GetRandomFloat(MinXVelocity, MaxXVelocity);

                score++;
                scoreLabel.Text = "Score: " + score;

                if(score >= 20 * level)
                {
                    level++;
                    losesCount = 0;
                    winner = true;
                    ResetGame();
                }
            }
            float ballRight = ballSprite.BoundingBoxTransformedToParent.MaxX;
            float ballLeft = ballSprite.BoundingBoxTransformedToParent.MinX;

            float screenRight = VisibleBoundsWorldspace.MaxX;
            float screenLeft = VisibleBoundsWorldspace.MinX;

            bool shouldReflectXVelocity = (ballRight > screenRight && ballXVelocity > 0) || (ballLeft < screenLeft && ballXVelocity < 0);

            if(shouldReflectXVelocity)
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


            if(winner)
            {
                AddChild(winnerLabel);
            }
            else if(gameOver)
            {
                AddChild(gameOverLabel);
            }
            else
            {
                AddChild(loserLabel);
            }
            AddChild(playLabel);
            CreateTouchListener();
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
                    if(winner)
                    {
                        levelLabel.Text = "Level: " + level;
                        //score = 0;
                        RemoveChild(winnerLabel);
                        levelMultiplier++;
                    }
                    if(!winner)
                    {
                        score = (level - 1) * 20;
                    }
                    if(gameOver)
                    {
                        score = 0;
                        level = 1;
                        losesCount = 0;
                        levelMultiplier = 1;
                        gameOver = false;
                        levelLabel.Text = "Level: " + level;
                        RemoveChild(gameOverLabel);
                    }
                    scoreLabel.Text = "Score: " + score;
                    RemoveChild(playLabel);
                    RemoveChild(loserLabel);
                    ballXVelocity = 0;
                    ballYVelocity = 0;
                    Schedule(RunGameLogic);                    
                }
            }
        }
    }
}
