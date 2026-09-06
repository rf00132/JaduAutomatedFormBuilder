# JaduFormBuilder

A .NET 8 console tool that generates Jadu XFP form-export ZIP packages from a structured JSON "form request" file. It builds the entire XFP export file tree programmatically — form metadata, pages, page-level branching, page templates (with typed questions and settings), submission/email actions, and SHA1 checksums — then packages everything into a ZIP that can be imported directly into the XFP forms platform.

I built this to solve a form-migration problem: a council needed to move 200+ forms from one low-code form platform to another. Rebuilding each form by hand in the target platform's editor would have been slow and error-prone, so instead each form is described once as a compact JSON request and this tool emits a ready-to-import XFP package. That turned days of manual clicking into a repeatable batch process.

## Stack

- .NET 8 console application (C#)
- No external dependencies — uses `System.Text.Json` for serialization and `System.IO.Compression` for zipping

## Build and run

```bash
dotnet build
dotnet run --project JaduFromJson.csproj
```

On startup the app creates two working folders next to the executable if they don't already exist:

- `FormRequestInput/` — drop your JSON form-request files here
- `FormRequestOutput/` — generated XFP ZIP packages are written here

Run the app and choose option `1` (Batch request Conversion). It reads every file in `FormRequestInput/`, converts each one, and writes a ZIP per form into `FormRequestOutput/`.

## Input and output

**Input** — one JSON file per form. Each request describes the form title, the service email(s) that should receive submissions, and the questions. Questions are supplied as a delimited string of records, each carrying section, section name, question text, mandatory flag, component type, options, and help text. Example:

```json
{
  "Title": "Example Service Request",
  "ServiceEmail": "example.service@example.gov.uk",
  "Questions": "[1,Your details,Full Name,Yes,Text field,,Enter your full name],[1,Your details,Email Address,Yes,Text field,,We will reply to this address]"
}
```

Supported question types map onto XFP components: Text field, Text area, Radio buttons, Checkboxes, Dropdown with list, Date field, Upload field, and Hidden. Text fields whose label contains "email" or "phone" automatically get the matching validation applied.

**Output** — one XFP-importable ZIP per form, containing the standard export layout:

```
form.json                       Form metadata
version.json                    Platform version stamp
checksums.json                  SHA1 hash of every other file
structure/pages.json            Page order
structure/branching.json        Page-to-page flow (Start -> pages -> Confirmation)
page-templates/{id}-{Title}.json  Page definitions with questions and settings
actions/templates/{id}-{Title}.json  Submission and email action templates
actions/rules/{id}-{Title}.json      Rule that fires the actions on submit
```

All ZIP entry paths use forward slashes (a hard requirement for XFP import — Windows-style backslash paths are rejected), and `checksums.json` holds the SHA1 hash of each generated file so the platform can verify export integrity.

## Project structure

```
src/
├── Program.cs          Console entry point and batch menu loop
├── Structure/          Models for the form's shape
│   ├── Form.cs         Top-level form.json (title, settings, metadata, categories)
│   ├── Pages.cs        Page list and page-order serialization
│   ├── PageSection.cs  Sections plus typed questions and question settings
│   ├── Template.cs     Page template (id + live version wrapping the sections)
│   ├── Metadata.cs     Shared page/section metadata
│   └── Version.cs      Platform version stamp for version.json
├── Actions/            Models for form behaviour on submit
│   ├── Action.cs       Action templates (submission confirmation, emails) and their field mappings/placeholders
│   ├── Rule.cs         The rule that chains the actions and fires on submit
│   └── Branching.cs    Page-to-page branching definitions
└── Utils/              The conversion engine and file handling
    ├── RequestToImport.cs  Parses a JSON request and orchestrates building every part of the export
    ├── SaveFiles.cs        Creates the directory tree, writes JSON files, and zips the package
    └── Checksum.cs         Computes SHA1 checksums for checksums.json
```

## Notes

- The generated packages use placeholder values (e.g. a `noreply@example.co.uk` "from" address and example category identifiers) where a real platform install would substitute its own — adjust these to match your target environment before import.
- The tool is deliberately dependency-free so it can run anywhere the .NET 8 runtime is available.
