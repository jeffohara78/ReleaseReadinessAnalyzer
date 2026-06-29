/* Jeff O'Hara
 * 6/29/22026
 * 
 * Helps software teams evaluate whether a software release is ready for deployment by tracking features, defects, 
 * test coverage, and known risks. Using JSON persistence and a release decision engine, the application generates 
 * a professional readiness report with actionable recommendations to help teams confidently determine whether 
 * to ship, review, or delay a release.
 */


using ReleaseReadinessAnalyzer.Models;
using ReleaseReadinessAnalyzer.Services;

ReleaseManager manager = new ReleaseManager();

bool running = true;

while (running)
{
    Console.Clear();

    PrintHeader("Release Readiness Analyzer");

    Console.WriteLine("This app helps a team decide if a software release is safe to ship.");
    Console.WriteLine("You will enter features, defects, test coverage, and risks.");
    Console.WriteLine("The app will then generate a release recommendation.");
    Console.WriteLine();

    Console.WriteLine("Main Menu");
    Console.WriteLine("-----------------------------------");
    Console.WriteLine("1. Add a Feature");
    Console.WriteLine("2. Add a Defect");
    Console.WriteLine("3. View Features");
    Console.WriteLine("4. View Defects");
    Console.WriteLine("5. View Release Readiness Report");
    Console.WriteLine("6. Exit");
    Console.WriteLine();

    Console.Write("Choose an option: ");
    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddFeature(manager);
            break;
        case "2":
            AddDefect(manager);
            break;
        case "3":
            ViewFeatures(manager);
            break;
        case "4":
            ViewDefects(manager);
            break;
        case "5":
            ViewReleaseReport(manager);
            break;
        case "6":
            running = false;
            break;
        default:
            Console.WriteLine("That was not a valid menu option.");
            Pause();
            break;
    }
}

static void AddFeature(ReleaseManager manager)
{
    Console.Clear();
    PrintHeader("Add a Feature");

    Console.WriteLine("A feature is a piece of functionality being included in the release.");
    Console.WriteLine("Examples:");
    Console.WriteLine("- User login screen");
    Console.WriteLine("- Password reset email");
    Console.WriteLine("- Admin dashboard");
    Console.WriteLine("- Export report to PDF");
    Console.WriteLine();
    Console.WriteLine("Enter 0 at any text prompt to return to the main menu.");
    Console.WriteLine();

    string? name = AskText(
        "Feature name",
        "Example: Password reset email");

    if (name == "0") return;

    string? owner = AskText(
        "Feature owner",
        "Example: Sarah, Backend Team, QA Team");

    if (owner == "0") return;

    bool codeComplete = AskYesNo(
        "Is the code complete?",
        "Choose yes only if developers believe the feature is fully built.");

    int coverage = AskNumber(
        "Test coverage percent",
        "Enter a number from 0 to 100. Example: 85 means most of the feature has been tested.",
        0,
        100);

    bool hasRisk = AskYesNo(
        "Does this feature have a known release risk?",
        "Choose yes if there are concerns such as unstable code, unclear requirements, or missing testing.");

    Feature feature = new Feature
    {
        Name = name,
        Owner = owner ?? "Unknown",
        CodeComplete = codeComplete,
        TestCoveragePercent = coverage,
        HasKnownRisk = hasRisk
    };

    manager.AddFeature(feature);

    Console.WriteLine();
    Console.WriteLine("Feature saved successfully.");
    Pause();
}

static void AddDefect(ReleaseManager manager)
{
    Console.Clear();
    PrintHeader("Add a Defect");

    Console.WriteLine("A defect is a bug, issue, or problem found before release.");
    Console.WriteLine("Examples:");
    Console.WriteLine("- Login button does not work");
    Console.WriteLine("- App crashes when saving a report");
    Console.WriteLine("- User receives the wrong email confirmation");
    Console.WriteLine();
    Console.WriteLine("Enter 0 at any text prompt to return to the main menu.");
    Console.WriteLine();

    string? title = AskText(
        "Defect title",
        "Example: App crashes when user clicks Save");

    if (title == "0") return;

    string severity = AskSeverity();

    bool resolved = AskYesNo(
        "Is this defect resolved?",
        "Choose yes only if the fix has been completed and verified.");

    Defect defect = new Defect
    {
        Title = title,
        Severity = severity,
        IsResolved = resolved
    };

    manager.AddDefect(defect);

    Console.WriteLine();
    Console.WriteLine("Defect saved successfully.");
    Pause();
}

static string AskSeverity()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("Severity tells the team how serious the defect is.");
        Console.WriteLine();
        Console.WriteLine("1. Low      - Cosmetic issue or small inconvenience");
        Console.WriteLine("2. Medium   - Problem affects use but has a workaround");
        Console.WriteLine("3. High     - Major feature is broken");
        Console.WriteLine("4. Critical - App crashes, data loss, security issue, or release blocker");
        Console.WriteLine();

        Console.Write("Choose severity 1-4, or enter 0 to cancel: ");
        string? input = Console.ReadLine();

        switch (input)
        {
            case "0":
                return "Medium";
            case "1":
                return "Low";
            case "2":
                return "Medium";
            case "3":
                return "High";
            case "4":
                return "Critical";
            default:
                Console.WriteLine("Please choose a valid severity option.");
                break;
        }
    }
}

