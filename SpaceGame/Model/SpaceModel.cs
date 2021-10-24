using SpaceGame.Persistance;
using System;
using System.Collections.Generic;

namespace SpaceGame.Model
{
    public class SpaceModel
    {

        #region events
      
        public event EventHandler<Player> PlayerChanged;
        public event EventHandler<Target> TargetChanged;
        public event EventHandler<int> LifeChanged;
        public event EventHandler GameOver;
        #endregion

        private SpaceWord spaceWord;
        private IFFileManager dataAccess;
        private string fileName;

        #region methods

        public SpaceModel() { }
        public SpaceModel(IFFileManager fileManager) {
            dataAccess = fileManager;
        }

        public void StartGame(int width, int height, int targetWidth, int targetHeight, int playerWidth, int playerHeight) {

            spaceWord = new SpaceWord(width, height, targetWidth, targetHeight, playerWidth, playerHeight);

            PlayerChanged?.Invoke(this, spaceWord.Player);
            LifeChanged?.Invoke(this, spaceWord.LifeNumber);

        }
        public string saveGame() {
            dataAccess = new SpaceGame.Persistance.Persistance();
            return dataAccess.SaveGame(spaceWord);
        }

        public void initializeGame() {
            foreach (Target target in spaceWord.Targets) {
                TargetChanged?.Invoke(this, target);
            }
            PlayerChanged?.Invoke(this, spaceWord.Player);
            LifeChanged?.Invoke(this, spaceWord.LifeNumber);
        }

        public void moveLeft() {
            spaceWord.Player.moveLeft();
            PlayerChanged?.Invoke(this, spaceWord.Player);
        }

        public void loadGame()
        {
            spaceWord = dataAccess.LoadGame(fileName);
            initializeGame();
        }

        public void moveRight()
        {
            spaceWord.Player.moveRight();
            PlayerChanged?.Invoke(this, spaceWord.Player);
        }

        public void createTarget()
        {
            spaceWord.createTarget();
        }

        public void moveTargets() {
            List<Target> tmp = new List<Target>();
            foreach (Target target in spaceWord.Targets) {
                target.moveDown();
                bool collided = spaceWord.collide(target, spaceWord.Player);
                bool outOfScreen = target.PositionY > spaceWord.WindowHeight - 0;
                if (outOfScreen)
                {
                    target.status = "DELETE";
                    tmp.Add(target);
                }
                else if (collided)
                {
                    target.status = "DELETE";
                    tmp.Add(target);
                    spaceWord.LifeNumber = spaceWord.LifeNumber - 1;
                    LifeChanged?.Invoke(this, spaceWord.LifeNumber);
                    if (spaceWord.LifeNumber == 0) {
                        GameOver?.Invoke(this, EventArgs.Empty);
                    }
                }
                TargetChanged?.Invoke(this, target);
            }

            foreach (Target target in tmp) {
                spaceWord.removeTarget(target);
            }
        }

        #endregion

        #region Setter/Getter

        public int PlayerWidth
        {
            get
            {
                return spaceWord.Player.Width;
            }

        }
        public int PlayerHeight
        {
            get
            {
                return spaceWord.Player.Height;
            }

        }
        public int TargetCreateTimer
        {
            get
            {
                return spaceWord.TargetCreateTimer;
            }
            set
            {
                spaceWord.TargetCreateTimer = value;
            }
        }

        public int TargetMoveTimer
        {
            get
            {
                return spaceWord.TargetMoveTimer;
            }
            set
            {
                spaceWord.TargetMoveTimer = value;
            }
        }

        public int SpeedUpTimer
        {
            get
            {
                return spaceWord.SpeedUpTimer;
            }
            set
            {
                spaceWord.SpeedUpTimer = value;
            }
        }

        public int GameTimeSeconds
        {
            get
            {
                return spaceWord.GameTimeSeconds;
            }
            set
            {
                spaceWord.GameTimeSeconds = value;
            }
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

        #endregion 
    }
}