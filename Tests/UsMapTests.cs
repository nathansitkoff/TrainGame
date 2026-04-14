using System;
using System.Linq;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class UsMapTests
{
    [Fact]
    public void HasThirtySixCities()
    {
        Assert.Equal(36, Enum.GetValues<City>().Length);
        Assert.Equal(36, UsMap.CityPositions.Count);
    }

    [Fact]
    public void EveryCityHasAPosition()
    {
        foreach (var city in Enum.GetValues<City>())
        {
            Assert.True(UsMap.CityPositions.ContainsKey(city), $"Missing position for {city}");
        }
    }

    [Fact]
    public void AllCityPositionsAreNormalized()
    {
        foreach (var (city, pos) in UsMap.CityPositions)
        {
            Assert.InRange(pos.X, 0f, 1f);
            Assert.InRange(pos.Y, 0f, 1f);
        }
    }

    [Fact]
    public void HasOneHundredRouteSegments()
    {
        // 78 unique connections + 22 double-route partners = 100 total segments
        Assert.Equal(100, UsMap.Routes.Count);
    }

    [Fact]
    public void HasSeventyEightUniqueConnections()
    {
        var unique = UsMap.Routes
            .Select(r => r.CityA < r.CityB ? (r.CityA, r.CityB) : (r.CityB, r.CityA))
            .Distinct()
            .Count();
        Assert.Equal(78, unique);
    }

    [Fact]
    public void HasTwentyTwoDoubleRoutePairs()
    {
        var doubles = UsMap.Routes.Count(r => r.DoublePartnerId.HasValue);
        Assert.Equal(44, doubles); // 22 pairs × 2 routes each
        Assert.Equal(22, doubles / 2);
    }

    [Fact]
    public void DoubleRoutePartnersAreMutual()
    {
        foreach (var route in UsMap.Routes)
        {
            if (route.DoublePartnerId is not int partnerId) continue;
            var partner = UsMap.Routes[partnerId];
            Assert.Equal(route.Id, partner.DoublePartnerId);
            // Partners must connect the same two cities
            Assert.True(
                (route.CityA == partner.CityA && route.CityB == partner.CityB) ||
                (route.CityA == partner.CityB && route.CityB == partner.CityA),
                $"Routes {route.Id} and {partnerId} are paired but connect different cities");
        }
    }

    [Fact]
    public void EveryRouteIdMatchesItsIndex()
    {
        for (int i = 0; i < UsMap.Routes.Count; i++)
        {
            Assert.Equal(i, UsMap.Routes[i].Id);
        }
    }

    [Fact]
    public void EveryCityIsConnectedToAtLeastOneRoute()
    {
        foreach (var city in Enum.GetValues<City>())
        {
            Assert.Contains(UsMap.Routes, r => r.CityA == city || r.CityB == city);
        }
    }

    [Fact]
    public void RouteLengthsAreInScoringRange()
    {
        foreach (var route in UsMap.Routes)
        {
            Assert.InRange(route.Length, 1, 6);
        }
    }

    // --- Spot checks against the published board ---

    [Fact]
    public void DenverKansasCityIsBlackOrangeDoubleLength4()
    {
        var pair = UsMap.Routes
            .Where(r => (r.CityA == City.Denver && r.CityB == City.KansasCity) ||
                        (r.CityA == City.KansasCity && r.CityB == City.Denver))
            .ToList();
        Assert.Equal(2, pair.Count);
        Assert.All(pair, r => Assert.Equal(4, r.Length));
        Assert.Contains(pair, r => r.Color == RouteColor.Black);
        Assert.Contains(pair, r => r.Color == RouteColor.Orange);
        Assert.All(pair, r => Assert.NotNull(r.DoublePartnerId));
    }

    [Fact]
    public void VancouverSeattleIsGrayDoubleLength1()
    {
        var pair = UsMap.Routes
            .Where(r => (r.CityA == City.Vancouver && r.CityB == City.Seattle) ||
                        (r.CityA == City.Seattle && r.CityB == City.Vancouver))
            .ToList();
        Assert.Equal(2, pair.Count);
        Assert.All(pair, r =>
        {
            Assert.Equal(1, r.Length);
            Assert.Equal(RouteColor.Gray, r.Color);
            Assert.NotNull(r.DoublePartnerId);
        });
    }

    [Fact]
    public void LosAngelesElPasoIsBlackSingleLength6()
    {
        var routes = UsMap.Routes
            .Where(r => (r.CityA == City.LosAngeles && r.CityB == City.ElPaso) ||
                        (r.CityA == City.ElPaso && r.CityB == City.LosAngeles))
            .ToList();
        Assert.Single(routes);
        Assert.Equal(6, routes[0].Length);
        Assert.Equal(RouteColor.Black, routes[0].Color);
        Assert.Null(routes[0].DoublePartnerId);
    }

    [Fact]
    public void NashvilleAtlantaIsGraySingleLength1()
    {
        var routes = UsMap.Routes
            .Where(r => (r.CityA == City.Nashville && r.CityB == City.Atlanta) ||
                        (r.CityA == City.Atlanta && r.CityB == City.Nashville))
            .ToList();
        Assert.Single(routes);
        Assert.Equal(1, routes[0].Length);
        Assert.Equal(RouteColor.Gray, routes[0].Color);
    }
}
