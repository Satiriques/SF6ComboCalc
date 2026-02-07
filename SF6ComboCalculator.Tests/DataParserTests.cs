using SF6ComboCalculator.Tests.Core;

namespace SF6ComboCalculator.Tests;

// these tests validate the parsing of the json data files
public class DataParserTests : TestCore
{
    public static IEnumerable<object[]> DataJsons()
    {
        var dataFiles = GetAllDataFiles();

        return  dataFiles.Select(x => new object[] { x });
    }
    
    [Theory]
    [MemberData(nameof(DataJsons))]
    public void Can_parse_data_json_files(string path)
    {
        var parser = new CharacterParser();

        var attacks = parser.Parse(path);
        
        Assert.NotNull(attacks);
        Assert.NotEmpty(attacks);
    }
}