using SF6ComboCalculator.Data;
using SF6ComboCalculator.Serialization;
using SF6ComboCalculator.Tests.Core;
using Xunit.Sdk;

namespace SF6ComboCalculator.Tests;

// checks the test data and the character data to see if we are missing 
// tests on certain stuff, for instance if a move has a start scaling, 
// we want to make sure that at least one of the test data is testing
// that
public class MissingValidationTests : TestCore
{
    private static IEnumerable<(string Character, string Version, AttackModel Attack)>? _characterMovesCache;

    public MissingValidationTests()
    {
        GenerateDataForTests();
    }

    public static IEnumerable<object[]> CharacterMoves()
    {
        foreach (var charactermove in GetCharacterMoves())
        {
            yield return [charactermove.Character, charactermove.Version, charactermove.Attack];
        }
    }

    public static IEnumerable<object[]> CharacterMovesWithStarterScaling()
    {
        foreach (var charactermove in GetCharacterMoves().Where(x => x.Attack.StarterScaling != default))
        {
            yield return [charactermove.Character, charactermove.Version, charactermove.Attack];
        }
    }
    
    public static IEnumerable<object[]> CharacterMovesWithImmediateScaling()
    {
        foreach (var charactermove in GetCharacterMoves().Where(x => x.Attack.ImmediateScaling != default))
        {
            yield return [charactermove.Character, charactermove.Version, charactermove.Attack];
        }
    }
    
    public static IEnumerable<object[]> CharacterMovesWithComboScaling()
    {
        foreach (var charactermove in GetCharacterMoves().Where(x => x.Attack.ComboScaling != default))
        {
            yield return [charactermove.Character, charactermove.Version, charactermove.Attack];
        }
    }

    private static IEnumerable<(string Character, string Version, AttackModel Attack)> GetCharacterMoves()
    {
        if (_characterMovesCache != null) return _characterMovesCache;
        
        var cache = new List<(string Character, string Version, AttackModel Attack)>();
        var versions = GetVersions();
        foreach (var version in versions)
        {
            var characters = GetCharacters(version);
            foreach (var character in characters)
            {
                var attacks = GetMoves(character, version);
                foreach (var attack in attacks)
                {
                    cache.Add((character, version, attack));
                }
            }
        }

        _characterMovesCache = cache;
        return _characterMovesCache;
    }

    private static AttackModel[] GetMoves(string character, string version)
    {
        var fetcher = new Fetcher();

        return fetcher.FetchAttacks(character, version);
    }

    [Theory]
    [MemberData(nameof(CharacterMovesWithImmediateScaling))]
    public void Move_with_immediate_scaling_should_be_tested(string character, string version, AttackModel attackModel)
    {
        if (!_data.Any(x => x.CharacterName == character && x.Version == version))
        {
            throw new SkipException($"no test found for : {character} version {version}");
        }

        var possibleNotation = attackModel.Aliases.ToList();
        possibleNotation.Add(attackModel.Notation);

        bool testContainsAttack = _data.Any(test => possibleNotation
            .Any(x => test.Notation.Contains(x, StringComparison.InvariantCultureIgnoreCase)));

        Assert.True(testContainsAttack);
    }
    
    [Theory]
    [MemberData(nameof(CharacterMovesWithComboScaling))]
    public void Move_with_combo_scaling_should_be_tested(string character, string version, AttackModel attackModel)
    {
        if (!_data.Any(x => x.CharacterName == character && x.Version == version))
        {
            throw new SkipException($"no test found for : {character} version {version}");
        }

        var possibleNotation = attackModel.Aliases.ToList();
        possibleNotation.Add(attackModel.Notation);

        // since the scaling applies to the next attack, we need to make sure that it doesn't end with it
        bool testContainsAttack = _data.Any(test => possibleNotation
            .Any(x => test.Notation.Contains(x, StringComparison.InvariantCultureIgnoreCase) && 
                      !test.Notation.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)));

        Assert.True(testContainsAttack);
    }
    
    [Theory]
    [MemberData(nameof(CharacterMovesWithStarterScaling))]
    public void Move_with_starter_scaling_should_be_tested(string character, string version, AttackModel attackModel)
    {
        if (!_data.Any(x => x.CharacterName == character && x.Version == version))
        {
            throw new SkipException($"no test found for : {character} version {version}");
        }

        var possibleNotation = attackModel.Aliases.ToList();
        possibleNotation.Add(attackModel.Notation);

        bool testContainsAttack = _data.Any(test => possibleNotation
            .Any(x => test.Notation.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)));

        Assert.True(testContainsAttack);
    }


    [Theory]
    [MemberData(nameof(CharacterMoves))]
    public void All_moves_should_have_at_least_one_test(string character, string version, AttackModel attackModel)
    {
        if (!_data.Any(x => x.CharacterName == character && x.Version == version))
        {
            throw new SkipException($"no test found for : {character} version {version}");
        }

        var possibleNotation = attackModel.Aliases.ToList();
        possibleNotation.Add(attackModel.Notation);

        bool testContainsAttack = _data.Any(test => possibleNotation
            .Any(x => test.Notation.Contains(x, StringComparison.InvariantCultureIgnoreCase)));

        Assert.True(testContainsAttack);
    }
}