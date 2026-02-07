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
    
    protected static readonly List<(string, string, string, int)> _data = [];
    
    
    protected static void GenerateDataForTests()
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
}