using Models.Bowling;
using System.Collections.Generic;

namespace Contracts.Bowling
{
    public interface IScoringEngine
    {
        void CalculateScores(List<Frame> frames, int? extraRoll);
    }
}