static void ViewFeatures(ReleaseManager manager)
{
    Console.Clear();
    PrintHeader("Feature List");

    if (manager.Features.Count == 0)
    {
        Console.WriteLine("No features have been added yet.");
    }
    else
    {
        foreach (Feature feature in manager.Features)
        {
            Console.WriteLine($"Feature #{feature.Id}: {feature.Name}");
            Console.WriteLine($"Owner: {feature.Owner}");
            Console.WriteLine($"Code Complete: {FormatYesNo(feature.CodeComplete)}");
            Console.WriteLine($"Test Coverage: {feature.TestCoveragePercent}%");
            Console.WriteLine($"Known Risk: {FormatYesNo(feature.HasKnownRisk)}");

            if (!feature.CodeComplete)
            {
                Console.WriteLine("Concern: This feature is not code complete.");
            }

            if (feature.TestCoveragePercent < 70)
            {
                Console.WriteLine("Concern: Test coverage is below the recommended 70% minimum.");
            }

            if (feature.HasKnownRisk)
            {
                Console.WriteLine("Concern: This feature has a known release risk.");
            }

            Console.WriteLine("-----------------------------------");
        }
    }

    Pause();
}

static void ViewDefects(ReleaseManager manager)
{
    Console.Clear();
    PrintHeader("Defect List");

    if (manager.Defects.Count == 0)
    {
        Console.WriteLine("No defects have been added yet.");
    }
    else
    {
        foreach (Defect defect in manager.Defects)
        {
            Console.WriteLine($"Defect #{defect.Id}: {defect.Title}");
            Console.WriteLine($"Severity: {defect.Severity}");
            Console.WriteLine($"Resolved: {FormatYesNo(defect.IsResolved)}");

            if (!defect.IsResolved && defect.Severity.Equals("Critical", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Concern: This is an unresolved critical defect.");
            }

            Console.WriteLine("-----------------------------------");
        }
    }

    Pause();
}

static void ViewReleaseReport(ReleaseManager manager)
{
    Console.Clear();
    PrintHeader("Release Readiness Report");

    ReleaseStatus status = manager.CalculateReleaseStatus();

    int totalFeatures = manager.Features.Count;
    int totalDefects = manager.Defects.Count;
    int openDefects = manager.GetOpenDefectCount();
    int averageCoverage = manager.GetAverageTestCoverage();

    int incompleteFeatures = manager.Features.Count(f => !f.CodeComplete);
    int riskyFeatures = manager.Features.Count(f => f.HasKnownRisk);
    int lowCoverageFeatures = manager.Features.Count(f => f.TestCoveragePercent < 70);
    int criticalOpenDefects = manager.Defects.Count(d =>
        d.Severity.Equals("Critical", StringComparison.OrdinalIgnoreCase) && !d.IsResolved);

    Console.WriteLine($"Total Features: {totalFeatures}");
    Console.WriteLine($"Total Defects: {totalDefects}");
    Console.WriteLine($"Open Defects: {openDefects}");
    Console.WriteLine($"Average Test Coverage: {averageCoverage}%");
    Console.WriteLine();

    Console.WriteLine("Risk Summary");
    Console.WriteLine("-----------------------------------");
    Console.WriteLine($"Incomplete Features: {incompleteFeatures}");
    Console.WriteLine($"Features With Known Risk: {riskyFeatures}");
    Console.WriteLine($"Features Below 70% Coverage: {lowCoverageFeatures}");
    Console.WriteLine($"Open Critical Defects: {criticalOpenDefects}");
    Console.WriteLine();

    Console.WriteLine("Release Decision");
    Console.WriteLine("-----------------------------------");
    Console.WriteLine(status);
    Console.WriteLine();

    if (status == ReleaseStatus.Ready)
    {
        Console.WriteLine("Recommendation:");
        Console.WriteLine("The release appears safe to ship.");
    }
    else if (status == ReleaseStatus.NeedsReview)
    {
        Console.WriteLine("Recommendation:");
        Console.WriteLine("The release may be possible, but the team should review risks first.");
        Console.WriteLine();
        Console.WriteLine("Suggested next steps:");
        Console.WriteLine("- Review features with known risks.");
        Console.WriteLine("- Improve test coverage below 70%.");
        Console.WriteLine("- Confirm QA sign-off before release.");
    }
    else
    {
        Console.WriteLine("Recommendation:");
        Console.WriteLine("Do not ship this release yet.");
        Console.WriteLine();
        Console.WriteLine("Required next steps:");
        Console.WriteLine("- Complete all unfinished features.");
        Console.WriteLine("- Resolve all open critical defects.");
        Console.WriteLine("- Re-run this report after updates are made.");
    }

    Pause();
}

static string? AskText(string label, string example)
{
    Console.WriteLine(label);
    Console.WriteLine(example);
    Console.Write("> ");

    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        return "Not Provided";
    }

    return input.Trim();
}

static bool AskYesNo(string question, string explanation)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine(question);
        Console.WriteLine(explanation);
        Console.Write("Enter y for yes or n for no: ");

        string? input = Console.ReadLine()?.Trim().ToLower();

        if (input == "y") return true;
        if (input == "n") return false;

        Console.WriteLine("Please enter y or n.");
    }
}

static int AskNumber(string label, string explanation, int min, int max)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine(label);
        Console.WriteLine(explanation);
        Console.Write("> ");

        string? input = Console.ReadLine();

        if (int.TryParse(input, out int number) && number >= min && number <= max)
        {
            return number;
        }

        Console.WriteLine($"Please enter a number between {min} and {max}.");
    }
}

static string FormatYesNo(bool value)
{
    return value ? "Yes" : "No";
}

static void PrintHeader(string title)
{
    Console.WriteLine("===================================");
    Console.WriteLine($"   {title}");
    Console.WriteLine("===================================");
    Console.WriteLine();
}

static void Pause()
{
    Console.WriteLine();
    Console.WriteLine("Press Enter to continue...");
    Console.ReadLine();
}