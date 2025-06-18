using System.Text.Json.Serialization;

namespace ViennaDotNet.DB.Models.Player;

public sealed class Profile
{
    [JsonInclude]
    public int health;
    [JsonInclude]
    public int experience;
    [JsonInclude]
    public int level;
    [JsonInclude]
    public Rubies rubies;

    public Profile()
    {
        health = 20;
        experience = 0;
        level = 1;
        rubies = new Rubies();
    }
}
