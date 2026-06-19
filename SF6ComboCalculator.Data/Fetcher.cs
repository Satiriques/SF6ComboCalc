using Newtonsoft.Json;
using SF6ComboCalculator.Serialization;

namespace SF6ComboCalculator.Data;

public class Fetcher
{
    public AttackModel[] FetchAttacks(string character)
    {
        if (!File.Exists($"{AppContext.BaseDirectory}/data/{character}.json"))
        {
            throw new ArgumentException($"Character {character}.json not found in {Path.GetFullPath("./data/")}");
        }

        var path = $"{AppContext.BaseDirectory}/data/{character}.json";

        var content = File.ReadAllText(path);
        var json = JsonConvert.DeserializeObject<AttackModel[]>(content);

        if (json == null)
        {
            throw new Exception($"Could not serialize {path}");
        }

        return json;
    }

    public string[] GetAllDataFiles()
    {
        return Directory.GetFiles("./data", "*.json", SearchOption.TopDirectoryOnly);
    }

    public string[] GetAllCharacters()
    {
        var files = Directory.GetFiles("./data", "*.json", SearchOption.TopDirectoryOnly);
        return files.Select(Path.GetFileNameWithoutExtension).ToArray();
    }
}
