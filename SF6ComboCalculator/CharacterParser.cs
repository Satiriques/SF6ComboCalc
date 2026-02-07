using Newtonsoft.Json;
using SF6ComboCalculator.Interfaces;
using SF6ComboCalculator.Serialization;

namespace SF6ComboCalculator;

public class CharacterParser
{
    public AttackModel[] Parse(string path)
    {
        var content = File.ReadAllText(path);

        var attackModels = JsonConvert.DeserializeObject<AttackModel[]>(content);

        return attackModels ?? throw new ArgumentException(null, nameof(path));
    }
}