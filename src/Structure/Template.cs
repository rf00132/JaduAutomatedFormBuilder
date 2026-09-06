using System.Text.Json;
using System.Text.Json.Serialization;
using JaduFromJson.Utils;

namespace JaduFromJson.Structure;

public class Template
{
    [JsonPropertyName("id")] public int Id { private set; get; }
    [JsonPropertyName("liveVersion")] public LiveVersion LiveVersion { private set; get; }
    [JsonPropertyName("formID")] public int? FormId { private set; get; }

  
    public Template(int id, int? formId, LiveVersion live, string formName = "")
    {
        Id = id;
        FormId = formId;
        LiveVersion = live;
        Save(formName);
    }

    private void Save(string formName = "")
    {
        string contents = JsonSerializer.Serialize(this);

        //fixes formatting issues
        contents = contents.Replace("\"\"\"[{", "\"[{").Replace("}]\"\"\"", "}]\"").Replace("\"\"[{", "\"[{")
            .Replace("}]\"\"", "}]\"").Replace("newline", "\\n").Replace(@"\\\u0022", "\\\"").Replace(@"\u0022", "\\\"")
            .Replace(@"\u003C", "<").Replace(@"\u003E", ">").Replace(@"\u0027", "'").Replace(@"\u0026", "&")
            .Replace("\"\\\"", "\"").Replace("\\\"\"", "\"")
            .Replace("</a>", @"<\/a>").Replace("</p>", @"<\/p>")
            .Replace(",\\\"data\\\":\\\"", ",\\\"data\\\":\\\"\\\"");
        string templateTitle = LiveVersion.Metadata.Title;
        templateTitle = templateTitle.Replace(" ", "_");

        SaveFiles.SaveString(@"\page-templates\" + Id + "-" + templateTitle, contents, formName);
   }
}


public class LiveVersion(Metadata sectionMetadata, PageSection[] pageSections)
{
    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; private set; } = sectionMetadata;
    [JsonPropertyName("pageSections")]
    public PageSection[] PageSections {get; private set;} = pageSections;
}