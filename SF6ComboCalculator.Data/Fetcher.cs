using System.Reflection;
using Newtonsoft.Json;
using SF6ComboCalculator.Serialization;

namespace SF6ComboCalculator.Data;

public class Fetcher
{
    public AttackModel[] FetchAttacks(string character, string version)
    {
        
        if (!Directory.Exists($"{AppContext.BaseDirectory}/data/{version}/"))
        {
            throw new ArgumentException($"Version specified {version} not found in {Path.GetFullPath("./data/")}");
        }

        if (!File.Exists($"{AppContext.BaseDirectory}/data/{version}/{character}.json"))
        {
            throw new ArgumentException($"Character {character}.json not found  in {Path.GetFullPath($"./data/{version}")}");
        }
        
        var path = $"{AppContext.BaseDirectory}/data/{version}/{character}.json";
        
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
        return Directory.GetFiles("./data", "*.json", SearchOption.AllDirectories);
    }

    public string[] GetAllVersions()
    {
        var folders =Directory.GetDirectories("./data/");
        return folders.Select(Path.GetFileName).ToArray();
    }

    public string[] GetAllCharacters(string version)
    {
        var files = Directory.GetFiles($"./data/{version}");
        return files.Select(Path.GetFileNameWithoutExtension).ToArray();
    }
}