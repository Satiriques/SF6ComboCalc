using SF6ComboCalculator.Combo;
using SF6ComboCalculator.Tests.Core;
using Xunit.Sdk;

namespace SF6ComboCalculator.Tests;

public class ComboTests : TestCore
{
    public static IEnumerable<object[]> ComboTestData()
    {
        GenerateDataForTests();

        return _data.Select(data => (object[])
            [
                data.Version, 
                data.CharacterName, 
                data.Notation, 
                data.DamageExpected, 
                data.ExpectedScaling,
                new CharacterStates()
                {
                    Level = data.Level,
                    NumberOfStocks = data.Stocks
                }
            ]);
    }

    [Theory]
    [MemberData(nameof(ComboTestData))]
    public void Combo_does_correct_amount_of_total_damage(string version, string characterName, string notation,
        int damageExpected, decimal[] expectedScaling, CharacterStates characterStates)
    {
        var comboParser = ComboParser.From(characterName, version);

        var result = comboParser.Parse(notation, characterStates);

        if (expectedScaling is not null)
        {
            Assert.True(Enumerable.SequenceEqual(expectedScaling, result.ScalingPerAttack));
        }
        Assert.Equal(damageExpected, result.TotalDamage);
    }
}