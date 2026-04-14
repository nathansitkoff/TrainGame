using System.Collections.Generic;

namespace TrainGame.Model;

/// <summary>
/// Static data for the 2004 US Ticket to Ride map: city positions and
/// all route segments. Positions are normalized [0,1] relative to the
/// board image <c>Resources/Board/us_map.jpg</c>; the view multiplies
/// them by its current render size so the board can scale freely.
/// Double-track edges appear as two Route entries whose DoublePartnerId
/// fields point at each other.
/// </summary>
public static class UsMap
{
    public static IReadOnlyList<Route> Routes { get; }
    public static IReadOnlyDictionary<City, NormPos> CityPositions { get; }

    static UsMap()
    {
        CityPositions = BuildCityPositions();
        Routes = BuildRoutes();
    }

    private static IReadOnlyDictionary<City, NormPos> BuildCityPositions()
    {
        // Positions transcribed from the normalized coordinates published
        // in the Rob217/TicketToRideAnalysis dataset (CC-BY). Original Y
        // axis ran bottom-to-top; we store Y top-to-bottom for Godot.
        return new Dictionary<City, NormPos>
        {
            [City.Atlanta]      = new(0.7806f, 0.6331f),
            [City.Boston]       = new(0.9452f, 0.2064f),
            [City.Calgary]      = new(0.2347f, 0.1285f),
            [City.Charleston]   = new(0.8719f, 0.6436f),
            [City.Chicago]      = new(0.6839f, 0.4036f),
            [City.Dallas]       = new(0.5545f, 0.7790f),
            [City.Denver]       = new(0.3903f, 0.5489f),
            [City.Duluth]       = new(0.5636f, 0.3142f),
            [City.ElPaso]       = new(0.3777f, 0.8146f),
            [City.Helena]       = new(0.3327f, 0.3225f),
            [City.Houston]      = new(0.5954f, 0.8371f),
            [City.KansasCity]   = new(0.5549f, 0.5228f),
            [City.LasVegas]     = new(0.2075f, 0.6645f),
            [City.LittleRock]   = new(0.6233f, 0.6556f),
            [City.LosAngeles]   = new(0.1447f, 0.7497f),
            [City.Miami]        = new(0.9037f, 0.8747f),
            [City.Montreal]     = new(0.8754f, 0.1223f),
            [City.Nashville]    = new(0.7310f, 0.5814f),
            [City.NewOrleans]   = new(0.6860f, 0.8198f),
            [City.NewYork]      = new(0.8939f, 0.3163f),
            [City.OklahomaCity] = new(0.5354f, 0.6499f),
            [City.Omaha]        = new(0.5347f, 0.4485f),
            [City.Phoenix]      = new(0.2616f, 0.7597f),
            [City.Pittsburgh]   = new(0.8116f, 0.3821f),
            [City.Portland]     = new(0.0823f, 0.3095f),
            [City.Raleigh]      = new(0.8447f, 0.5479f),
            [City.SaintLouis]   = new(0.6383f, 0.5254f),
            [City.SaltLakeCity] = new(0.2626f, 0.5035f),
            [City.SanFrancisco] = new(0.0684f, 0.5976f),
            [City.SantaFe]      = new(0.3830f, 0.6818f),
            [City.SaultStMarie] = new(0.6885f, 0.2174f),
            [City.Seattle]      = new(0.1036f, 0.2331f),
            [City.Toronto]      = new(0.7952f, 0.2483f),
            [City.Vancouver]    = new(0.1081f, 0.1542f),
            [City.Washington]   = new(0.9016f, 0.4491f),
            [City.Winnipeg]     = new(0.4534f, 0.1447f),
        };
    }

