using System.Globalization;
using System.Text;
using System.Text.Json;
using SF6ComboCalculator;
using SF6ComboCalculator.Combo;

// Usage:
//   calculator <character> <combo> [--detailed] [--json] [--level N] [--stocks N]
//
//   (no flags)  -> "Total damage:<n>"        (backward compatible)
//   --detailed  -> per-move table breakdown
//   --json      -> per-move breakdown as JSON (implies --detailed)
//   --level N   -> sets the character level (for level-based moves, e.g. Manon/Jamie)
//   --stocks N  -> sets the number of stocks (for {s} stock-enhanced moves)

var positional = new List<string>();
var detailed = false;
var json = false;
var level = 1;
var stocks = 0;

for (var i = 0; i < args.Length; i++)
{
    var arg = args[i];
    switch (arg)
    {
        case "--detailed":
        case "-d":
            detailed = true;
            break;
        case "--json":
            json = true;
            detailed = true;
            break;
        case "--level":
            level = int.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        case "--stocks":
            stocks = int.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        default:
            positional.Add(arg);
            break;
    }
}

if (positional.Count < 2)
{
    Console.Error.WriteLine(
        "Usage: calculator <character> <combo> [--detailed] [--json] [--level N] [--stocks N]");
    return 1;
}

var character = positional[0];
var combo = positional[1];

var comboParser = ComboParser.From(character);
var result = comboParser.Parse(combo, new CharacterStates { Level = level, NumberOfStocks = stocks });

if (json)
{
    PrintJson();
}
else if (detailed)
{
    PrintTable();
}
else
{
    Console.WriteLine("Total damage:" + result.TotalDamage);
}

return 0;

string[] FlagsOf(ComboStep step)
{
    var flags = new List<string>();
    if (step.IsStarter) flags.Add("starter");
    if (step.IsCounterHit) flags.Add("CH");
    if (step.IsPunishCounter) flags.Add("PC");
    if (step.IsDrEnhanced) flags.Add("DR");
    if (step.Airborne) flags.Add("airborne");
    if (step.IsTargetComboPart) flags.Add("target-combo");
    if (step.ExtraScalingHits > 0) flags.Add($"+{step.ExtraScalingHits} scaling hit(s)");
    return flags.ToArray();
}

void PrintTable()
{
    Console.WriteLine($"{character}  {combo}");
    Console.WriteLine($"level={level} stocks={stocks}   TOTAL: {result.TotalDamage}");
    Console.WriteLine();

    var rows = new List<string[]>
    {
        new[] { "#", "move", "raw", "scaled", "total", "hits", "total hits", "scaling", "flags" }
    };

    var runningTotal = 0;
    var runningHits = 0;
    foreach (var step in result.Steps)
    {
        runningTotal += step.ScaledDamage;
        runningHits += step.RawDamage.Length;
        rows.Add(
        [
            step.Index.ToString(CultureInfo.InvariantCulture),
            step.Notation,
            "[" + string.Join(",", step.RawDamage) + "]",
            step.ScaledDamage.ToString(CultureInfo.InvariantCulture),
            runningTotal.ToString(CultureInfo.InvariantCulture),
            step.RawDamage.Length.ToString(CultureInfo.InvariantCulture),
            runningHits.ToString(CultureInfo.InvariantCulture),
            step.Scaling.ToString("0.00", CultureInfo.InvariantCulture),
            string.Join(" ", FlagsOf(step))
        ]);
    }

    var widths = new int[rows[0].Length];
    foreach (var row in rows)
        for (var c = 0; c < row.Length; c++)
            widths[c] = Math.Max(widths[c], row[c].Length);

    foreach (var row in rows)
    {
        var sb = new StringBuilder();
        for (var c = 0; c < row.Length; c++)
        {
            sb.Append(row[c].PadRight(widths[c] + 2));
        }
        Console.WriteLine(sb.ToString().TrimEnd());
    }
}

void PrintJson()
{
    var payload = new
    {
        character,
        combo,
        level,
        stocks,
        total = result.TotalDamage,
        steps = result.Steps.Select(step => new
        {
            i = step.Index,
            notation = step.Notation,
            raw = step.RawDamage,
            scaled = step.ScaledDamage,
            scaling = step.Scaling,
            scalingAfter = step.ScalingAfter,
            flags = FlagsOf(step)
        })
    };

    var options = new JsonSerializerOptions { WriteIndented = true };
    Console.WriteLine(JsonSerializer.Serialize(payload, options));
}
