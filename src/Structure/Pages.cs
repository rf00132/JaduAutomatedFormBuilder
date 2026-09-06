using System.Text.Json;
using System.Text.Json.Serialization;
using JaduFromJson.Utils;

namespace JaduFromJson.Structure;

public class Pages
{
    [JsonPropertyName("pages")] public Page[] JsonPages { private set; get; }

    public Pages(Page[] pages, string formName = "")
    {
        JsonPages = pages;
        Save(formName);
    }

    public string BuildPagesJsonString()
    {
        string pagesJsonString = "[";
        foreach (Page page in JsonPages)
        {
            pagesJsonString += JsonSerializer.Serialize(page);
            if (JsonPages.ToList().IndexOf(page) != JsonPages.Count() - 1)
            {
                pagesJsonString += ",";
            }
        }

        pagesJsonString += "]";
        return pagesJsonString;
    }

    public void Save(string formName = "")
    {
        if (formName == "")
        {
            SaveFiles.SaveString("pages", BuildPagesJsonString(), "structure");
        }
        else
        {
            SaveFiles.SaveString("pages", BuildPagesJsonString(), "structure", formName);
        }
    }
}

public class Page(int id, int templateId, Metadata pageMetadata)
{
    [JsonPropertyName("id")]
    public int Id { private set; get;} = id;
    [JsonPropertyName("metadata")]
    public Metadata Metadata { private set; get;} = pageMetadata;
    [JsonPropertyName("templateID")]
    public int TemplateId { private set; get;} = templateId;
}