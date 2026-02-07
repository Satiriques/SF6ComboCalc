using System.Text.RegularExpressions;
using SF6ComboCalculator.Interfaces;

namespace SF6ComboCalculator;

public class ComboParser
{
    private readonly string _dataJsonPath;
    private readonly CharacterParser _characterParser;
    private readonly AttackModel[] _attacks;

    public ComboParser(string dataJsonPath)
    {
        _dataJsonPath = dataJsonPath;
        _characterParser =  new CharacterParser();
        _attacks = _characterParser.Parse(dataJsonPath);
    }


    public ComboParserResult Parse(string comboNotation)
    {
        List<IAttack> unwrapedCombo = [];
        List<int> damagePerAttack = [];
        List<decimal> scalingPerAttack = [];
        var numberOfHitRegex = new Regex(@"\((\d+)\)");
        
        var splittedString = comboNotation.Split('>', ',');
        var adapte = new AttackModelAdapter();

        for (var index = 0; index < splittedString.Length; index++)
        {
            var str = splittedString[index];
            var (cleanString, isDrEnhanced, numberOfHits, isPunishCounter, isCounterHit) =
                CleanupAttackString(str, numberOfHitRegex, index);

            var attack = adapte.Adapt(FindAttack(cleanString));
            attack.NumberOfHits = numberOfHits;

            var attacksToAdd = new List<IAttack>();

            if (attack.IsTargetCombo)
            {
                attacksToAdd.AddRange(DecomposeTargetCombo(cleanString));
            }
            else
            {
                attacksToAdd.Add(attack);
            }

            attacksToAdd[0].IsDrEnhanced = isDrEnhanced;
            attacksToAdd[0].IsPunishCounter = isPunishCounter;
            attacksToAdd[0].IsCounterHit = isCounterHit;
            unwrapedCombo.AddRange(attacksToAdd);
        }

        var combo = unwrapedCombo;
        
        // calculate the damage
        decimal baseScaling = 1;
        int totalDamage = 0;
        var hasStarterScaling = false;
        var drScaling = 1m;
        bool airborne = false;

        for (var index = 0; index < combo.Count; index++)
        {
            var counterScaling = 1m;
            var attack = combo[index];

            if (attack.IsDrEnhanced)
            {
                drScaling = .85M; // 15% penalty
            }

            if (attack.IsCounterHit || attack.IsPunishCounter)
            {
                // todo: not sure yet if it multiplicative with the dr scaling, need to check
                // test with 5HK (starter scaling)
                counterScaling = 1.2M;
            }
            
            var damage = attack.CalculateDamage(baseScaling * drScaling, airborne);
            totalDamage += damage;

            damagePerAttack.Add(damage);
            scalingPerAttack.Add(attack.CalculateScaling(baseScaling * drScaling));
            var numberOfScalingHits = 1 + attack.NumberOfExtraScalingHits;
            
            if (index == 0)
            {
                baseScaling -= attack.StarterScaling;
                hasStarterScaling = attack.StarterScaling != 0m;
            }
            else if (index == 1 && !hasStarterScaling)
            {
                numberOfScalingHits++;
                baseScaling -= numberOfScalingHits * .1m;
            }
            else
            {
                baseScaling -= numberOfScalingHits * .1m;
            }

            if (attack.MakesAirborne)
            {
                airborne = true;
            }
            
            baseScaling = Math.Max(0.1m, baseScaling);
        }

        return new ComboParserResult()
        {
            Combo = unwrapedCombo,
            TotalDamage = totalDamage,
            DamagePerAttack = damagePerAttack,
            ScalingPerAttack = scalingPerAttack
        };
    }

    private IAttack[] DecomposeTargetCombo(string cleanString)
    {
        var decomposedInputs = new List<string>();
        var adapter = new AttackModelAdapter();
        
        // MP~HP~HK
        // => MP > MP~HP > MP~HP~HK
        var parts = cleanString.Split('~');

        for (int i = 0; i < parts.Length; i++)
        {
            decomposedInputs.Add(string.Join("~", parts.Take(i+1)));
        }

        return decomposedInputs.Select(x=> adapter.Adapt( FindAttack(x))).ToArray();
    }

    private AttackModel FindAttack(string notation)
    {
        var attack =
            _attacks.FirstOrDefault(x => x.Notation.Equals(notation, StringComparison.InvariantCultureIgnoreCase));

        if (attack is null)
        {
            attack = _attacks.FirstOrDefault(x =>
                x.Aliases.Contains(notation, StringComparer.InvariantCultureIgnoreCase));
        }

        return attack ?? throw new ArgumentException($"attack with notation or alias  \"{notation}\" was not found in the json.");
    }

    private static (string CleanString, 
                    bool IsDrEnhanced, 
                    int? NumberOfHits,
                    bool IsPunishCounter,
                    bool IsCounterHit) CleanupAttackString(string str, Regex numberOfHitRegex, int index)
    {
        int? numberOfHits = null;
        bool isPunishCounter = false, isCounterHit = false;
        var isDrEnhanced = str.Contains("DRC") || (str.Contains("DR") && index != 0);
        var cleanString = str.Replace("DRC", string.Empty)
            .Replace("dl.", string.Empty)
            .Replace("DR", string.Empty);

        if (numberOfHitRegex.IsMatch(cleanString))
        {
            numberOfHits = int.Parse(numberOfHitRegex.Match(cleanString).Groups[1].Value);
            cleanString = numberOfHitRegex.Replace(cleanString, string.Empty);
        }

        if (cleanString.Contains("(PC)"))
        {
            isPunishCounter = true;
            cleanString = cleanString.Replace("(PC)", string.Empty);
        }
        
        if (cleanString.Contains("(CH)"))
        {
            isCounterHit = true;
            cleanString = cleanString.Replace("(CH)", string.Empty);
        }

        return (cleanString.Trim(), isDrEnhanced, numberOfHits, isPunishCounter, isCounterHit);
    }
}