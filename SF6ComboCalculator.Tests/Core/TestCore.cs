using Newtonsoft.Json;
using SF6ComboCalculator.Data;
using SF6ComboCalculator.Tests.data;

namespace SF6ComboCalculator.Tests.Core;

public class TestCore
{
    public static string[] GetAllDataFiles()
    {
        var fetcher = new Fetcher();

        return fetcher.GetAllDataFiles();
    }

    public static string[] GetCharacters()
    {
        var fetcher = new Fetcher();
        return fetcher.GetAllCharacters();
    }

    private static readonly Lazy<List<(string CharacterName,
        string Notation,
        int DamageExpected,
        decimal[] ExpectedScaling,
        int Level,
        int Stocks,
        bool Validated)>> _lazyData = new(LoadAllTestData);

    protected static List<(string CharacterName,
        string Notation,
        int DamageExpected,
        decimal[] ExpectedScaling,
        int Level,
        int Stocks,
        bool Validated)> _data => _lazyData.Value;

    private static List<(string CharacterName,
        string Notation,
        int DamageExpected,
        decimal[] ExpectedScaling,
        int Level,
        int Stocks,
        bool Validated)> LoadAllTestData()
    {
        var result = new List<(string, string, int, decimal[], int, int, bool)>();
        var allTestFiles = Directory.GetFiles("./test_data/", "*.json", SearchOption.TopDirectoryOnly);

        foreach (var testFile in allTestFiles)
        {
            var splittedPath = testFile.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var characterName = splittedPath[2].Replace(".json", string.Empty);

            var fileContent = File.ReadAllText(testFile);
            var deserialized = JsonConvert.DeserializeObject<DataModel[]>(fileContent);

            foreach (var data in deserialized)
            {
                result.Add((characterName,
                    data.ComboNotation,
                    data.ExpectedDamage,
                    data.ExpectedScaling,
                    data.Level,
                    data.Stocks,
                    data.Validated));
            }
        }

        return result;
    }

    protected static void GenerateDataForTests()
    {
        _ = _data; // trigger Lazy initialization
    }
}
