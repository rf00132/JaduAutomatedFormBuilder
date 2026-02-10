using System.Security.Cryptography;
using System.Text;

namespace JaduFromJson.Utils;

public static class Checksum
{
    static List<string> GetFilePaths(string formName = "")
    {
        List<string> paths = new List<string>(Directory.GetFiles($"{SaveFiles.BaseFilePath}\\{formName}\\"));
        List<string> folders = new List<string>(Directory.GetDirectories($"{SaveFiles.BaseFilePath}\\{formName}\\"));
        foreach (string folder in folders)
        {
            List<string> files = new List<string>(Directory.GetFiles(folder));
            paths.AddRange(files);
        }
        return paths;
    }

    static string GetSha1ChecksumFromFilePath(string path)
    {
        string read = File.ReadAllText(path);
        return Hash(read);
    }
    
    static string Hash(string input)
        => Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(input)));

    public static string MakeChecksumJson(string formName = "")
    {
        List<string> filePaths = GetFilePaths(formName);
        filePaths = filePaths.Where(path => !path.EndsWith("checksums.json")).ToList();
        
        List<List<string>> checksumPaths = filePaths.Select(path => 
               new List<string>{ path.Replace($"{SaveFiles.BaseFilePath}\\{formName}\\", ""), GetSha1ChecksumFromFilePath(path).ToLower()}
        ).ToList();
        checksumPaths = checksumPaths.Select(path => new List<string> {path[0].Replace("\\", @"\/"), path[1]}).ToList();
        string json = "{\"" + checksumPaths[0][0] + "\":\"" + checksumPaths[0][1]+"\"";
        for (int i = 1; i < checksumPaths.Count; i++)
        {
            json += ",\"" + checksumPaths[i][0] + "\":\"" + checksumPaths[i][1]+"\"";
        }
        json += "}";
        return json;
    }

    
    public static string SaveString(string formName = "")
    {
        string checksum = MakeChecksumJson(formName);
        SaveFiles.SaveString("checksums", checksum, formName);
        return checksum;
    }

}
