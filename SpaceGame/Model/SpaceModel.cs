using System;
using System.Collections.Generic;

namespace SpaceGame.Model
{
    class SpaceModel
    {
        #region Private fields

        private int windowHeight;
        private int windowWidth;

        private int targetWidth;
        private int targetHeight;

        private int lifeNumber;

        private int targetCreateTimer = 2000;
        private int targetMoveTimer = 50;
        private int speedUpTimer = 5000;
        private int gameTimeSeconds = 0;

        public List<Target> targets;
        public Player player;

        Random random = new Random();

        #endregion
        #region events
      
        public event EventHandler<Player> PlayerChanged;
        public event EventHandler<Target> TargetChanged;
        public event EventHandler<int> LifeChanged;
        public event EventHandler GameOver;

        #endregion
        #region methods

        public void StartGame(int width, int height) {

            this.windowHeight = height;
            this.windowWidth = width;

            targets = new List<Target>();
            player = new Player();

            player.PositionX = width / 2 - (player.Width / 2);
            player.PositionY = height - 100;
            lifeNumber = 3;

            PlayerChanged?.Invoke(this, player);
            LifeChanged?.Invoke(this, lifeNumber);

        }

        private bool collide(Target target, Player player) {

            if (player.PositionX + player.Width/ 2 <= target.PositionX + target.Width + player.Width/ 2 + 2 &&
              player.PositionX + player.Width / 2 >= target.PositionX - player.Width / 2 - 2 &&
              player.PositionY <= target.PositionY+ target.Height &&
              player.PositionY >= target.PositionY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string saveGame() {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return json;
        }

        public void initializeGame() {
            foreach (Target target in targets) {
                TargetChanged?.Invoke(this, target);
            }
            PlayerChanged?.Invoke(this, player);
            LifeChanged?.Invoke(this, lifeNumber);
        }

        public void moveLeft() {
            player.moveLeft();
            PlayerChanged?.Invoke(this, player);
        }

        public void moveRight()
        {
            player.moveRight();
            PlayerChanged?.Invoke(this, player);
        }

        public void createTarget()
        {
            targets.Add(new Target(random.Next(0, windowWidth-targetWidth), 0, targetWidth, targetHeight));
        }

        public void moveTargets() {
            List<Target> tmp = new List<Target>();
            foreach (Target target in targets) {
                target.moveDown();
                bool collided = collide(target, player);
                bool outOfScreen = target.PositionY > windowHeight - 0;
                if (outOfScreen)
                {
                    target.status = "DELETE";
                    tmp.Add(target);
                }
                else if (collided)
                {
                    target.status = "DELETE";
                    tmp.Add(target);
                    lifeNumber--;
                    LifeChanged?.Invoke(this, lifeNumber);
                    if (lifeNumber == 0) {
                        GameOver?.Invoke(this, EventArgs.Empty);
                    }
                }
                TargetChanged?.Invoke(this, target);
            }

            foreach (Target target in tmp) {
                targets.Remove(target);
            }
        }

        #endregion

        #region Setter/Getter for json
        public int TargetWidth
        {
            get
            {
                return targetWidth;
            }
            set
            {
                targetWidth = value;
            }
        }
        public int TargetHeight
        {
            get
            {
                return targetHeight;
            }
            set
            {
                targetHeight = value;
            }
        }

        public int WindowHeight
        {
            get
            {
                return windowHeight;
            }
            set
            {
                windowHeight = value;
            }
        }
        public int WindowWidth
        {
            get
            {
                return windowWidth;
            }
            set
            {
                windowWidth = value;
            }
        }
        public int LifeNumber
        {
            get
            {
                return lifeNumber;
            }
            set
            {
                lifeNumber = value;
            }
        }
        public int TargetCreateTimer
        {
            get
            {
                return targetCreateTimer;
            }
            set
            {
                targetCreateTimer = value;
            }
        }

        public int TargetMoveTimer
        {
            get
            {
                return targetMoveTimer;
            }
            set
            {
                targetMoveTimer = value;
            }
        }

        public int SpeedUpTimer
        {
            get
            {
                return speedUpTimer;
            }
            set
            {
                speedUpTimer = value;
            }
        }

        public int GameTimeSeconds
        {
            get
            {
                return gameTimeSeconds;
            }
            set
            {
                gameTimeSeconds = value;
            }
        }

        #endregion 
    }
}