using System.Text.Json;
using System.Text.Json.Serialization;
using JaduFromJson.Utils;

namespace JaduFromJson.Actions;

public class Rule
{
    public Rule(string title, int position, string runtime, List<Action> actions, string formName)
    {
        Title = title;
        Position = position;
        Runtime = runtime;
        Actions = GetRuleActions(actions);
        string contents = JsonSerializer.Serialize(this);
        SaveFiles.SaveString(@"\actions\rules\" + 0 + "-" + Title.Replace(" ", "_"), contents, formName);
    }

    [JsonPropertyName("title")] 
    public string Title { get; set; }
    [JsonPropertyName("position")] 
    public int Position { get; set; }
    [JsonPropertyName("runtime")] 
    public string Runtime { get; set; }
    [JsonPropertyName("actions")] 
    public List<RuleAction> Actions { get; set; }
    [JsonPropertyName("enabled")] 
    public bool Enabled { get; set; } = true;
    [JsonPropertyName("clauseGroups")] 
    public List<string> ClauseGroups { get; set; } = [];
    
    private static List<RuleAction> GetRuleActions(List<Action> actions)
    {
        int i = 1001;
        List<RuleAction> ruleActions = [];
        foreach (Action action in actions)
        {
            ruleActions.Add(new RuleAction(action, i));
            i++;
        }
        return ruleActions;
    }
}

public class RuleAction(Action action, int id)
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = id;
    [JsonPropertyName("actionType")]
    public string ActionType { get; set; } = action.ActionType;
    [JsonPropertyName("inputs")]
    public List<string> Inputs { get; set; } = [];
    [JsonPropertyName("actionNotifications")]
    public List<string> ActionNotifications { get; set; } = [];
    [JsonPropertyName("template")]
    public int Template { get; set; } = action.Id;
}