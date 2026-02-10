using System.Text.Json;
using JaduFromJson.Actions;
using JaduFromJson.Structure;
using Action = JaduFromJson.Actions.Action;
using Version = JaduFromJson.Structure.Version;

namespace JaduFromJson.Utils;

public class RequestToImport
{
    private static int _templateIds = 2001;
    private static List<string> _templateQuestionNames = [];

    private static int _formId = 100;
    private static int _templateId = 1000;
    private static int _pageId = 500;
    public static string CurrentFormName { get; private set; } = "";
    public static string CurrentFormString { get; private set; } = "";
    public static List<string> UsedDataNames { get; private set; } = [];
    
    private static List<RequestQuestion> RequestQuestions { get; set; } = [];
    
    public static void BuildImport(string json)
    {
        RequestQuestions = [];
        UsedDataNames = [];
        CurrentFormString = json;
        json = json.Replace("\r", "").Replace("\n","");
        JsonElement formFile = JsonSerializer.Deserialize<JsonElement>(json);
        CurrentFormName = formFile.GetProperty("Title").ToString().Replace(" ", "_");
        var serviceEmail = GetServiceEmails(formFile.GetProperty("ServiceEmail").ToString());
        Console.WriteLine("Starting conversion of: " + CurrentFormName.Replace("_", " "));
        SaveFiles.MakeDirectories(CurrentFormName);
       
        Form form = BuildFormJson(formFile.GetProperty("Title").ToString(),  _formId, formFile.GetProperty("Title") + " form.");

        string? questionsString = formFile.GetProperty("Questions").GetString();
        RequestQuestions = RequestQuestion.ParseStringToRequestQuestions(questionsString);
        
        List<Template> templates = [];

        Dictionary<int, string> sections = [];
        foreach (var question in RequestQuestions)
        {
            if (!sections.ContainsKey(question.Section))
            {
                sections.Add(question.Section, question.SectionName);
            }
        }
        List<List<RequestQuestion>> questionsBySection = [];
        foreach (var section in sections)
        {
            questionsBySection.Add(RequestQuestions.Where(q => q.Section == section.Key).ToList());
        }

        foreach (List<RequestQuestion> list in questionsBySection)
        {
            _templateQuestionNames = [];
            List<TemplateQuestion> questions = [];
            foreach (RequestQuestion q in list)
            {
                List<QuestionSetting> questionSettings = [];
                 switch (q.Type)
                {
                    case "Radio buttons":
                        q.Type = "RadioButtons";
                        questionSettings.Add(new QuestionSetting("answers", BuildAnswerString(q.Options)));
                        break;
                    case "Checkboxes":
                        questionSettings.Add(new QuestionSetting("answers", BuildAnswerString(q.Options)));
                        break;
                    case "Dropdown with list":
                        q.Type = "Dropdown";
                        questionSettings.Add(new QuestionSetting("empty_option", "Please select..."));
                        questionSettings.Add(new QuestionSetting("answers", BuildAnswerString(q.Options)));
                        break;
                    case "Text field":
                        if (q.Text.ToLower().Contains("email"))
                        {
                            questionSettings.Add(new QuestionSetting("validation", "4"));
                        }
                        else if (q.Text.ToLower().Contains("phone"))
                        {
                            questionSettings.Add(new QuestionSetting("validation", "7"));
                        }
                        q.Type = "TextField";
                        break;
                    case "Text area":
                        q.Type = "TextArea";
                        questionSettings.Add(new QuestionSetting("rows", "4"));
                        questionSettings.Add(new QuestionSetting("cols", "40"));
                        break;
                    case "Date field":
                        q.Type = "Date";
                        break;
                    case "Upload field":
                        q.Type = "Upload";
                        break;
                    case "Hidden":
                        q.Type = "Hidden";
                        break;
                    default:
                        Console.WriteLine("Unknown component type detected: " + q.Type);
                        q.Type = "TextField";
                       break;
                }
                questionSettings.Add(new QuestionSetting("help", q.HelpText));
                if (q.Mandatory)
                {
                    questionSettings.Add(new QuestionSetting("required", "true"));
                }

                string name = q.Text;
                if (_templateQuestionNames.Contains(name))
                {
                    string newName;
                    int i = 1;
                    do
                    {
                        newName = name + i;
                        i++;
                    } while (_templateQuestionNames.Contains(name + i));

                    name = newName;
                }
                _templateQuestionNames.Add(name);
                questions.Add(new TemplateQuestion(q.ID, name,q.Type, questionSettings.ToArray()));
            }

            PageSection pageSection = new("main", "main", null, null, questions.ToArray());
            Metadata metadata = new(1, sections[list[0].Section], "", []);
            LiveVersion live = new(metadata, [pageSection]);
            Template template = new(_templateId, _formId, live, CurrentFormName);
            templates.Add(template);
            _templateId+=10;
        }
        
        
        
        Pages pages = BuildPages(templates.ToArray());
        Branching branching = BuildBranches(pages.JsonPages);
        Version version = BuildVersion();
        string checksum = Checksum.SaveString(CurrentFormName);
        string content =
            "<p>Thank you for submitting this {{1}}.</p>\r\n\r\n<p>Your reference number is:\u00a0{{0}}.</p>\r\n\r\n";
        Action submission = CreateSubmissionAction(content);
        Action emailToUser = CreateEmailUserAction(content);
        Action emailToService = CreateEmailServiceAction(content, serviceEmail);
        
        Rule rule = new("On Submit", 0, "on-submit", [submission, emailToService, emailToUser], CurrentFormName);
        
        SaveFiles.ZipFiles(CurrentFormName);
        Console.WriteLine("Finished conversion of: " + CurrentFormName.Replace("_", " "));
    }
    
