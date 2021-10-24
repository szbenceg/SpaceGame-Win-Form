using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Model
{
    public class SpaceWord
    {
        #region Fields

        private int windowHeight;
        private int windowWidth;

        private int targetWidth;
        private int targetHeight;

        private int lifeNumber;

        private int targetCreateTimer = 2000;
        private int targetMoveTimer = 50;
        private int speedUpTimer = 5000;
        private int gameTimeSeconds = 0;

        private List<Target> targets;
        private Player player;

        private Random random = new Random();

        #endregion

        #region Getter/Setter

        public List<Target> Targets {
            get {
                return targets;
            }
            set {
                targets = value;
            }
        }

        public Player Player {
            get {
                return player;
            }
            set {
                player = value;
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

        #region Constructor

        public SpaceWord(int width, int height, int targetWidth, int targetHeight, int playerWidth, int playerHeight)
        {

            this.windowHeight = height;
            this.windowWidth = width;
            this.targetWidth = targetWidth;
            this.targetHeight = targetHeight;

            targets = new List<Target>();
            player = new Player();

            player.PositionX = width / 2 - (player.Width / 2);
            player.PositionY = height - 100;
            lifeNumber = 3;

            player.Width = playerWidth;
            player.Height = playerHeight;
            player.Speed = 12;
        }

        #endregion

        #region Methods
        public bool collide(Target target, Player player)
        {

            if (player.PositionX + player.Width / 2 <= target.PositionX + target.Width + player.Width / 2 + 2 &&
              player.PositionX + player.Width / 2 >= target.PositionX - player.Width / 2 - 2 &&
              player.PositionY <= target.PositionY + target.Height &&
              player.PositionY >= target.PositionY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void moveLeft()
        {
            player.moveLeft();
        }

        public void moveRight()
        {
            player.moveRight();
        }

        public void createTarget()
        {
            targets.Add(new Target(random.Next(0, windowWidth - targetWidth), 0, targetWidth, targetHeight));
        }

        public void removeTarget(Target target) {
            targets.Remove(target);
        }

        #endregion

    }
}
