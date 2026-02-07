using Newtonsoft.Json;
using SF6ComboCalculator.Tests.Core;
using SF6ComboCalculator.Tests.data;

namespace SF6ComboCalculator.Tests.CharacterComboValidation._2._0001;

//todo: move the notation and expected damage to a json file so it's easier to add new test 
public class ComboTests : TestCore
{
    private static readonly List<(string, string, string, int)> _data = [];

    private static void GenerateDataForTests()
    {
        if (_data.Count != 0)
        {
            return;
        }

        var allTestFiles = Directory.GetFiles("./test_data/", "*.json", SearchOption.AllDirectories);

        foreach (var testFile in allTestFiles)
        {
            var splittedPath = testFile.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            
            var version = splittedPath[2];
            var characterName = splittedPath[3].Replace(".json", string.Empty);

            var fileContent = File.ReadAllText(testFile);
            var deserialized = JsonConvert.DeserializeObject<DataModel[]>(fileContent);

            foreach (var data in deserialized)
            {
                _data.Add((version, characterName, data.ComboNotation, data.ExpectedDamage));
            }
        }
    }

    // get all the test files
    // determine their version via path
    // determine the character via file name
    // pass the character, version, and combo data via params

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

    // [Theory]
    // [InlineData("5LP (CH), 2MK > 28MK", 1600)]
    // [InlineData("j.HK, 4HP > 236HK", 2400)]
    // [InlineData("j.HP, 5HK > ss.MK > 28HK", 3320)]
    // [InlineData("5HK > ss.MK > 28MK", 2380)]
    // [InlineData("5HK > ss.MK > 28HK", 2520)]
    // [InlineData("5HK (PC) > ss.MK > 28HK", 2700)]
    // [InlineData("2MP (PC), ss.MK > [2]8HK", 2640)]
    // [InlineData("5MP, 2MP > [2]8KK, 236LK, 22HK", 3120)]
    // [InlineData("2MK > DRC 5MP, 4HP > ss.LK > DRC 4HP > ss.HK, j.2MK, j.2MK, j.HP~HP, SA3", 4415)] 
    // [InlineData("2HP > ss.HK, j.2MK~j.2MK > j.236HK(4)",2578)]
    // [InlineData("2HP > ss.HK, j.2MK, j.2MK, j.5HP~j.5HP", 2600)]
    // [InlineData("2HP > ss.HK, j.MP > j.236HK(4)", 2696)]
    // [InlineData("2HP > ss.HK, j.MP > j.236KK", 3200)]
    // [InlineData("2HP > SA2, dl. j.MP(1), 236HK, 22HK", 3810)]
    // [InlineData("2HP > 236HK, 22HK", 2860)]
    // [InlineData("2HP > 236HK, SA3", 5100)]
    // [InlineData("2HP (PC) > ss.MK > 28HK", 3000)]
    // [InlineData("2HP (PC) > ss.MK > DRC 2HP > ss.MK > 28HK", 3673)]
    // [InlineData("2HP (PC) > ss.MK > 28KK, 236LK, SA1", 4100)]
    // [InlineData("2HP (PC) > ss.HK, j.MP > j.SA1", 3660)]
    // [InlineData("2HP (PC) > 236HK, j.2MK > j.236KK", 3440)] 
    // [InlineData("2HP (PC) > 236HK, j.2MK > j.236KK, SA1", 4290)]
    // [InlineData("DR 4HP (PC) > 214HK, 2MP > 28HK", 3620)]
    // [InlineData("DR 4HP (PC) > 214HK, 2MP > 28KK, 236LK, 22HK", 4280)]
    // [InlineData("DR 4HP (PC) > 214HK, 2MP > 28KK, DR 5LP > ss.MK > 22HK", 4097)]
    // [InlineData("DR 4HP (PC) > 214HK, 2MP > DRC 2HP > ss.MK > 46PP, 5MP, 2MP > DRC 2HP > ss.HK, j.2MK, j.2MK, j.HP~HP, SA3", 6604)]
    // public void Combo_does_correct_amount_of_total_damage(string comboNotation, int expectedDamage)
    // {
    //    var result = _comboParser.Parse(comboNotation);
    //    
    //    Assert.Equal(expectedDamage, result.TotalDamage);
    // }

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