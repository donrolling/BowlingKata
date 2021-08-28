using Models.Bowling;

namespace Business
{
    public interface IBowlingEngine
    {
        GameState Roll(int value);
    }
}