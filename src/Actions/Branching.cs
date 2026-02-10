using System.Text.Json;
using System.Text.Json.Serialization;
using JaduFromJson.Utils;

namespace JaduFromJson.Actions;
public class Branching
{
    [JsonPropertyName("branches")]
    public Branch[] Branches { private set; get;} 

    public Branching(Branch[] branches, string formName = "")
    {
        Branches = branches;
        Save(formName);
    }
    string MakeBranchingJsonString()
    {
        string branchingJsonString = "[";
        
        foreach(Branch branch in Branches)
        {
            branchingJsonString += JsonSerializer.Serialize(branch);
            if (Branches.Length - 1 != Branches.ToList().IndexOf(branch))
            {
                branchingJsonString += ",";
            }
        }
        branchingJsonString += "]";
        return branchingJsonString;
    }

    void Save(string formName = "")
    {
        SaveFiles.SaveString("branching", MakeBranchingJsonString(), "structure", formName);
    }
}

public class Branch(string pageId, string goToPageId, int ruleNumber = 1, int conditionNumber = 1, string? clause = null)
{
    [JsonPropertyName("pageID")]
    public string PageId { private set; get;} = pageId;

    [JsonPropertyName("clause")]
    public string? Clause { private set; get;} = clause;

    [JsonPropertyName("ruleNumber")]
    public int RuleNumber { private set; get;} = ruleNumber;

    [JsonPropertyName("conditionNumber")]
    public int ConditionNumber { private set; get;} = conditionNumber;

    [JsonPropertyName("goToPageID")]
    public string GoToPageId { private set; get;} = goToPageId;
}
