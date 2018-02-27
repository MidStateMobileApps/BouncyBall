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
        CCLabel levelLabel;
        CCLabel gameOverLabel;

        float ballXVelocity;
        float ballYVelocity;
        bool winner;
        bool gameOver;

        const float GRAVITY = 140;

        int levelMultiplier = 1;
        int score = 0;
        int lossColumn = 0;
        int level = 1;

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

            levelLabel = new CCLabel("Level: 0", "Ariel", 70, CCLabelFormat.SystemFont);
            levelLabel.PositionX = 50;
            levelLabel.PositionY = 950;
            levelLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(levelLabel);

            loseLabel = new CCLabel("You lose!", "ChalkDuster", 100);
            loseLabel.AnchorPoint = CCPoint.AnchorLowerLeft;

            playLabel = new CCLabel("Play Again?", "Chalkduster", 70);
            playLabel.PositionX = 750;
            playLabel.PositionY = 100;
            playLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            winnerLabel = new CCLabel("WINNER!", "Chalkduster", 100);
            winnerLabel.PositionX = 750;
            winnerLabel.PositionY = 150;
            winnerLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            gameOverLabel = new CCLabel("Game over! \nThanks for playing!", "Chalkduster", 185);
            gameOverLabel.AnchorPoint = CCPoint.AnchorLowerLeft;

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
                winner = false;
                lossColumn++;
                if (lossColumn == 3)
                {
                    gameOver = true;
                }
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
                if (score / level == 2 || score == 2)
                {
                    level++;
                    lossColumn = 0;
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
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;
            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;

            if (winner)
            {
                AddChild(winnerLabel);
            }
            else if (gameOver)
            {
                AddChild(gameOverLabel);
            }
            else
            {
                AddChild(loseLabel);
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
                    if (gameOver)
                    {
                        score = 0;
                        level = 1;
                        lossColumn = 0;
                        RemoveChild(gameOverLabel);
                        levelLabel.Text = "Level: 1";
                        gameOver = false;
                        ballXVelocity = 0;
                        ballYVelocity = 0;
                    }

                    scoreLabel.Text = "Score: " + score;
                    RemoveChild(loseLabel);
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
