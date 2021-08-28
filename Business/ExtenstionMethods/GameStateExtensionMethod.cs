using Models.Bowling;
using System.Linq;

namespace Business.ExtenstionMethods
{
    public static class GameStateExtensionMethod
    {
        public static Frame CurrentFrame(this GameState gameState)
        {
            return gameState.Frames.Skip(gameState.CurrentFrameNumber - 1).First();
        }
    }
}