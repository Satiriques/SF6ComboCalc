// See https://aka.ms/new-console-template for more information

using SF6ComboCalculator;

var comboParser = ComboParser.From(args[0], args[1]);

var combo = comboParser.Parse(args[2]);
Console.WriteLine("Total damage:" +combo.TotalDamage);