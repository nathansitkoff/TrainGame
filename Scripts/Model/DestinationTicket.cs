namespace TrainGame.Model;

/// <summary>
/// A destination ticket: complete the path between two cities using your
/// claimed routes to score Points, or lose Points if unfinished.
/// </summary>
public sealed record DestinationTicket(int Id, City CityA, City CityB, int Points);
