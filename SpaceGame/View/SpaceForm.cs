using SpaceGame.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceGame.View
{
    public partial class SpaceForm : Form
    {

        private SpaceModel model;
        private PictureBox player;

        private Dictionary<Target, PictureBox> targets;

        private Timer targetCreateTimer;
        private Timer speedUpTimer;
        private Timer targetMoveTimer;
        private bool timerFinsihed = true;

        private bool pausePressed = false;

        PictureBox lifePictureBox0;
        PictureBox lifePictureBox1;
        PictureBox lifePictureBox2;

        private bool simpleGameStart = true;
        Stopwatch stopper;

        public SpaceForm()
        {
            //AllocConsole();

            InitializeComponent();

            showButton("START GAME", "startButton", new Point(240, 345), new Size(300, 80), this.startButtonClicked);

            showButton("LOAD GAME FROM FILE", "loadGameButton", new Point(240, 450), new Size(300, 80), this.loadGameFromFileButtonClicked);


            this.KeyPreview = true;

            this.KeyDown += new KeyEventHandler(this.keyDown);
            this.KeyUp += new KeyEventHandler(this.keyUp);


            #region optimalization
            //Optimalization
            this.SetStyle(ControlStyles.UserPaint, true);
            //2. Enable double buffer.
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //3. Ignore a windows erase message to reduce flicker.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            #endregion
        }


        #region ButtonClicked
        private void startButtonClicked(object sender, EventArgs e)
        {
            startGame();

        }

        private void restartButtonClicked(object sender, EventArgs e)
        {
            startGame();
        }

        private void loadGameFromFileButtonClicked(object sender, EventArgs e)
        {

            simpleGameStart = false;

            string fileContent = string.Empty;
            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "*.space|";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            model = new SpaceModel(new Persistance.Persistance());
            model.FileName = filePath;
            if (filePath != null && filePath != "")
            {
                model.loadGame();
                startGame();
            }
            else {
                simpleGameStart = true;
            }

        }

        private void saveGameButtonClicked(object sender, EventArgs e)
        {
            model.GameTimeSeconds = (int)(stopper.ElapsedMilliseconds / 1000);
            string gameStatusJson = model.saveGame();

            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.DefaultExt = "space";
            saveFileDialog1.Filter = "*.space|";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    myStream.Write(Encoding.ASCII.GetBytes(gameStatusJson));
                    myStream.Close();
                }
            }

        }

        #endregion

        public void startGame()
        {
            //Events

            this.Controls.Clear();

            timerFinsihed = true;
            pausePressed = false;

            player = new PictureBox();
            targets = new Dictionary<Target, PictureBox>();

            if (simpleGameStart)
            {
                model = new SpaceModel(new Persistance.Persistance());

                model.PlayerChanged += new EventHandler<Player>(playerChanged);
                model.TargetChanged += new EventHandler<Target>(targetChanged);
                model.LifeChanged += new EventHandler<int>(lifeChanged);
                model.GameOver += new EventHandler(gameOver);

                showLives();

                model.StartGame(Width, Height, 55, 48, 60, 50);

                stopper = Stopwatch.StartNew();
            }
            else
            {
                model.PlayerChanged += new EventHandler<Player>(playerChanged);
                model.TargetChanged += new EventHandler<Target>(targetChanged);
                model.LifeChanged += new EventHandler<int>(lifeChanged);
                model.GameOver += new EventHandler(gameOver);

                showLives();

                model.initializeGame();
                simpleGameStart = true;
            }

            player.Image = Properties.Resources.rocket;
            player.Size = new Size(model.PlayerWidth, model.PlayerHeight);
            player.SizeMode = PictureBoxSizeMode.StretchImage;
            player.BackColor = Color.Transparent;

            targetCreateTimer = new Timer();
            targetCreateTimer.Interval = model.TargetCreateTimer;
            targetCreateTimer.Tick += new EventHandler(targetCreateTimerTick);
            targetCreateTimer.Start();

            targetMoveTimer = new Timer();
            targetMoveTimer.Interval = model.TargetMoveTimer;
            targetMoveTimer.Tick += new EventHandler(targetmoveTimerTick);
            targetMoveTimer.Start();

            speedUpTimer = new Timer();
            speedUpTimer.Interval = model.SpeedUpTimer;
            speedUpTimer.Tick += new EventHandler(speedUpTimerTick);
            speedUpTimer.Start();

            stopper = new Stopwatch();
            stopper.Start();
        }

        #region Event handlers
        private void lifeChanged(object sender, int lifeNumber)
        {
            if (lifeNumber == 2)
            {
                lifePictureBox2.Image = Properties.Resources.emptyLife;
            }
            if (lifeNumber == 1)
            {
                lifePictureBox1.Image = Properties.Resources.emptyLife;
                lifePictureBox2.Image = Properties.Resources.emptyLife;
            }
            if (lifeNumber == 0)
            {
                lifePictureBox0.Image = Properties.Resources.emptyLife;
                lifePictureBox1.Image = Properties.Resources.emptyLife;
                lifePictureBox2.Image = Properties.Resources.emptyLife;
            }

        }
        private void targetChanged(object sender, Target target)
        {

            if (targets.ContainsKey(target))
            {
                targets[target].Left = target.PositionX;
                targets[target].Top = target.PositionY;
                if (target.status == "DELETE")
                {
                    this.Controls.Remove(targets[target]);
                    targets.Remove(target);
                }
            }
            else
            {

                PictureBox pictureBoxTarget = new PictureBox();
                pictureBoxTarget.Image = Properties.Resources.target;
                pictureBoxTarget.Left = target.PositionX;
                pictureBoxTarget.Top = target.PositionY;
                pictureBoxTarget.Size = new Size(target.Width, target.Height);
                pictureBoxTarget.SizeMode = PictureBoxSizeMode.StretchImage;
                //pictureBoxTarget.BringToFront();
                pictureBoxTarget.BackColor = Color.Transparent;
                targets.Add(target, pictureBoxTarget);
                this.Controls.Add(pictureBoxTarget);

            }


        }

        private void playerChanged(object sender, Player component)
        {
            player.Left = component.PositionX;
            player.Top = component.PositionY;
            this.Controls.Add(player);

        }

        #endregion

        #region Timer Tickers
        private void gameOver(object sender, EventArgs e)
        {
            pause();
            var tmp = this.Controls.OfType<Button>().Where(x => (string)x.Tag == "saveGameButton").ToArray();
            this.Controls.Remove(tmp[0]);
            tmp = this.Controls.OfType<Button>().Where(x => (string)x.Tag == "loadGameButton").ToArray();
            this.Controls.Remove(tmp[0]);
            showRestartScreen();
        }
        private void targetmoveTimerTick(object sender, EventArgs e)
        {
            if (timerFinsihed)
            {
                timerFinsihed = false;
                model.moveTargets();
                timerFinsihed = true;
            }
        }

        private void targetCreateTimerTick(object sender, EventArgs e)
        {
            model.createTarget();
        }

        private void speedUpTimerTick(object sender, EventArgs e)
        {
            if (targetCreateTimer.Interval > 200)
            {
                targetCreateTimer.Interval = (model.TargetCreateTimer -= 100);
            }
        }

        #endregion

        #region Key Events
        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                model.moveLeft();
            }

            if (e.KeyCode == Keys.Right)
            {
                model.moveRight();
            }
        }

        private void keyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.P)
            {
                pause();
            }
        }

        #endregion

        #region Functions

        public void showButton(string title, string name, Point position, Size size, Action<object, EventArgs> eventHandler)
        {
            Button button = new Button();
            button.Text = title;
            button.Location = position;
            button.Tag = name;
            button.Size = size;
            button.TabIndex = 0;
            button.UseVisualStyleBackColor = true;
            button.Click += new System.EventHandler(eventHandler);
            button.BringToFront();
            this.Controls.Add(button);
        }

        public void showLabel(string title, string name, Point position, Size size)
        {
            Label label = new Label();
            label.Text = title;
            label.ForeColor = Color.White;
            label.Location = position;
            label.Tag = name;
            label.Size = size;
            label.TabIndex = 0;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Font = new System.Drawing.Font(label.Font.Name, 24F);
            label.BackColor = Color.Transparent;
            label.BringToFront();
            this.Controls.Add(label);
        }

        private void pause()
        {
            //TODO game over figyel
            if (!pausePressed)
            {
                pausePressed = true;
                targetMoveTimer.Stop();
                targetCreateTimer.Stop();
                speedUpTimer.Stop();
                model.PlayerChanged -= new EventHandler<Player>(playerChanged);
                model.TargetChanged -= new EventHandler<Target>(targetChanged);
                model.LifeChanged -= new EventHandler<int>(lifeChanged);

                showButton("SAVE GAME", "saveGameButton", new Point(240, 450), new Size(300, 80), this.saveGameButtonClicked);
                showButton("LOAD GAME FROM FILE", "loadGameButton", new Point(240, 345), new Size(300, 80), this.loadGameFromFileButtonClicked);

            }
            else
            {
                var tmp = this.Controls.OfType<Button>().Where(x => (string)x.Tag == "saveGameButton").ToArray();
                this.Controls.Remove(tmp[0]);
                tmp = this.Controls.OfType<Button>().Where(x => (string)x.Tag == "loadGameButton").ToArray();
                this.Controls.Remove(tmp[0]);

                pausePressed = false;
                targetMoveTimer.Start();
                targetCreateTimer.Start();
                speedUpTimer.Start();
                stopper.Start();
                model.PlayerChanged += new EventHandler<Player>(playerChanged);
                model.TargetChanged += new EventHandler<Target>(targetChanged);
                model.LifeChanged += new EventHandler<int>(lifeChanged);
            }
        }

        private void showRestartScreen()
        {

            stopper.Stop();
            int ellapsedTime = (int)(stopper.ElapsedMilliseconds / 1000) + model.GameTimeSeconds;
            showLabel("GAME TIME: " + ellapsedTime.ToString() + " S", "noButton", new Point(240, 240), new Size(300, 80));

            showButton("START GAME", "startGameButton", new Point(240, 345), new Size(300, 80), this.startButtonClicked);

            showButton("LOAD GAME FROM FILE", "loadGameButton", new Point(240, 450), new Size(300, 80), this.loadGameFromFileButtonClicked);


        }

        private void showLives()
        {
            //TODO make dynamic
            #region Life picture boxis not dynamic
            lifePictureBox0 = new PictureBox();
            lifePictureBox0.BackgroundImageLayout = ImageLayout.Center;
            lifePictureBox0.Image = Properties.Resources.life;
            lifePictureBox0.Location = new Point(700, 12);
            lifePictureBox0.Name = "lifePictureBox0";
            lifePictureBox0.Size = new Size(39, 42);
            lifePictureBox0.SizeMode = PictureBoxSizeMode.Zoom;
            lifePictureBox0.TabIndex = 1;
            lifePictureBox0.TabStop = false;
            lifePictureBox0.Tag = "life0";
            // 
            // lifePictureBox1
            // 
            lifePictureBox1 = new PictureBox();
            lifePictureBox1.BackgroundImageLayout = ImageLayout.Center;
            lifePictureBox1.Image = Properties.Resources.life;
            lifePictureBox1.Location = new Point(655, 12);
            lifePictureBox1.Name = "lifePictureBox1";
            lifePictureBox1.Size = new Size(39, 42);
            lifePictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            lifePictureBox1.TabIndex = 2;
            lifePictureBox1.TabStop = false;
            lifePictureBox1.Tag = "life1";
            // 
            // lifePictureBox2
            // 
            lifePictureBox2 = new PictureBox();
            lifePictureBox2.BackgroundImageLayout = ImageLayout.Center;
            lifePictureBox2.Image = Properties.Resources.life;
            lifePictureBox2.Location = new Point(610, 12);
            lifePictureBox2.Name = "lifePictureBox2";
            lifePictureBox2.Size = new Size(39, 42);
            lifePictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            lifePictureBox2.TabIndex = 3;
            lifePictureBox2.TabStop = false;
            lifePictureBox2.Tag = "life2";
            #endregion

            this.Controls.Add(lifePictureBox0);
            this.Controls.Add(lifePictureBox1);
            this.Controls.Add(lifePictureBox2);
        }

        #endregion
    }

}
