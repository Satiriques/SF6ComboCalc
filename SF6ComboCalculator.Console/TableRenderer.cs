using System.Globalization;
using System.Text;
using SF6ComboCalculator;

namespace SF6ComboCalculator.ConsoleApp;

/// <summary>
/// Renders a parsed combo into the per-move breakdown table as a list of lines.
/// Shared by the one-shot <c>--detailed</c> output and the live interactive mode.
/// </summary>
internal static class TableRenderer
{
    public static string[] FlagsOf(ComboStep step)
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

    public static List<string> Render(
        string character,
        string combo,
        int level,
        int stocks,
        ComboParserResult result)
    {
        var lines = new List<string>
        {
            $"{character}  {combo}",
            $"level={level} stocks={stocks}   TOTAL: {result.TotalDamage}",
            string.Empty
        };

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
                sb.Append(row[c].PadRight(widths[c] + 2));
            lines.Add(sb.ToString().TrimEnd());
        }

        return lines;
    }
}
