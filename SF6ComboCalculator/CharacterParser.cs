using Newtonsoft.Json;
using SF6ComboCalculator.Interfaces;

namespace SF6ComboCalculator;

public class CharacterParser
{
    public AttackModel[] Parse(string path)
    {
        var content = File.ReadAllText(path);

        var attackModels = JsonConvert.DeserializeObject<AttackModel[]>(content);
        return attackModels;
    }

    public IAttack[] Convert(AttackModel[] models)
    {
        var attacks = new List<IAttack>();
        var adapter = new AttackModelAdapter();
        
        foreach (var model in models)
        {
            attacks.Add(adapter.Adapt(model));
        }

        return attacks.ToArray();
    }
}