using System.Collections.Generic;

namespace Models.Bowling
{
    public class GameState
    {
        public int RunningTotal { get; set; } = 0;

        public int CurrentFrameNumber { get; set; } = 1;

        public RollNumber CurrentRoll { get; set; } = RollNumber.One;

        public List<Frame> Frames { get; set; }

        public int? LastFrameExtraRoll { get; set; }
    }
}