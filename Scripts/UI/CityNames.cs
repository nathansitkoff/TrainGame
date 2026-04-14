using TrainGame.Model;

namespace TrainGame.UI;

public static class CityNames
{
    public static string Display(City c) => c switch
    {
        City.ElPaso       => "El Paso",
        City.KansasCity   => "Kansas City",
        City.LasVegas     => "Las Vegas",
        City.LittleRock   => "Little Rock",
        City.LosAngeles   => "Los Angeles",
        City.NewOrleans   => "New Orleans",
        City.NewYork      => "New York",
        City.OklahomaCity => "Oklahoma City",
        City.SaintLouis   => "Saint Louis",
        City.SaltLakeCity => "Salt Lake City",
        City.SanFrancisco => "San Francisco",
        City.SantaFe      => "Santa Fe",
        City.SaultStMarie => "Sault St. Marie",
        _                 => c.ToString(),
    };
}
