// See https://aka.ms/new-console-template for more information

using SF6ComboCalculator;

var comboParser = ComboParser.From(args[0]);

var combo = comboParser.Parse(args[1]);
Console.WriteLine("Total damage:" + combo.TotalDamage);
