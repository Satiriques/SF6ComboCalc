using SF6ComboCalculator.Data;

namespace SF6ComboCalculator.Tests.Core;

public class TestCore
{
    public static string[] GetAllDataFiles()
    {
        var fetcher = new Fetcher();

        return fetcher.GetAllDataFiles();
    }
}