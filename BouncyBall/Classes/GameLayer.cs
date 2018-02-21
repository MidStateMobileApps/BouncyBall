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
        CCSprite bombSprite;
        CCLabel levelLabel;
        CCLabel scoreLabel;
        CCLabel winnerLabel;
        CCLabel loserLabel;
        CCLabel gameOverLabel;
        CCLabel playLabel;

        float ballXVelocity;
        float ballYVelocity;
        
        float bombYVelocity;

        const float gravity = 140;
        int levelMultiplier = 1;

        int level = 1;
        int score = 0;

        int lostCount = 0;
        Random random = new Random(Guid.NewGuid().GetHashCode());

        bool winner;
        bool gameOver;

        public GameLayer(): base(CCColor4B.Black)
        {
            bombSprite = new CCSprite("bomb");
            bombSprite.PositionX = random.Next(10, 520);
            bombSprite.PositionY = 1000;

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

            gameOverLabel = new CCLabel("Game Over", "Chalkduster", 80);
            gameOverLabel.PositionX = 750;
            gameOverLabel.PositionY = 150;
            gameOverLabel.Color = CCColor3B.Red;
            gameOverLabel.AnchorPoint = CCPoint.AnchorLowerRight;

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
                lostCount++;
                if (lostCount == 3)
                {
                    gameOver = true;
                }
                ResetGame();
                return;
            }

            if (level >= 5)
            {
                AddChild(bombSprite);

                bombYVelocity += frameTimeInSeconds * (-gravity * levelMultiplier);
                bombSprite.PositionY += bombYVelocity * frameTimeInSeconds;
                if (bombSprite.BoundingBoxTransformedToParent.MaxY < paddleSprite.BoundingBoxTransformedToParent.MinY)
                {
                    RemoveChild(bombSprite);
                    bombSprite.PositionX = random.Next(10, 620);
                    bombSprite.PositionY = 1200;
                    AddChild(bombSprite);
                    bombYVelocity += frameTimeInSeconds * (-gravity * levelMultiplier);
                    bombSprite.PositionY += bombYVelocity * frameTimeInSeconds;
                }

                if (bombSprite.BoundingBoxTransformedToParent.IntersectsRect(paddleSprite.BoundingBoxTransformedToParent))
                {
                    winner = false;
                    ResetGame();
                    return;
                }

            }

            if (doesBallOverlapPaddle && isMovingDownwards)
            {
                ballYVelocity *= -1;
                const float minXVelocity = -300;
                const float maxXVelocity = 300;

                ballXVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);

                score++;
                
                scoreLabel.Text = "Score: " + score;
                if (score / level == 20 || score == 20)                    
                {
                    level++;
                    lostCount = 0;
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

            if(winner)
            {
                AddChild(winnerLabel);
            }
            else if (gameOver)
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
                    if (gameOver)
                    {
                        score = 0;
                        level = 1;
                        lostCount = 0;
                        gameOver = false;
                        levelLabel.Text = "Level: " + level;
                        RemoveChild(gameOverLabel);
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
