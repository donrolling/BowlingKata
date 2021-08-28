using Business.Constants;
using Business.ExtenstionMethods;
using Business.Factories;
using Contracts.Bowling;
using Models.Bowling;
using System;
using System.Linq;

namespace Business.Engines
{
    public class BowlingEngine : IBowlingEngine
    {
        private GameState _gameState;
        private IScoringEngine _scoringEngine;

        public BowlingEngine(IScoringEngine scoringEngine)
        {
            _scoringEngine = scoringEngine;
        }

        public GameState StartGame()
        {
            _gameState = GameStateFactory.CreateNewGame(BowlingConstants.LastFrame);
            return _gameState;
        }

        public GameState Roll(int value)
        {
            var isLastFrame = _gameState.CurrentFrameNumber == BowlingConstants.LastFrame;
            var currentFrame = _gameState.CurrentFrame();
            CheckForErrorState(isLastFrame, currentFrame);
            if (_gameState.CurrentRoll == RollNumber.One)
            {
                currentFrame.FirstRoll = value;
                if (
                    value == BowlingConstants.FullPoints
                    && _gameState.CurrentFrameNumber != BowlingConstants.LastFrame // last frame gets extra rolls
                )
                {
                    currentFrame.FrameComplete = true;
                }
            }
            else if (_gameState.CurrentRoll == RollNumber.Two)
            {
                currentFrame.SecondRoll = value;
                if (!isLastFrame)
                {
                    currentFrame.FrameComplete = true;
                }
                else if (
                  currentFrame.FirstRoll.Value + currentFrame.SecondRoll.Value < BowlingConstants.FullPoints
                )
                {
                    currentFrame.FrameComplete = true;
                }
            }
            else if (_gameState.CurrentRoll == RollNumber.Three)
            {
                _gameState.LastFrameExtraRoll = value;
                currentFrame.FrameComplete = true;
            }

            _scoringEngine.CalculateScores(_gameState.Frames, _gameState.LastFrameExtraRoll);
            _gameState.RunningTotal = 
                _gameState.Frames.Any(a => a.Score.HasValue) ?
                _gameState.Frames.Where(a => a.Score.HasValue).Max(a => a.Score.Value)
                : 0;

            if (!currentFrame.FrameComplete)
            {
                UpdateRollTracker();
                return _gameState;
            }

            // frame is complete, advancing CurrentFrameNumber and resetting CurrentRoll
            _gameState.CurrentFrameNumber++;
            _gameState.CurrentRoll = RollNumber.One;
            return _gameState;
        }

        private void UpdateRollTracker()
        {
            switch (_gameState.CurrentRoll)
            {
                case RollNumber.One:
                    _gameState.CurrentRoll = RollNumber.Two;
                    break;

                case RollNumber.Two:
                    _gameState.CurrentRoll = RollNumber.Three;
                    break;

                case RollNumber.Three:
                default:
                    break;
            }
        }

        private void CheckForErrorState(bool isLastFrame, Frame currentFrame)
        {
            if (
                _gameState.CurrentRoll == RollNumber.Three
                && !isLastFrame
            )
            {
                var message = $"Can't get to the third Roll Number unless you are in the last frame. Current Frame: { _gameState.CurrentFrameNumber  }";
                throw new Exception(message);
            }
        }
    }
}