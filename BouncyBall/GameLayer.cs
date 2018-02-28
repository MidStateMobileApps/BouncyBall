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
        CCLabel levelLabel;
        CCLabel gameOverLabel;
        CCLabel youLoseLabel;
        CCLabel restartLabel;
        CCLabel youWinLabel;
        CCLabel nextLevelLabel;
        CCLabel scoreCounterLabel;

        float ballXVelocity;
        float ballYVelocity;

        const float GRAVITY = 140;
        int levelMultiplier = 1;
        int lossCounter = 0;
        int currentLevelScore = 0;
        int level = 1;
        bool winner = false;

        int score = 0;

        public GameLayer() : base(CCColor4B.Black)
        {
            paddleSprite = new CCSprite("Paddle");
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;
            AddChild(paddleSprite);

            ballSprite = new CCSprite("Ball");
            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;
            AddChild(ballSprite);

            scoreLabel = new CCLabel("Total Score: 0", "Arial", 60, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 1000;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            scoreCounterLabel = new CCLabel("Current Level Score: 0", "Arial", 60, CCLabelFormat.SystemFont);
            scoreCounterLabel.PositionX = 50;
            scoreCounterLabel.PositionY = 920;
            scoreCounterLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreCounterLabel);

            levelLabel = new CCLabel("Level: 1", "Arial", 60, CCLabelFormat.SystemFont);
            levelLabel.PositionX = 50;
            levelLabel.PositionY = 840;
            levelLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(levelLabel);

            playLabel = new CCLabel("Play Again?", "Chalkduster", 70);
            playLabel.PositionX = 750;
            playLabel.PositionY = 100;
            playLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            restartLabel = new CCLabel("Restart?", "Chalkduster", 70);
            restartLabel.PositionX = 750;
            restartLabel.PositionY = 100;
            restartLabel.AnchorPoint = CCPoint.AnchorLowerRight;

            youLoseLabel = new CCLabel("You Lose!", "Chalkduster", 100);
            youLoseLabel.PositionX = 100;
            youLoseLabel.PositionY = 500;
            youLoseLabel.AnchorPoint = CCPoint.AnchorUpperLeft;

            gameOverLabel = new CCLabel("Game Over!", "Chalkduster", 100);
            gameOverLabel.PositionX = 100;
            gameOverLabel.PositionY = 500;
            gameOverLabel.AnchorPoint = CCPoint.AnchorUpperLeft;

            youWinLabel = new CCLabel("You Won!", "Chalkduster", 100);
            youWinLabel.PositionX = 100;
            youWinLabel.PositionY = 500;
            youWinLabel.AnchorPoint = CCPoint.AnchorUpperLeft;

            nextLevelLabel = new CCLabel("Next Level?", "Chalkduster", 70);
            nextLevelLabel.PositionX = 750;
            nextLevelLabel.PositionY = 100;
            nextLevelLabel.AnchorPoint = CCPoint.AnchorLowerRight;


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

            levelLabel.Text = "Level: " + level;

            

            if (doesBallOverlapPaddle && isMovingDownward)
            {
                ballYVelocity *= -1;
                const float MINXVELOCITY = -300;
                const float MAXXVELOCITY = 300;

                ballXVelocity = CCRandom.GetRandomFloat(MINXVELOCITY, MAXXVELOCITY);
                score++;
                currentLevelScore++;
                scoreLabel.Text = "Total Score: " + score;
                scoreCounterLabel.Text = "Current Level Score: " + currentLevelScore;
                
                if (currentLevelScore >= 20)
                {
                    winner = true;
                    WonGame();
                }
            }

            if (isBallBelowPaddle)
            {
                lossCounter++;

                if (lossCounter < 3)
                {
                    ResetGame();
                    return;
                }
                else
                {
                    GameOver();
                }
            }

            float ballRight = ballSprite.BoundingBoxTransformedToParent.MaxX;
            float ballLeft = ballSprite.BoundingBoxTransformedToParent.MinX;
            float screenRight = VisibleBoundsWorldspace.MaxX;
            float screenLeft = VisibleBoundsWorldspace.MinX;
            bool shouldReflectXVelocity = (ballRight > screenRight && ballXVelocity > 0) || (ballLeft < screenLeft && ballXVelocity < 00);

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

            AddChild(playLabel);
            CreateTouchListener();
        }

        private void WonGame()
        {
            StopAllActions();
            Unschedule(RunGameLogic);

            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;

            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;

            AddChild(youWinLabel);
            AddChild(nextLevelLabel);

            CreateTouchListenerWinGame();
        }

        private void GameOver()
        {
            StopAllActions();
            Unschedule(RunGameLogic);

            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;

            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;

            AddChild(restartLabel);
            AddChild(gameOverLabel);
            CreateTouchListenerGameOver();
        }

        
        // On Loss events
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
                    score = score - currentLevelScore;
                    scoreLabel.Text = "Total Score: " + score;
                    currentLevelScore = 0;
                    scoreCounterLabel.Text = "Current Level Score: " + currentLevelScore;
                    levelMultiplier = 1;

                    RemoveChild(playLabel);
                    RemoveChild(youLoseLabel);                    
                    winner = false;

                    Schedule(RunGameLogic);
                }
            }
        }

        // Game Over events
        private void CreateTouchListenerGameOver()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = TouchesBeganGameOver;
            AddEventListener(touchListener);
        }

        private void TouchesBeganGameOver(List<CCTouch> touches, CCEvent touchEvent)
        {
            foreach (var touch in touches)
            {
                if (restartLabel.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    score = 0;
                    scoreLabel.Text = "Total Score :" + score;
                    currentLevelScore = 0;
                    scoreCounterLabel.Text = "Current Level Score: " + currentLevelScore;

                    
                    lossCounter = 0;
                    ballYVelocity = 0;
                    level = 1;
                    levelMultiplier = level;


                    RemoveChild(gameOverLabel);
                    RemoveChild(restartLabel);
                    winner = false;

                    Schedule(RunGameLogic);
                }
            }
        }

        // Game won events
        private void CreateTouchListenerWinGame()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = TouchesBeganGameWon;
            AddEventListener(touchListener);
        }

        
        private void TouchesBeganGameWon(List<CCTouch> touches, CCEvent touchEvent)
        {
            foreach (var touch in touches)
            {
                if (nextLevelLabel.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {                   
                    level++;
                    levelMultiplier = level;
                    lossCounter = 0;
                    RemoveChild(youWinLabel);
                    RemoveChild(nextLevelLabel);
                    currentLevelScore = 0;
                    scoreCounterLabel.Text = "Current Level Score: " + currentLevelScore;
                    winner = false;

                    
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