    public static string BuildAnswerString(List<string> answers)
    {
        List<string> answersStrings = [];
        
        if(answers.ToString() == "") return "";
        foreach (string answer in answers)
        {
            string trimmedAnswer = answer.Trim();
            answersStrings.Add("{\\\"value\\\":\\\"" + trimmedAnswer.Replace(" ","_") + "\\\",\\\"text\\\":\\\"" + trimmedAnswer + "\\\",\\\"data\\\":\\\"\\\",\\\"internal\\\":false}");
        }
        string jsonAnswers = "\"[" + string.Join(',', answersStrings.ToArray()) + "]\"";
        return jsonAnswers;
    }
    
    static Form BuildFormJson(string formTitle, int formId, string formDescription)
    {
        Category[] categories = [new Category("4804", "LGNL", formId, "100004")];
        FormMetadata metadata = new("772", formId.ToString(), formDescription);
        Form form = new(formTitle, "", metadata, categories, CurrentFormName);
        return form;
    }

    static Pages BuildPages(Template[] templates)
    {
        List<Page> jsonPages = new();
        foreach (Template template in templates)
        {
            jsonPages.Add(BuildPage(template));
        }
        Pages pages = new(jsonPages.ToArray(), CurrentFormName);
        return pages;
    }
    
    static Page BuildPage(Template template)
    {
        Page page = new(_pageId, template.Id, template.LiveVersion.Metadata);
        _pageId++;
        return page;
    }
    
    static Branching BuildBranches(Page[] pages)
    {
        List<Branch> branches = [new("Start", pages[0].Id.ToString())];

        for (int i = 1; i < pages.Length; i++)
        {
            branches.Add(new Branch(pages[i - 1].Id.ToString(), pages[i].Id.ToString()));
        }
        branches.Add(new Branch(pages[^1].Id.ToString(), "Confirmation"));


        return new Branching(branches.ToArray(), CurrentFormName); 
    }
    
    static Version BuildVersion()
    {
        Version version = new(["1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1"]);
        version.Save(CurrentFormName);
        return version;
        
    }

    private static Action CreateSubmissionAction(string content)
    {
        return CreateAction("Submission", "Submission message", content);
    }   
    
