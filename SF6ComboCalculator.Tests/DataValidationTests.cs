using SF6ComboCalculator.Tests.Core;

namespace SF6ComboCalculator.Tests;

// these tests make sure that the json data files are filled out correctly
public class DataValidationTests : TestCore
{
    public static IEnumerable<object[]> DataJsons()
    {
        var dataFiles = GetAllDataFiles();

        return  dataFiles.Select(x => new object[] { x });
    }
    
    [Theory]
    [MemberData(nameof(DataJsons))]
    public void Data_files_dont_have_duplicate_notations(string path)
    {
        var parser = new CharacterParser();
        
        var attacks = parser.Parse(path);
        var numberOfUniqueNotations = attacks.DistinctBy(x => x.Notation).Count();
        var numberOfAttacks = attacks.Count();
        
        Assert.True(numberOfUniqueNotations == numberOfAttacks, 
            $"file {path} has {numberOfAttacks} attacks, but {numberOfUniqueNotations} unique notations");
    }

    [Theory]
    [MemberData(nameof(DataJsons))]
    public void Data_files_dont_have_clashing_alias_and_notation(string path)
    {
        var parser = new CharacterParser();
        
        var attacks = parser.Parse(path);
        var notations = attacks.Select(x => x.Notation);
        var aliases = attacks.SelectMany(x => x.Aliases);
        var numberOfDistinctAliasAndNotation = notations.Union(aliases).Distinct().Count();

        Assert.True(numberOfDistinctAliasAndNotation == notations.Count() + aliases.Count());
    }
}