    private static IReadOnlyList<Route> BuildRoutes()
    {
        // Raw edge list. Double routes are listed as two consecutive
        // entries with the same (CityA, CityB) pair; BuildRoutes will
        // auto-link their DoublePartnerIds below.
        var raw = new (City a, City b, int len, RouteColor c)[]
        {
            (City.Vancouver,    City.Calgary,      3, RouteColor.Gray),
            (City.Vancouver,    City.Seattle,      1, RouteColor.Gray),
            (City.Vancouver,    City.Seattle,      1, RouteColor.Gray),
            (City.Seattle,      City.Calgary,      4, RouteColor.Gray),
            (City.Seattle,      City.Helena,       6, RouteColor.Yellow),
            (City.Seattle,      City.Portland,     1, RouteColor.Gray),
            (City.Seattle,      City.Portland,     1, RouteColor.Gray),
            (City.Portland,     City.SaltLakeCity, 6, RouteColor.Blue),
            (City.Portland,     City.SanFrancisco, 5, RouteColor.Green),
            (City.Portland,     City.SanFrancisco, 5, RouteColor.Pink),
            (City.SanFrancisco, City.SaltLakeCity, 5, RouteColor.Orange),
            (City.SanFrancisco, City.SaltLakeCity, 5, RouteColor.White),
            (City.SanFrancisco, City.LosAngeles,   3, RouteColor.Yellow),
            (City.SanFrancisco, City.LosAngeles,   3, RouteColor.Pink),
            (City.LosAngeles,   City.LasVegas,     2, RouteColor.Gray),
            (City.LosAngeles,   City.Phoenix,      3, RouteColor.Gray),
            (City.LosAngeles,   City.ElPaso,       6, RouteColor.Black),
            (City.Calgary,      City.Winnipeg,     6, RouteColor.White),
            (City.Calgary,      City.Helena,       4, RouteColor.Gray),
            (City.Helena,       City.Winnipeg,     4, RouteColor.Blue),
            (City.Helena,       City.SaltLakeCity, 3, RouteColor.Pink),
            (City.Helena,       City.Denver,       4, RouteColor.Green),
            (City.Helena,       City.Duluth,       6, RouteColor.Orange),
            (City.Helena,       City.Omaha,        5, RouteColor.Red),
            (City.SaltLakeCity, City.Denver,       3, RouteColor.Red),
            (City.SaltLakeCity, City.Denver,       3, RouteColor.Yellow),
            (City.LasVegas,     City.SaltLakeCity, 3, RouteColor.Orange),
            (City.Phoenix,      City.Denver,       5, RouteColor.White),
            (City.Phoenix,      City.SantaFe,      3, RouteColor.Gray),
            (City.Phoenix,      City.ElPaso,       3, RouteColor.Gray),
            (City.Winnipeg,     City.SaultStMarie, 6, RouteColor.Gray),
            (City.Winnipeg,     City.Duluth,       4, RouteColor.Black),
            (City.Duluth,       City.SaultStMarie, 3, RouteColor.Gray),
            (City.Duluth,       City.Toronto,      6, RouteColor.Pink),
            (City.Duluth,       City.Chicago,      3, RouteColor.Red),
            (City.Duluth,       City.Omaha,        2, RouteColor.Gray),
            (City.Duluth,       City.Omaha,        2, RouteColor.Gray),
            (City.Omaha,        City.Chicago,      4, RouteColor.Blue),
            (City.Omaha,        City.KansasCity,   1, RouteColor.Gray),
            (City.Omaha,        City.KansasCity,   1, RouteColor.Gray),
            (City.KansasCity,   City.SaintLouis,   2, RouteColor.Blue),
            (City.KansasCity,   City.SaintLouis,   2, RouteColor.Pink),
            (City.KansasCity,   City.OklahomaCity, 2, RouteColor.Gray),
            (City.KansasCity,   City.OklahomaCity, 2, RouteColor.Gray),
            (City.OklahomaCity, City.LittleRock,   2, RouteColor.Gray),
            (City.OklahomaCity, City.Dallas,       2, RouteColor.Gray),
            (City.OklahomaCity, City.Dallas,       2, RouteColor.Gray),
            (City.Dallas,       City.LittleRock,   2, RouteColor.Gray),
            (City.Dallas,       City.Houston,      1, RouteColor.Gray),
            (City.Dallas,       City.Houston,      1, RouteColor.Gray),
            (City.Houston,      City.NewOrleans,   2, RouteColor.Gray),
            (City.ElPaso,       City.Houston,      6, RouteColor.Green),
            (City.ElPaso,       City.Dallas,       4, RouteColor.Red),
            (City.ElPaso,       City.OklahomaCity, 5, RouteColor.Yellow),
            (City.ElPaso,       City.SantaFe,      2, RouteColor.Gray),
            (City.SantaFe,      City.OklahomaCity, 3, RouteColor.Blue),
            (City.OklahomaCity, City.Denver,       4, RouteColor.Red),
            (City.SantaFe,      City.Denver,       2, RouteColor.Gray),
            (City.Denver,       City.KansasCity,   4, RouteColor.Black),
            (City.Denver,       City.KansasCity,   4, RouteColor.Orange),
            (City.Denver,       City.Omaha,        4, RouteColor.Pink),
            (City.NewOrleans,   City.Miami,        6, RouteColor.Red),
            (City.NewOrleans,   City.Atlanta,      4, RouteColor.Orange),
            (City.NewOrleans,   City.Atlanta,      4, RouteColor.Yellow),
            (City.NewOrleans,   City.LittleRock,   3, RouteColor.Green),
            (City.LittleRock,   City.Nashville,    3, RouteColor.White),
            (City.LittleRock,   City.SaintLouis,   2, RouteColor.Gray),
            (City.SaintLouis,   City.Nashville,    2, RouteColor.Gray),
            (City.SaintLouis,   City.Pittsburgh,   5, RouteColor.Green),
            (City.SaintLouis,   City.Chicago,      2, RouteColor.Green),
            (City.SaintLouis,   City.Chicago,      2, RouteColor.White),
            (City.Chicago,      City.Pittsburgh,   3, RouteColor.Black),
            (City.Chicago,      City.Pittsburgh,   3, RouteColor.Orange),
            (City.Chicago,      City.Toronto,      4, RouteColor.White),
            (City.SaultStMarie, City.Montreal,     5, RouteColor.Black),
            (City.Toronto,      City.Montreal,     3, RouteColor.Gray),
            (City.SaultStMarie, City.Toronto,      2, RouteColor.Gray),
            (City.Toronto,      City.Pittsburgh,   2, RouteColor.Gray),
            (City.Pittsburgh,   City.NewYork,      2, RouteColor.White),
            (City.Pittsburgh,   City.NewYork,      2, RouteColor.Green),
            (City.Pittsburgh,   City.Washington,   2, RouteColor.Gray),
            (City.Pittsburgh,   City.Raleigh,      2, RouteColor.Gray),
            (City.Nashville,    City.Raleigh,      3, RouteColor.Black),
            (City.Nashville,    City.Atlanta,      1, RouteColor.Gray),
            (City.Nashville,    City.Pittsburgh,   4, RouteColor.Yellow),
            (City.Atlanta,      City.Miami,        5, RouteColor.Blue),
            (City.Atlanta,      City.Charleston,   2, RouteColor.Gray),
            (City.Atlanta,      City.Raleigh,      2, RouteColor.Gray),
            (City.Atlanta,      City.Raleigh,      2, RouteColor.Gray),
            (City.Charleston,   City.Miami,        4, RouteColor.Pink),
            (City.Raleigh,      City.Charleston,   2, RouteColor.Gray),
            (City.Raleigh,      City.Washington,   2, RouteColor.Gray),
            (City.Raleigh,      City.Washington,   2, RouteColor.Gray),
            (City.Washington,   City.NewYork,      2, RouteColor.Orange),
            (City.Washington,   City.NewYork,      2, RouteColor.Black),
            (City.NewYork,      City.Boston,       2, RouteColor.Yellow),
            (City.NewYork,      City.Boston,       2, RouteColor.Red),
            (City.NewYork,      City.Montreal,     3, RouteColor.Blue),
            (City.Boston,       City.Montreal,     2, RouteColor.Gray),
            (City.Boston,       City.Montreal,     2, RouteColor.Gray),
        };

        var routes = new Route[raw.Length];
        for (int i = 0; i < raw.Length; i++)
        {
            var r = raw[i];
            routes[i] = new Route(i, r.a, r.b, r.len, r.c, DoublePartnerId: null);
        }

        // Link double-track partners. Doubles are always consecutive in
        // the raw list, so a simple pairwise scan is enough.
        for (int i = 0; i < routes.Length - 1; i++)
        {
            var a = routes[i];
            var b = routes[i + 1];
            bool sameEdge =
                (a.CityA == b.CityA && a.CityB == b.CityB) ||
                (a.CityA == b.CityB && a.CityB == b.CityA);
            if (sameEdge && a.DoublePartnerId is null && b.DoublePartnerId is null)
            {
                routes[i] = a with { DoublePartnerId = b.Id };
                routes[i + 1] = b with { DoublePartnerId = a.Id };
            }
        }

        return routes;
    }
}