    private static Action CreateEmailUserAction(string content)
    {
        List<Mapping> mappings = [CreateEmailToMapping(), CreateEmailFromMapping(), CreateEmailSubjectMapping(), CreateFormatMapping(),
        CreateContentMapping(""), CreateContentPlainMapping(), CreateAttachmentTypeMapping(), CreateAttachmentFormatMapping(), CreateAttachmentContentMapping(), CreateAttachmentPlainMapping()];
        
        
        return CreateAction("EmailUser", "Email to user", content, mappings);
    }
    
    private static Action CreateEmailServiceAction(string content, string serviceEmailAddress)
    {
        List<Mapping> mappings = [CreateEmailToMapping(serviceEmailAddress), CreateEmailFromMapping(), CreateEmailSubjectMapping(), CreateFormatMapping(),
            CreateContentMapping(""), CreateContentPlainMapping(), CreateAttachmentTypeMapping("File"), CreateAttachmentFormatMapping("PDF"), CreateAttachmentContentMapping(), CreateAttachmentPlainMapping()];
        
        
        return CreateAction("EmailService", "Email to service", content, mappings);
    }
    
    private static Action CreateAction(string type,string title, string content, List<Mapping>? mappings = null)
    {
        string typing = "formstaticmapping";
        string actionType = "";
        List<Mapping> defaultMappings = [];
        
        switch (type)
        {
            case "Submission":
                actionType = "Jadu\\XFormsPro\\Form\\Rules\\Actions\\CompletePage\\CompletePageActionType";
                Source source = new(_formId, content,typing);
                List<Placeholder> placeholders = [];
                placeholders.Add(new ReferenceNumberPlaceholder("0", _formId));
                placeholders.Add(new FormTitlePlaceholder("1", _formId));
                defaultMappings.Add(new("content", source, "Content", placeholders));
                break;
            case "EmailUser":
                actionType = "Jadu\\XFormsPro\\Form\\Rules\\Actions\\Email\\EmailActionType";
                Source sourceEmail = new(_formId, content,typing);
                List<Placeholder> placeholdersEmail = [];
                placeholdersEmail.Add(new ReferenceNumberPlaceholder("0", _formId));
                placeholdersEmail.Add(new FormTitlePlaceholder("1", _formId));
                placeholdersEmail.Add(new UserEmailAddressPlaceholder("2", _formId));
                defaultMappings.Add(new("content", sourceEmail, "Content", placeholdersEmail));
                break;
            case "EmailService":
                actionType = "Jadu\\XFormsPro\\Form\\Rules\\Actions\\Email\\EmailActionType";
                Source sourceService = new(_formId, content,typing);
                List<Placeholder> placeholdersService = [];
                placeholdersService.Add(new ReferenceNumberPlaceholder("0", _formId));
                placeholdersService.Add(new FormTitlePlaceholder("1", _formId));
                placeholdersService.Add(new AllQuestionsAndAnswersLabelPlaceholder("3", _formId, new PlaceholderMapping(_formId, "")));
                defaultMappings.Add(new("content", sourceService, "Content", placeholdersService));
                break;
            default:
                Console.WriteLine("Unknown template type: " + type);
                break;
        }
        
        if (mappings == null)
        {
            mappings = defaultMappings;
        }
        
        Action action = new(_templateIds,actionType, title, mappings, CurrentFormName);
        _templateIds++;
        return action;
    }

    private static Mapping CreateEmailToMapping(string email = "")
    {
        Mapping template;
        List<Placeholder> toPlaceholders = [];
        if (email.Equals(""))
        {
            email = "replace.me@email.co.uk";
        }
        template = new("template-to", new(_formId, email, "formstaticmapping"), "To", toPlaceholders);

        return template;
    }

    private static Mapping CreateEmailFromMapping()
    {
        return new("template-from", new(_formId, "noreply@email.co.uk", "formstaticmapping"), "From", []);
    }

    private static Mapping CreateEmailSubjectMapping()
    {
        Mapping template;
        List<Placeholder> subjectPlaceholders;
        string value = "{{1}}: {{0}}";
       
        subjectPlaceholders = [new FormTitlePlaceholder("1", _formId), new ReferenceNumberPlaceholder("0", _formId)];
        
        template = new("template-subject", new(_formId, value, "formstaticmapping"), "Subject", subjectPlaceholders);

        return template;
    }

