using SF6ComboCalculator.Tests.Core;
using Xunit.Sdk;

namespace SF6ComboCalculator.Tests;

public class ComboTests : TestCore
{
    public static IEnumerable<object[]> ComboTestData()
    {
        GenerateDataForTests();
        
        return _data.Select(data => (object[])[data.Item1, data.Item2, data.Item3, data.Item4]);
    }

    [Theory]
    [MemberData(nameof(ComboTestData))]
    public void Combo_does_correct_amount_of_total_damage(string version, string characterName, string notation,
        int damageExpected)
    {
        var comboParser = ComboParser.From(characterName, version);
        
        var result = comboParser.Parse(notation);
        
        Assert.Equal(damageExpected, result.TotalDamage);
    }

    // todo: add a new key value in the existing test jsons to do this test
    // public static TheoryData<string, decimal[]> ComboScalingTestData =>
    //     new()
    //     {
    //         // this means that for this combo, we expect that:
    //         // 1st attack has 100% scaling
    //         // 2nd attack has 80% scaling => 5HK has 20% starter scaling, otherwise this would be 100%
    //         // 3rd attack has 70% scaling
    //         { "5HK > ss.MK > 28HK", [1M, .8M, .7M] },
    //         { "5MP, 2MP > [2]8KK, 236LK, 22HK", [1M, 1M, .8M, .7M, .6M] },
    //         { "2MK > DRC 5MP, 4HP > ss.LK > 28MK", [1M, .68M, .59M, .51M, .42M] },
    //         {
    //             "2MK > DRC 5MP, 4HP > ss.LK > DRC 4HP > ss.HK, j.2MK, j.2MK, j.HP~HP, SA3",
    //             [1M, .68M, .59M, .51M, .42M, .34M, .25M, .17M, .08M, .08M, .5M]
    //         },
    //         { "2HP (PC) > ss.MK > 28HK", [1M, 1M, .8M] }
    //     };
    //
    // [Theory]
    // [MemberData(nameof(ComboScalingTestData))]
    // public void Combo_does_correct_amount_of_scaling_per_attack(string comboNotation, decimal[] expectedScaling)
    // {
    //     var comboParser = ComboParser.From(characterName, version);
    //     
    //     var result = _comboParser.Parse(comboNotation);
    //
    //     Assert.Equal(expectedScaling, result.ScalingPerAttack);
    // }
}