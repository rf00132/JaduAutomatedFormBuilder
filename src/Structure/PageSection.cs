using System.Text.Json.Serialization;

namespace JaduFromJson.Structure;
public class PageSection
{
    [JsonPropertyName("showTitle")]
    public string? ShowTitle { private set; get;}
    [JsonPropertyName("clause")] 
    public string? Clause { private set; get; }
    [JsonPropertyName("isRepeatableQuestion")]
    public string? IsRepeatableQuestion { private set; get;}
    [JsonPropertyName("repeatableQuestionSettings")]
    public string[] RepeatableQuestionSettings { private set; get;} = [];
    [JsonPropertyName("name")]
    public string Name { private set; get;}
    [JsonPropertyName("title")]
    public string Title { private set; get;}
    [JsonPropertyName("questions")]
    public TemplateQuestion[] Questions { private set; get;}
    [JsonPropertyName("structure")]
    public string Structure { private set; get;}
    [JsonPropertyName("discr")]
    public string Discr { private set; get;} = "section";

    public PageSection(string templateTitle, string templateName, string? isRepeatable, string? showTitle, TemplateQuestion[] questions, string? clause = null)
    {
        Title = templateTitle;
        Name = templateName;
        IsRepeatableQuestion = isRepeatable;
        ShowTitle = showTitle;
        Questions = questions;
        Clause = clause;

        Structure = BuildStructureString();
    }

    public string BuildStructureString()
    {
        List<string> structureComponents = new();
        for(int i = 0; i < Questions.Length; i++) 
        {
            structureComponents.Add($"{{\\\"type\\\":\\\"{Questions[i].Discr}\\\",\\\"name\\\":\\\"{Questions[i].Name}\\\"}}");
        }
        string ret = '[' + string.Join(',', structureComponents) + ']';
        return ret;
    }

}

public class TemplateQuestion 
{
    [JsonPropertyName("componentGUID")]
    public string ComponentGuid { private set; get;}
    [JsonPropertyName("settings")]
    public QuestionSetting[] Settings { private set; get;}
    [JsonPropertyName("name")]
    public string Name { private set; get;}
    [JsonPropertyName("title")]
    public string Title { private set; get;}
    [JsonPropertyName("discr")]
    public string Discr { private set; get;} = "question";

    public TemplateQuestion(string name, string title, string type, QuestionSetting[] settings)
    {
        Title = title;
        Settings = settings;
        ComponentGuid = SetType(type);
        Name = name;
        
    }
    
    string SetType(string type)
    {
        string typeString = "Jadu\\XFormsPro\\Bundle\\StandardFormComponentBundle\\Component\\";
        switch (type) 
        {
            case "TextField":
                return typeString + "TextField\\TextFieldComponent";
            case "TextArea":
                return typeString + "TextArea\\TextAreaComponent";
            case "TextBlock":
                return typeString + "TextBlock\\TextBlockComponent";
            case "RadioButtons":
                return typeString + "RadioButtons\\RadioButtonsComponent";
            case "Dropdown":
                return typeString + "Dropdown\\DropdownComponent";
            case "Checkboxes":
                return typeString + "Checkboxes\\CheckboxesComponent";
            case "Date":
                return typeString + "Date\\DateComponent";
            case "Address":
                return typeString + "IntegratedComponent\\IntegratedComponent";
            case "Upload":
                return typeString + "FileUpload\\FileUploadComponent";
            case "Hidden":
                return typeString + "HiddenField\\HiddenFieldComponent";
            default: 
                Console.WriteLine("Unknown question type: " + type);
                return "Unknown";
        }
    }
}

public class QuestionSetting(string name, string value)
{
    [JsonPropertyName("name")]
    public string Name {private set; get;} = name;

    [JsonPropertyName("value")]
    public string Value {private set; get;} = value;
}