using Business.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests.BowlingTests
{
    [TestClass]
    public class ScoringTest
    {
        private BowlingEngine _bowlingEngine;

        public ScoringTest()
        {
            var scoringEngine = new ScoringEngine();
            _bowlingEngine = new BowlingEngine(scoringEngine);
        }

        [TestMethod]
        public void GivenAllFramesAreCompleteErrorOccursWhenRolling()
        {
            // act
            var gameState = _bowlingEngine.StartGame();
            foreach (var frame in gameState.Frames)
            {
                _bowlingEngine.Roll(10);
            }
            _bowlingEngine.Roll(10);
            _bowlingEngine.Roll(10);
            // now break it
            var didThrow = false;
            try
            {
                _bowlingEngine.Roll(10);
            }
            catch (System.Exception)
            {
                didThrow = true;
            }
            Assert.IsTrue(didThrow, "Should have thrown an error.");
        }

        [TestMethod]
        public void GivenThreeConsecutiveRollsOfFivePointsScoreShouldBeFifteen()
        {
            // arrange
            var expectedResult = 15;

            // act
            _bowlingEngine.StartGame();
            _bowlingEngine.Roll(5);
            _bowlingEngine.Roll(5);
            var gameState = _bowlingEngine.Roll(5);

            // assert
            Assert.AreEqual(expectedResult, gameState.Frames.First().Score);
        }

        [TestMethod]
        public void CalculateSpareCorrectly()
        {
            // arrange
            var expectedFirstFrameScore = 11;
            var expectedRunningTotal = 17;

            // act
            _bowlingEngine.StartGame();
            _bowlingEngine.Roll(1);
            _bowlingEngine.Roll(9);// 10 + 1
            _bowlingEngine.Roll(1);
            var gameState = _bowlingEngine.Roll(5);

            // assert
            Assert.AreEqual(expectedFirstFrameScore, gameState.Frames.First().Score);
            Assert.AreEqual(expectedRunningTotal, gameState.RunningTotal);
        }

        [TestMethod]
        public void GivenFirstRollIsAStrikeAndTheNextTwoAreZeroScoreShouldBe10()
        {
            // arrange
            var expectedResult = 10;

            // act
            _bowlingEngine.StartGame();
            _bowlingEngine.Roll(10);
            _bowlingEngine.Roll(0);
            var gameState = _bowlingEngine.Roll(0);

            // assert
            Assert.AreEqual(expectedResult, gameState.RunningTotal);
        }

        [TestMethod]
        public void GivenAllGuttersTheScoreShouldBe0()
        {
            // arrange
            var expectedResult = 0;

            // act
            var gameState = _bowlingEngine.StartGame();
            foreach (var frame in gameState.Frames)
            {
                _bowlingEngine.Roll(0);
                _bowlingEngine.Roll(0);
            }

            // assert
            Assert.AreEqual(expectedResult, gameState.RunningTotal);
        }

        [TestMethod]
        public void GivenAllStrikesTheScoreShouldBe300()
        {
            // arrange
            var expectedResult = 300;

            // act
            var gameState = _bowlingEngine.StartGame();
            foreach (var frame in gameState.Frames)
            {
                _bowlingEngine.Roll(10);
            }
            // now roll two more strikes at the end
            _bowlingEngine.Roll(10);
            gameState = _bowlingEngine.Roll(10);

            // assert
            Assert.AreEqual(expectedResult, gameState.RunningTotal);
        }

        [TestMethod]
        public void GivenAllZerosLastFrameSpareWithAStrikeFollowingTheScoreShouldBe20()
        {
            // arrange
            var expectedResult = 20;

            // act
            var gameState = _bowlingEngine.StartGame();
            foreach (var frame in gameState.Frames)
            {
                if (frame.Number < 10)
                {
                    _bowlingEngine.Roll(0);
                    _bowlingEngine.Roll(0);
                }
                else
                {
                    _bowlingEngine.Roll(1);
                    _bowlingEngine.Roll(9);
                }
            }
            // now roll a strike at the end
            gameState = _bowlingEngine.Roll(10);

            // assert
            Assert.AreEqual(expectedResult, gameState.RunningTotal);
        }

        [TestMethod]
        public void GivenAllZeros9thFrameSpareWithThreeStrikesFollowingTheScoreShouldBe50()
        {
            // arrange
            var expectedResult = 50;

            // act
            var gameState = _bowlingEngine.StartGame();
            foreach (var frame in gameState.Frames)
            {
                if (frame.Number < 9)
                {
                    _bowlingEngine.Roll(0);
                    _bowlingEngine.Roll(0);
                }
                else if (frame.Number == 9)
                {
                    _bowlingEngine.Roll(1);
                    _bowlingEngine.Roll(9);
                }
                else
                {
                    _bowlingEngine.Roll(10);
                    _bowlingEngine.Roll(10);
                }
            }
            // now roll a strike at the end
            gameState = _bowlingEngine.Roll(10);

            // assert
            Assert.AreEqual(expectedResult, gameState.RunningTotal);
        }
    }
}