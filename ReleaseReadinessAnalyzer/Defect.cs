namespace ReleaseReadinessAnalyzer.Models;

public class Defect
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Severity { get; set; } = "";
    public bool IsResolved { get; set; }
}