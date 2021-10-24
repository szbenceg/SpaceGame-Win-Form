using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpaceGame;
using SpaceGame.Model;
using SpaceGame.Persistance;
using System;
using System.Collections.Generic;

namespace SpaceGameTest
{
    [TestClass]
    public class SpaceModelTest
    {


        [TestMethod]
        public void GameStarEvents()
        {

            SpaceModel model = new SpaceModel();

            List<string> receivedEvents = new List<string>();

            model.LifeChanged += delegate (object sender, int lifeNumber)
            {
                receivedEvents.Add("LifeChangedEvent");
            };

            model.PlayerChanged += delegate (object sender, Player player)
            {
                receivedEvents.Add("PlayerChangedEvent");
            };

            model.StartGame(500, 600, 55, 48, 60, 50);


            Assert.AreEqual(2, receivedEvents.Count);
            Assert.AreEqual("PlayerChangedEvent", receivedEvents[0]);
            Assert.AreEqual("LifeChangedEvent", receivedEvents[1]);

        }

        [TestMethod]
        public void LoadGameEvents()
        {
            SpaceWord spaceword = new SpaceWord(500, 600, 55, 48, 60, 50);
            spaceword.createTarget();
            spaceword.createTarget();

            Mock<IFFileManager> mock = new Mock<IFFileManager>();
            mock.Setup(m => m.LoadGame("path")).Returns(
                spaceword
            );

            SpaceModel model = new SpaceModel(mock.Object);


            List<string> receivedEvents = new List<string>();

            model.LifeChanged += delegate (object sender, int lifeNumber)
            {
                receivedEvents.Add("LifeChangedEvent");
            };

            model.PlayerChanged += delegate (object sender, Player player)
            {
                receivedEvents.Add("PlayerChangedEvent");
            };

            model.TargetChanged += delegate (object sender, Target player)
            {
                receivedEvents.Add("TargetChanged");
            };

            model.FileName = "path";
            model.loadGame();

            Assert.AreEqual(4, receivedEvents.Count);
            Assert.AreEqual("TargetChanged", receivedEvents[0]);
            Assert.AreEqual("TargetChanged", receivedEvents[1]);
            Assert.AreEqual("PlayerChangedEvent", receivedEvents[2]);
            Assert.AreEqual("LifeChangedEvent", receivedEvents[3]);


        }

        [TestMethod]
        public void TargetMove()
        {
            SpaceWord spaceword = new SpaceWord(500, 600, 55, 48, 60, 50);
            spaceword.createTarget();
            spaceword.createTarget();
            spaceword.createTarget();
            spaceword.createTarget();

            Mock<IFFileManager> mock = new Mock<IFFileManager>();
            mock.Setup(m => m.LoadGame("path")).Returns(
                spaceword
            );

            SpaceModel model = new SpaceModel(mock.Object);


            List<string> receivedEvents = new List<string>();

            model.FileName = "path";
            model.loadGame();

            model.TargetChanged += delegate (object sender, Target player)
            {
                receivedEvents.Add("TargetChanged");
            };

            model.moveTargets();

            Assert.AreEqual(4, receivedEvents.Count);
            Assert.AreEqual("TargetChanged", receivedEvents[0]);
            Assert.AreEqual("TargetChanged", receivedEvents[1]);
            Assert.AreEqual("TargetChanged", receivedEvents[2]);
            Assert.AreEqual("TargetChanged", receivedEvents[3]);


        }

        [TestMethod]
        public void PlayerMove()
        {
            SpaceWord spaceword = new SpaceWord(500, 600, 55, 48, 60, 50);
            spaceword.createTarget();
            spaceword.createTarget();
            spaceword.createTarget();
            spaceword.createTarget();

            Mock<IFFileManager> mock = new Mock<IFFileManager>();
            mock.Setup(m => m.LoadGame("path")).Returns(
                spaceword
            );

            SpaceModel model = new SpaceModel(mock.Object);


            List<string> receivedEvents = new List<string>();

            model.FileName = "path";
            model.loadGame();

            model.PlayerChanged += delegate (object sender, Player player)
            {
                receivedEvents.Add("PlayerChanged");
            };
            
            model.moveLeft();
            model.moveRight();


            Assert.AreEqual(2, receivedEvents.Count);
            Assert.AreEqual("PlayerChanged", receivedEvents[0]);
            Assert.AreEqual("PlayerChanged", receivedEvents[1]);


        }

        [TestMethod, Timeout(5000)]
        public void CollideLifeChange()
        {
            SpaceWord spaceword = new SpaceWord(500, 600, 55, 48, 60, 50);
            spaceword.Targets.Add(new Target(spaceword.Player.PositionX, 0, 55, 48));

            Mock<IFFileManager> mock = new Mock<IFFileManager>();
            mock.Setup(m => m.LoadGame("path")).Returns(
                spaceword
            );

            SpaceModel model = new SpaceModel(mock.Object);

            List<string> receivedEvents = new List<string>();

            model.FileName = "path";
            model.loadGame();

            bool collided = false;

            model.LifeChanged += delegate (object sender, int lifeNumber)
            {
                receivedEvents.Add("LifeChanged");
                collided = true;
            };

            model.TargetChanged += delegate (object sender, Target target)
            {
                if (collided) {
                    Assert.AreEqual("DELETE", target.status);
                }
            };

            while (!collided) {
                model.moveTargets();
            }

            Assert.AreEqual(true, collided);


        }

        [TestMethod, Timeout(5000)]
        public void GameOver()
        {
            SpaceWord spaceword = new SpaceWord(500, 600, 55, 48, 60, 50);
            spaceword.Targets.Add(new Target(spaceword.Player.PositionX, 0, 55, 48));
            spaceword.LifeNumber = 1;

            Mock<IFFileManager> mock = new Mock<IFFileManager>();
            mock.Setup(m => m.LoadGame("path")).Returns(
                spaceword
            );

            SpaceModel model = new SpaceModel(mock.Object);

            List<string> receivedEvents = new List<string>();

            model.FileName = "path";
            model.loadGame();

            bool collided = false;

            model.LifeChanged += delegate (object sender, int lifeNumber)
            {
                receivedEvents.Add("LifeChanged");
                collided = true;
            };

            model.GameOver += delegate (object sender, EventArgs e)
            {
                receivedEvents.Add("GameOver");
            };

            while (!collided)
            {
                model.moveTargets();
            }

            Assert.AreEqual(2, receivedEvents.Count);
            Assert.AreEqual("LifeChanged", receivedEvents[0]);
            Assert.AreEqual("GameOver", receivedEvents[1]);


        }

    }
}
