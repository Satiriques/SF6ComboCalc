using System.Reflection;
using Newtonsoft.Json;
using SF6ComboCalculator.Serialization;

namespace SF6ComboCalculator.Data;

public class Fetcher
{
    public AttackModel[] FetchAttacks(string character, string version)
    {
        var path = $"./data/{version}/{character}.json";
        
        var content = File.ReadAllText(path);
        var json = JsonConvert.DeserializeObject<AttackModel[]>(content);
        
        return json;
    }

    public string[] GetAllDataFiles()
    {
        return Directory.GetFiles("./data", "*.json", SearchOption.AllDirectories);
    }
    
}