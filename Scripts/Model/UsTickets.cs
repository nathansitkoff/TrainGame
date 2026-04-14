using System.Collections.Generic;

namespace TrainGame.Model;

/// <summary>
/// The 30 destination tickets from the 2004 US Ticket to Ride map.
/// Transcribed from the public Rob217/TicketToRideAnalysis dataset.
/// </summary>
public static class UsTickets
{
    public static IReadOnlyList<DestinationTicket> All { get; } = Build();

    private static IReadOnlyList<DestinationTicket> Build()
    {
        var raw = new (City a, City b, int pts)[]
        {
            (City.LosAngeles,   City.NewYork,       21),
            (City.Duluth,       City.Houston,        8),
            (City.SaultStMarie, City.Nashville,      8),
            (City.NewYork,      City.Atlanta,        6),
            (City.Portland,     City.Nashville,     17),
            (City.Vancouver,    City.Montreal,      20),
            (City.Duluth,       City.ElPaso,        10),
            (City.Toronto,      City.Miami,         10),
            (City.Portland,     City.Phoenix,       11),
            (City.Dallas,       City.NewYork,       11),
            (City.Calgary,      City.SaltLakeCity,   7),
            (City.Calgary,      City.Phoenix,       13),
            (City.LosAngeles,   City.Miami,         20),
            (City.Winnipeg,     City.LittleRock,    11),
            (City.SanFrancisco, City.Atlanta,       17),
            (City.KansasCity,   City.Houston,        5),
            (City.LosAngeles,   City.Chicago,       16),
            (City.Denver,       City.Pittsburgh,    11),
            (City.Chicago,      City.SantaFe,        9),
            (City.Vancouver,    City.SantaFe,       13),
            (City.Boston,       City.Miami,         12),
            (City.Chicago,      City.NewOrleans,     7),
            (City.Montreal,     City.Atlanta,        9),
            (City.Seattle,      City.NewYork,       22),
            (City.Denver,       City.ElPaso,         4),
            (City.Helena,       City.LosAngeles,     8),
            (City.Winnipeg,     City.Houston,       12),
            (City.Montreal,     City.NewOrleans,    13),
            (City.SaultStMarie, City.OklahomaCity,   9),
            (City.Seattle,      City.LosAngeles,     9),
        };

        var tickets = new DestinationTicket[raw.Length];
        for (int i = 0; i < raw.Length; i++)
        {
            tickets[i] = new DestinationTicket(i, raw[i].a, raw[i].b, raw[i].pts);
        }
        return tickets;
    }
}
