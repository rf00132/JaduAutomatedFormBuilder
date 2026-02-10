using System.IO.Compression;
using System.Text.Json;

namespace JaduFromJson.Utils;
public static class SaveFiles
{
    public static readonly string BaseFilePath = "FormRequestOutput";

    public static void Save(string fileName, object json, string formName = "")
    {
        fileName = fileName.Replace("/", "");
        string path = BaseFilePath + "\\" + formName;
        string contents = JsonSerializer.Serialize(json);
        File.WriteAllText(path + "\\" + fileName + ".json", contents);
    }
    public static void SaveFormJson(string fileName, object json, string formName = "")
    {
        fileName = fileName.Replace("/", "");
        string path = BaseFilePath + "\\" + formName;
        string contents = JsonSerializer.Serialize(json);
        contents = contents.Replace(@"text/html", "text\\/html");
        File.WriteAllText(path + "\\" + fileName + ".json", contents);
    }

    public static void SaveString(string fileName, string jsonAsString, string? filePath = null, string formName = "")
    {
        fileName = fileName.Replace("/", "");
        if (filePath != null) fileName = filePath + "\\" + fileName;
        string path = $"{BaseFilePath}\\{formName}";
        File.WriteAllText(path +  "\\" + fileName.Replace("?","") + ".json", jsonAsString);
    }

    public static void MakeDirectories(string formName = "")
    {
        string path = BaseFilePath + "\\" + formName;
        Directory.CreateDirectory(path);
        Directory.CreateDirectory(path + "\\page-templates");
        Directory.CreateDirectory(path + "\\misc");
        Directory.CreateDirectory(path + "\\structure");
        Directory.CreateDirectory(path + "\\actions\\templates");
        Directory.CreateDirectory(path + "\\actions\\rules");
    }

   public static void ZipFiles(string formName = "")
    {
        string sourceDirectoryPath = Path.Combine(BaseFilePath, formName);
        string miscFolderPath = "misc";
        string zipFilePath = $"{sourceDirectoryPath}.zip";
        
        // Delete existing zip file if it exists
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }
        
        // Create the zip file using ZipArchive for more control
        using (FileStream zipToCreate = new FileStream(zipFilePath, FileMode.Create))
        {
            using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
            {
                // Add files from the form directory
                AddDirectoryToZip(archive, sourceDirectoryPath, "");
                
                // Add files from the misc folder if it exists
                if (Directory.Exists(miscFolderPath))
                {
                    AddDirectoryToZip(archive, miscFolderPath, "misc");
                }
            }
        }
        
        // Delete the source directory after creating the zip file with retry logic
        if (Directory.Exists(sourceDirectoryPath))
        {
            try
            {
                DeleteDirectoryWithRetry(sourceDirectoryPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not delete directory {sourceDirectoryPath}: {ex.Message}");
            }
        }
    }
    
    private static void DeleteDirectoryWithRetry(string directoryPath, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                // Remove read-only attributes from all files
                foreach (string file in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
                
                Directory.Delete(directoryPath, true);
                return;
            }
            catch (IOException) when (i < maxRetries - 1)
            {
                Thread.Sleep(100); 
            }
        }
    }
    
    private static void AddDirectoryToZip(ZipArchive archive, string directoryPath, string entryPrefix)
    {
        foreach (string filePath in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
        {
            string relativePath = filePath.Substring(directoryPath.Length).TrimStart('\\', '/');
            string entryName = string.IsNullOrEmpty(entryPrefix) 
                ? relativePath 
                : Path.Combine(entryPrefix, relativePath).Replace('\\', '/');
                
            archive.CreateEntryFromFile(filePath, entryName);
        }
    }
}