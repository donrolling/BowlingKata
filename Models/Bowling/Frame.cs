namespace Models.Bowling
{
    public class Frame
    {
        public int Number { get; set; }

        public int? FirstRoll { get; set; }

        public int? Score { get; set; }

        public int? SecondRoll { get; set; }

        public bool FrameComplete { get; set; }
    }
}