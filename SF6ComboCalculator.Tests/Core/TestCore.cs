namespace SF6ComboCalculator.Tests.Core;

public class TestCore
{
    public static DirectoryInfo GetSolutionFolder(string currentPath = null)
    {
        var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());

        while (directory != null && directory.GetFiles("*.sln").Length == 0)
        {
            directory = directory.Parent;
        }

        return directory;
    }

    public static string[] GetAllDataFiles()
    {
        var solutionFolder = GetSolutionFolder();
        var dataFolder = Path.Combine(solutionFolder.FullName, "data");
        
        var dataFiles = Directory.GetFiles(dataFolder, "*.json",  SearchOption.AllDirectories);

        return dataFiles;
    }
}