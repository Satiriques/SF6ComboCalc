using System.Globalization;
using System.Text.Json;
using SF6ComboCalculator;
using SF6ComboCalculator.Combo;
using SF6ComboCalculator.ConsoleApp;

// Usage:
//   calculator <character> <combo> [--detailed] [--json] [--level N] [--stocks N]
//   calculator <character> [combo] --interactive [--level N] [--stocks N]
//
//   (no flags)    -> "Total damage:<n>"        (backward compatible)
//   --detailed    -> per-move table breakdown
//   --json        -> per-move breakdown as JSON (implies --detailed)
//   --interactive -> live combo builder: type a combo and watch the table update,
//                    Tab-complete moves, ghost-text suggestions (combo arg optional)
//   --level N     -> sets the character level (for level-based moves, e.g. Manon/Jamie)
//   --stocks N    -> sets the number of stocks (for {s} stock-enhanced moves)

var positional = new List<string>();
var detailed = false;
var json = false;
var interactive = false;
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
        case "--interactive":
        case "-i":
            interactive = true;
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

if (interactive)
{
    if (positional.Count < 1)
    {
        Console.Error.WriteLine(
            "Usage: calculator <character> [combo] --interactive [--level N] [--stocks N]");
        return 1;
    }

    var seed = positional.Count >= 2 ? positional[1] : string.Empty;
    return new InteractiveSession(positional[0], level, stocks, seed).Run();
}

if (positional.Count < 2)
{
    Console.Error.WriteLine(
        "Usage: calculator <character> <combo> [--detailed] [--json] [--interactive] [--level N] [--stocks N]");
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

string[] FlagsOf(ComboStep step) => TableRenderer.FlagsOf(step);

void PrintTable()
{
    foreach (var line in TableRenderer.Render(character, combo, level, stocks, result))
        Console.WriteLine(line);
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
