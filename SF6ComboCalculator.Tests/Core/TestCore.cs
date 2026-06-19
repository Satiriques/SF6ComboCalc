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

    protected static readonly List<(string CharacterName,
        string Notation,
        int DamageExpected,
        decimal[] ExpectedScaling,
        int Level,
        int Stocks,
        bool Validated)> _data = [];


    protected static void GenerateDataForTests()
    {
        if (_data.Count != 0)
        {
            return;
        }

        var allTestFiles = Directory.GetFiles("./test_data/", "*.json", SearchOption.TopDirectoryOnly);

        foreach (var testFile in allTestFiles)
        {
            var splittedPath = testFile.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            var characterName = splittedPath[2].Replace(".json", string.Empty);

            var fileContent = File.ReadAllText(testFile);
            var deserialized = JsonConvert.DeserializeObject<DataModel[]>(fileContent);

            foreach (var data in deserialized)
            {
                _data.Add((characterName,
                    data.ComboNotation,
                    data.ExpectedDamage,
                    data.ExpectedScaling,
                    data.Level,
                    data.Stocks,
                    data.Validated));
            }
        }
    }
}
