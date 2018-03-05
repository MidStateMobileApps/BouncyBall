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
        CCLabel scoreLabel;
        CCLabel playLabel;
        CCLabel losePlayLabel;
        CCLabel winPlayLabel;
        CCLabel levelLabel;
        CCLabel gameOverLabel;

        float ballXVelocity;
        float ballYVelocity;

        float bombXVelocity;
        float bombYVelocity;

        float gravity = 140;
        int levelMultiplier = 1;
        bool winner = false;

        int score = 0;
        int level = 1;
        int round = 1;
        int beginningScore = 0;
        int lostRounds = 0;
        

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

            bombSprite = new CCSprite("bomb");
            bombSprite.PositionX = 320;
            bombSprite.PositionY = 600;
            
            scoreLabel = new CCLabel("Score: 0", "Arial", 70, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 1000;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            levelLabel = new CCLabel("Level 1", "Arial", 70, CCLabelFormat.SystemFont);
            levelLabel.PositionX = 750;
            levelLabel.PositionY = 1000;
            levelLabel.AnchorPoint = CCPoint.AnchorUpperRight;
            AddChild(levelLabel);

            losePlayLabel = new CCLabel("You Lose!!", "Chalkduster", 100);
            losePlayLabel.PositionX = 750;
            losePlayLabel.PositionY = 300;
            losePlayLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            winPlayLabel = new CCLabel("You Win!!", "Chalkduster", 100);
            winPlayLabel.PositionX = 750;
            winPlayLabel.PositionY = 300;
            winPlayLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            playLabel = new CCLabel("Play Again?", "Chalkduster", 70);
            playLabel.PositionX = 750;
            playLabel.PositionY = 100;
            playLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            gameOverLabel = new CCLabel("Game Over", "Chalkduster", 70);
            gameOverLabel.PositionX = 750;
            gameOverLabel.PositionY = 300;
            gameOverLabel.AnchorPoint = CCPoint.AnchorLowerRight;

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
                levelLabel.Text = "Level " + level;
                score = beginningScore;
                scoreLabel.Text = "Score: " + score;
                lostRounds++;

                if (lostRounds == 3)
                {
                    GameOver();
                }
                else
                {
                    ResetGame();
                }
                return;
            }
            if (doesBallOverlapPaddle && isMovingDownward)
            {
                ballYVelocity *= -1;
                const float minXVelocity = -300;
                const float maxXVelocity = 300;
                ballXVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);
                score++;
                levelLabel.Text = "Level " + level;
                scoreLabel.Text = "Score: " + score;
                if ( score % 20 == 0)
                {
                    lostRounds = 0;
                    beginningScore = round * 20;
                    round++;
                    level++;
                        levelLabel.Text = "Level " + level;
                    winner = true;
                    WinGame();
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
            if ( level >= 5 )
            {
                AddChild(bombSprite);
                bombYVelocity += frameTimeInSeconds * (-gravity * levelMultiplier);
                bombSprite.PositionX += bombXVelocity * frameTimeInSeconds;
                bombSprite.PositionY += bombYVelocity * frameTimeInSeconds;

                bool doesBombOverlapPaddle =
                    bombSprite.BoundingBoxTransformedToParent.
                    IntersectsRect(paddleSprite.BoundingBoxTransformedToParent);


                bool isBombMovingDownward = bombYVelocity < 0;
                bool isBombBelowPaddle = bombSprite.BoundingBoxTransformedToParent.MaxY <
                                paddleSprite.BoundingBoxTransformedToParent.MinY;
                if (doesBombOverlapPaddle && isBombMovingDownward)
                {
                    bombYVelocity *= -1;
                    const float minXVelocity = -300;
                    const float maxXVelocity = 300;
                    bombXVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);
                    GameOver();
                }
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

            AddChild(losePlayLabel);
            AddChild(playLabel);

            CreateTouchListener();
        }

        private void WinGame()
        {
            gravity += 25;

            StopAllActions();
            Unschedule(RunGameLogic);

            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;

            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;

            AddChild(playLabel);
            AddChild(winPlayLabel);
  
            CreateTouchListener();
        }

        private void GameOver()
        {
            gravity = 100;
            gravity = 140;
            levelMultiplier = 1;
            winner = false;
            score = 0;
            beginningScore = 0;
            round = 1;
            level = 1;
            ballYVelocity = -1;

            scoreLabel.Text = "Score: " + score;
            levelLabel.Text = "Level " + level;

            StopAllActions();
            Unschedule(RunGameLogic);

            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;

            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;

            AddChild(gameOverLabel);
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
                    if ( winner )
                        levelMultiplier++;
                    Schedule(RunGameLogic);
                    RemoveChild(playLabel);
                    RemoveChild(losePlayLabel);
                    RemoveChild(winPlayLabel);
                    RemoveChild(gameOverLabel);
                    winner = false;
                }
                if (winPlayLabel.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    if (winner)
                        levelMultiplier++;
                    Schedule(RunGameLogic);
                    RemoveChild(playLabel);
                    RemoveChild(losePlayLabel);
                    RemoveChild(winPlayLabel);
                    RemoveChild(gameOverLabel);
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
