namespace TrainGame.Model;

/// <summary>
/// A claimable route segment between two cities.
/// <see cref="DoublePartnerId"/> is the Id of the parallel route on
/// double-track edges, or null for single routes.
/// </summary>
public sealed record Route(
    int Id,
    City CityA,
    City CityB,
    int Length,
    RouteColor Color,
    int? DoublePartnerId);
