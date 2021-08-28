using Business.Constants;
using Contracts.Bowling;
using Models.Bowling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Engines
{
    public class ScoringEngine : IScoringEngine
    {
        public void CalculateScores(List<Frame> frames, int? extraRoll)
        {
            var completeFramesWithoutScores = frames.Where(a => a.FrameComplete && !a.Score.HasValue);
            foreach (var frame in completeFramesWithoutScores)
            {
                CalculateScore(frames, frame, extraRoll);
            }
        }

        private void CalculateScore(List<Frame> frames, Frame frame, int? extraRoll)
        {
            var previousScore = GetPreviousFrameScore(frames, frame);
            var firstValue = frame.FirstRoll.Value;
            var isStrike = firstValue == BowlingConstants.FullPoints;

            var isLastFrame = frame.Number == BowlingConstants.LastFrame;
            if (isLastFrame)
            {
                frame.Score = CalculateLastFrame(frame, extraRoll, previousScore.Value);
                return;
            }

            if (isStrike)
            {
                frame.Score = CalculateStrikeScore(frames, frame, previousScore);
                return;
            }

            var secondValue = frame.SecondRoll.Value;
            var isSpare = firstValue + secondValue == BowlingConstants.FullPoints;
            frame.Score = isSpare
                ? CalculateSpareScore(frames, frame, previousScore)
                : previousScore.Value + frame.FirstRoll + frame.SecondRoll;
        }

        private int? CalculateSpareScore(List<Frame> frames, Frame frame, int? previousScore)
        {
            if (frame.Number == BowlingConstants.LastFrame)
            {
            }
            var nextFrameFirstValue = GetNextFrameValue(frames, frame);
            if (!nextFrameFirstValue.HasValue)
            {
                return new int?();
            }
            return previousScore.Value + BowlingConstants.FullPoints + nextFrameFirstValue.Value;
        }

        private int? CalculateStrikeScore(List<Frame> frames, Frame frame, int? previousScore)
        {
            var values = CalculateStrikeScore(frames, frame);
            if (!values.OneRollAhead.HasValue || !values.TwoRollAhead.HasValue)
            {
                return new int?();
            }
            return previousScore.Value + BowlingConstants.FullPoints + values.OneRollAhead.Value + values.TwoRollAhead.Value;
        }

        private int? CalculateLastFrame(Frame frame, int? extraRoll, int previousScore)
        {
            if (frame.FirstRoll.HasValue && frame.SecondRoll.HasValue)
            {
                var value = frame.FirstRoll.Value + frame.SecondRoll.Value;
                if (value < BowlingConstants.FullPoints)
                {
                    return previousScore + value;
                }
                else
                {
                    if (extraRoll.HasValue)
                    {
                        return previousScore + value + extraRoll.Value;
                    }
                    return new int?();
                }
            }
            return new int?();
        }

        private (int? OneRollAhead, int? TwoRollAhead) CalculateStrikeScore(List<Frame> frames, Frame frame)
        {
            var isLastFrame = frame.Number == BowlingConstants.LastFrame;
            var isNextToLastFrame = frame.Number == BowlingConstants.LastFrame - 1;
            if (isLastFrame)
            {
                throw new Exception("Shouldn't be here.");
            }
            else if (isNextToLastFrame)
            {
                return CalculateStrikeScoreSecondToLastFrame(frames, frame);
            }
            else
            {
                return CalculateStrikeScoreEarlyFrames(frames, frame);
            }
        }

        private (int? OneRollAhead, int? TwoRollAhead) CalculateStrikeScoreEarlyFrames(List<Frame> frames, Frame frame)
        {
            var nextFrameFirstValue = GetNextFrameValue(frames, frame);
            if (!nextFrameFirstValue.HasValue)
            {
                return (new int?(), new int?());
            }
            var nextFrameIsStrike = nextFrameFirstValue == BowlingConstants.FullPoints;
            var nextFrame = nextFrameIsStrike ? GetNextFrame(frames, frame.Number) : frame;
            return nextFrameIsStrike
                ? (nextFrameFirstValue, GetNextFrameValue(frames, nextFrame))
                : (nextFrameFirstValue, GetNextFrameValue(frames, frame, RollNumber.Two));
        }

        private (int? OneRollAhead, int? TwoRollAhead) CalculateStrikeScoreSecondToLastFrame(List<Frame> frames, Frame frame)
        {
            // get the first two rolls of the last frame
            var nextFrame = GetNextFrame(frames, frame.Number);
            return (nextFrame.FirstRoll, nextFrame.SecondRoll);
        }

        private Frame GetNextFrame(List<Frame> frames, int number)
        {
            var nextNumber = number + 1;
            if (nextNumber == BowlingConstants.LastFrame + 1)
            {
                return null;
            }
            return frames.Single(a => a.Number == nextNumber);
        }

        private int? GetNextFrameValue(List<Frame> frames, Frame frame, RollNumber roll = RollNumber.One)
        {
            var nextFrame = GetNextFrame(frames, frame.Number);
            var result = roll == RollNumber.One ? nextFrame.FirstRoll : nextFrame.SecondRoll;
            return result;
        }

        private int? GetPreviousFrameScore(List<Frame> frames, Frame frame)
        {
            if (frame.Number == 1)
            {
                return 0;
            }
            var previousFrame = frames.Skip(frame.Number - 2).First();
            if (previousFrame == null)
            {
                throw new Exception("Previous Frame cannot be null.");
            }
            return previousFrame.Score;
        }
    }
}