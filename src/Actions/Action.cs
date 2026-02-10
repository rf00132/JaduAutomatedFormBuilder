using System.Text.Json;
using System.Text.Json.Serialization;
using JaduFromJson.Utils;

namespace JaduFromJson.Actions;

public class Action
{
    [JsonPropertyName("id")]
    public int Id {private set; get;} 

    [JsonPropertyName("actionType")]
    public string ActionType {private set; get;}

    [JsonPropertyName("title")]
    public string Title {private set; get;} 
    
    [JsonPropertyName("mappings")]
    public List<Mapping>? Mappings {private set; get;} 
    
    public Action(int formId, string actionType, string title, List<Mapping>? mappings, string formName)
    {
        Id = formId;
        ActionType = actionType;
        Title = title;
        Mappings = mappings;

        string contents = JsonSerializer.Serialize(this);
        SaveFiles.SaveString(@"\actions\templates\" + Id + "-" + Title.Replace(" ", "_"), contents, formName);
        
    }
}

public class Mapping(string destination, Source source, string label, List<Placeholder> placeholders)
{
    [JsonPropertyName("destination")]
    public string Destination {private set; get;} = destination;
    [JsonPropertyName("source")]
    public Source SourceMapping {private set; get;} = source;
    [JsonPropertyName("label")]
    public string Label {private set; get;} = label;
    [JsonPropertyName("contentPlaceholders")]
    public List<Placeholder> Placeholders {private set; get;} = placeholders;
}

public class Source(int id, string value, string type)
{
    [JsonPropertyName("formID")]
    public int Id {private set; get;} = id;
    [JsonPropertyName("value")]
    public string Value {private set; get;} = value;
    [JsonPropertyName("type")]
    public string Type {private set; get;} = type;
}

public class Placeholder(string field, PlaceholderMapping? map, string? contentType)
{
    [JsonPropertyName("field")]
    public string Field { set; get;} = field;
    [JsonPropertyName("mapping")]
    public PlaceholderMapping? Map {set; get;} = map;
    [JsonPropertyName("contentType")]
    public string? ContentType { set; get;} = contentType;
}

public class PlaceholderMapping (int formId, string type, string guid = "", int? page = null, string questionName = "", string? secondaryField = "", string sectionName = "")
{
    [JsonPropertyName("formID")]
    public int Id {set; get;} = formId;
    [JsonPropertyName("guid")]
    public string Guid {set; get;} = guid;
    [JsonPropertyName("type")]
    public string Type {set; get;} = type;

    [JsonPropertyName("page")] 
    public int? Page { set; get; } = page;
    [JsonPropertyName("questionName")]
    public string QuestionName { set; get; } = questionName;
    [JsonPropertyName("secondaryField")]
    public string? SecondaryField { set; get; } = secondaryField;
    [JsonPropertyName("sectionName")]
    public string SectionName { set; get; } = sectionName;
}

public class ReferenceNumberPlaceholder : Placeholder
{
    public ReferenceNumberPlaceholder(string field, int formId, string? contentType = null, PlaceholderMapping? map = null) : base(field, map, contentType)
    {
        string guid =  "Jadu\\XFormsPro\\Bundle\\StandardFormComponentBundle\\VariableDefinition\\CXMCaseOrUserFormReferenceDefinition";
        string type = "formvariablemapping";
        Map = new PlaceholderMapping(formId, type, guid);
      
        ContentType = "ActionTemplateMapping";
    }
}

public class FormTitlePlaceholder : Placeholder
{
    public FormTitlePlaceholder(string field, int formId, string? contentType = null, PlaceholderMapping? map = null) : base(field, map, contentType)
    {
        string guid =  "Jadu\\XFormsPro\\Bundle\\StandardFormComponentBundle\\VariableDefinition\\FormTitleDefinition";
        string type = "formvariablemapping";
        Map = new PlaceholderMapping(formId, type, guid);
        
        ContentType = "ActionTemplateMapping";
    }
}

public class UserEmailAddressPlaceholder : Placeholder
{
    public UserEmailAddressPlaceholder(string field, int formId, string? contentType = null, PlaceholderMapping? map = null, int? page = null) : base(field, map, contentType)
    {
        string type = "formanswermapping";
        string questionName = "Email_Address";
        string? secondaryField = null;
        string sectionName = "main";
        Map = new PlaceholderMapping(formId,type, "", page, questionName, secondaryField, sectionName);
        ContentType = "ActionTemplateMapping";
    }
}

public class AllQuestionsAndAnswersLabelPlaceholder : Placeholder
{
    public AllQuestionsAndAnswersLabelPlaceholder(string field, int formId, PlaceholderMapping? map = null, string contentType = "") : base(field, map, contentType)
    {
        string type = "formvariablemapping";
        string guid = "Jadu\\XFormsPro\\Bundle\\StandardFormComponentBundle\\VariableDefinition\\AllQuestionAnswers\\FrontEndLabelsAndVisible";
        Map = new PlaceholderMapping(formId,type, guid);
        ContentType = "ActionTemplateMapping";
        Field = field;
    }
}

public class SubmittedDatePlaceholder : Placeholder
{
    public SubmittedDatePlaceholder(string field, int formId, PlaceholderMapping? map = null, string contentType = "") : base(field, map, contentType)
    {
        string type = "formvariablemapping";
        string guid = "Jadu\\XFormsPro\\Bundle\\StandardFormComponentBundle\\VariableDefinition\\UserFormSubmittedDateDefinition";
        Map = new PlaceholderMapping(formId, type, guid);
      
        ContentType = "ActionTemplateMapping";
        Field = field;
    }
}