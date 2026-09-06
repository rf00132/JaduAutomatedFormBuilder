using System.Text.Json.Serialization;
using JaduFromJson.Utils;

namespace JaduFromJson.Structure;
public class Form
{
    public Form(string title, string instructions, FormMetadata metadata, Category[] categories, string formName = "")
    {
        Title = title;
        Instructions = instructions;
        Metadata = metadata;
        Categories = categories;
        Save(formName);
    }

    [JsonPropertyName("title")]
    public string Title {get; private set;}

    [JsonPropertyName("instructions")]
    public string Instructions {get; private set;}

    [JsonPropertyName("progressBar")]
    public bool ProgressBar {get; private set;} = false;

    [JsonPropertyName("allowUnregistered")]
    public bool AllowUnregistered {get; private set;} = true;

    [JsonPropertyName("resumeAtStart")]
    public bool ResumeAtStart {get; private set;} = false;

    [JsonPropertyName("reCaptcha")]
    public bool ReCaptcha {get; private set;} = false;

    [JsonPropertyName("internalOnly")]
    public bool InternalOnly {get; private set;} = false;

    [JsonPropertyName("embeddable")]
    public bool Embeddable {get; private set;} = false;

    [JsonPropertyName("metadata")]
    public FormMetadata Metadata {get; private set;}

    [JsonPropertyName("categories")]
    public Category[] Categories {get; private set;}

    [JsonPropertyName("contentPlaceholders")]
    public string[] ContentPlaceholders {get; private set;} = [];


    public void Save(string formName = "")
    {
        SaveFiles.SaveFormJson("form", this, formName);
    }
}

public class Category(string id, string categoryType, int itemId, string categoryId)
{
    [JsonPropertyName("id")]
    public string Id { get; private set; } = id;
    [JsonPropertyName("categoryType")]
    public string PageSections {get; private set;} = categoryType;
    [JsonPropertyName("itemID")]
    public int ItemId { get; private set; } = itemId;
    [JsonPropertyName("categoryID")]
    public string CategoryId { get; private set; } = categoryId;
}

public class FormMetadata(string id, string itemId, string description)
{
    [JsonPropertyName("id")]
    public string Id { get; private set; } = id;
    [JsonPropertyName("itemID")]
    public string ItemId { get; private set; } = itemId;
    [JsonPropertyName("creator")]
    public string Creator { get; private set; } = "Creator Name";
    [JsonPropertyName("contributor")]
    public string Contributor { get; private set; } = "";
    [JsonPropertyName("publisher")]
    public string Publisher { get; private set; } = "Publisher Name";
    [JsonPropertyName("rights")]
    public string Rights { get; private set; } = "Rights";
    [JsonPropertyName("source")]
    public string Source { get; private set; } = "";
    [JsonPropertyName("status")]
    public string Status { get; private set; } = "status";
    [JsonPropertyName("coverage")]
    public string Coverage { get; private set; } = "The World";
    [JsonPropertyName("created")]
    public long Created { get; private set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    [JsonPropertyName("modified")]
    public long Modified { get; private set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    [JsonPropertyName("valid")]
    public bool? Valid { get; private set; } = null;
    [JsonPropertyName("expired")]
    public bool? Expired { get; private set; } = null;
    [JsonPropertyName("format")]
    public string Format { get; private set; } = @"text/html";
    [JsonPropertyName("language")]
    public string Language { get; private set; } = "en";
    [JsonPropertyName("subject")]
    public string Subject { get; private set; } = "";
    [JsonPropertyName("description")]
    public string Description { get; private set; } = description;
}