    private static Mapping CreateFormatMapping()
    {
        return new Mapping("format", new Source(_formId, "HTML", "formstaticmapping"), "Format", []);
    }

    private static Mapping CreateContentMapping(string content)
    {
        content = "{{1}} submitted.\n\nReference: {{0}}\n\n" + content;
        List<Placeholder> placeholders = [new FormTitlePlaceholder("1", _formId), new ReferenceNumberPlaceholder("0", _formId)];
        
        return new Mapping("content", new Source(_formId, content, "formstaticmapping"), "Content", placeholders);
    }
    
    private static Mapping CreateContentPlainMapping()
    {
        return new Mapping("contentPlain", new Source(_formId, "", "formstaticmapping"), "Content", []);
    }
    
    private static Mapping CreateAttachmentTypeMapping(string type = "None")
    {
        return new Mapping("attachmentType", new Source(_formId, type, "formstaticmapping"), "Attachment", []);
    }
    
    private static Mapping CreateAttachmentFormatMapping(string type = "")
    {
        return new Mapping("attachmentFormat", new Source(_formId, type, "formstaticmapping"), "Attachment format", []);
    }
    
    private static Mapping CreateAttachmentContentMapping()
    {
        return new Mapping("attachmentContent", new Source(_formId, "<h1>{{1}}</h1><p>Reference: {{0}}</p><p>Date: {{5}}</p><p>{{4}}</p>", "formstaticmapping"), "Attachment content", [new AllQuestionsAndAnswersLabelPlaceholder("4", _formId), new FormTitlePlaceholder("1", _formId), new ReferenceNumberPlaceholder("0", _formId), new SubmittedDatePlaceholder("5", _formId)]);
    }
    
    private static Mapping CreateAttachmentPlainMapping()
    {
        return new Mapping("attachmentPlain", new Source(_formId, "", "formstaticmapping"), "Attachment content", []);
    }
    
    private static string GetServiceEmails(string serviceEmailField)
    {
        var splitSpace = serviceEmailField.Split(" ").ToList().Select((x) => x.Contains("@")? x : null).ToList();
        var splitComma = serviceEmailField.Split(",").ToList().Select((x) => x.Contains("@")? x : null).ToList();
        var splitNewline = serviceEmailField.Split("\n").ToList().Select((x) => x.Contains("@")? x : null).ToList();
        
        if (splitSpace.Count > splitComma.Count && splitSpace.Count > splitNewline.Count)
        {
            return string.Join(", ", splitSpace);
        }

        if (splitComma.Count > splitNewline.Count && splitComma.Count > splitSpace.Count)
        {
            return string.Join(", ", splitComma);
        }

        if (splitNewline.Count > splitSpace.Count && splitNewline.Count > splitComma.Count)
        {
            return string.Join(", ", splitNewline);
        }

        return string.Join(", ", splitComma);
    }
}


public class RequestQuestion
{
    public string ID { get; set; }
    public string Text { get; set; }
    public string Type { get; set; }
    public string HelpText { get; set; }
    public List<string> Options { get; set; }
    public int Section { get; set; }
    public string SectionName { get; set; }
    
    public bool Mandatory { get; set; }
    
    public RequestQuestion(string[] data)
    {
        ID = data[2].ToLower().Replace(" ","_");
        Text = data[2];
        Type = data[4];
        HelpText = data[^1];
        Section = int.Parse(data[0]);
        SectionName = data[1];
        Mandatory = data[3] == "Yes";
        List<string> commaSeparated = data[5].Replace(" -", ",").Split(",").ToList();
        List<string> newlineSeparated  = data[5].Split("\n").ToList();
        Options = commaSeparated.Count > newlineSeparated.Count ? commaSeparated : newlineSeparated;
    }
    
    public static List<RequestQuestion> ParseStringToRequestQuestions(string input)
    {
        var result = new List<RequestQuestion>();
        var records = input.Split(new[] { "],[" }, StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < records.Length; i++)
        {
            var record = records[i].Trim('[', ']');
            var fields = record.Split(',');
            result.Add(new RequestQuestion(fields));
        }
        
        return result;
    }
}