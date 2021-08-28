using Models.Bowling;
using System.Collections.Generic;

namespace Business.Factories
{
    public static class GameStateFactory
    {
        public static GameState CreateNewGame(int numberOfFrames)
        {
            var gameState = new GameState();
            gameState.Frames = new List<Frame>();
            for (int i = 0; i < numberOfFrames; i++)
            {
                var frame = new Frame { Number = i + 1 };
                gameState.Frames.Add(frame);
            }
            return gameState;
        }
    }
}