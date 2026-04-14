namespace TrainGame.Logic;

/// <summary>
/// Injectable randomness so that tests and replays can be deterministic.
/// </summary>
public interface IRandom
{
    int Next(int maxExclusive);
}
