using System.IO.Compression;
using JaduFromJson.Utils;

namespace JaduFromJson;
public static class Program
{
    public static void Main(string[] args)
    {
        Menu();
    }

    static void Menu()
    {
        Console.Clear();
        Console.WriteLine("Please select an option:\n\t1 Batch request Conversion\n\t2 Generate CXM case from XFP export (In development)\n\t3 Close Application");
        Console.Write("\nSelect:\t");
        string? menuOption = Console.ReadLine();
        switch (menuOption)
        {
            case "1":
                List<string> list = GetFilePaths("FormRequestInput");
                foreach(string file in list)
                {
                    string readAll = File.ReadAllText(file);
                    RequestToImport.BuildImport(readAll);
                }
                break;
            case "2":
                string path = "testOutput";
                List<string> list2 = GetFilePaths(path);
                foreach(string formPath in list2)
                {
                    Console.WriteLine("Starting generation for: " + formPath);
                    var formName = formPath.Replace(".zip", "");
                    string zipPath = $"{formPath}";
                    string extractPath = $"{formName}";
                    if (File.Exists(zipPath))
                    {
                        Directory.CreateDirectory(extractPath);
                        ZipFile.ExtractToDirectory(zipPath, extractPath, true);
                        SafeDeleteDirectory(extractPath);
                        Console.WriteLine($"CXM generated for {formName}");
                    }
                    else
                    {
                        Console.WriteLine($"Zip file not found: {zipPath}");
                    }
                }
                break;
            case "3":
                Console.WriteLine("Buh bye");
                return;
            default:
                Console.WriteLine("please select a valid option");
                Menu();
                break;

        }
        Console.WriteLine("Would you like to perform another action? y/n");
        string? anythingElse = Console.ReadLine();
        if(anythingElse == "y")
        {
            Menu();
        }
        else
        {
            Console.WriteLine("Cheers bud, see ya l8r");
        }
    }


    static List<string> GetFilePaths(string path = "testInput")
    {
        List<string> paths = new List<string>(Directory.GetFiles(path));
        return paths;
    }
    
    static void SafeDeleteDirectory(string directoryPath)
    {
        try
        {
            // Use cmd to force delete - this bypasses most file locks
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c rmdir /s /q \"{directoryPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                process?.WaitForExit(5000); // Wait max 5 seconds
            }
            
            // Check if deletion worked
            if (Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Warning: Directory still exists after deletion attempt: {directoryPath}");
                Console.WriteLine("You may need to manually delete this directory or close any programs that might be using these files.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not delete directory {directoryPath}: {ex.Message}");
            Console.WriteLine("Try closing Windows Explorer, antivirus software, or any other programs that might be accessing these files.");
        }
    }
    

}