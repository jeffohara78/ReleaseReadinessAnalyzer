namespace ReleaseReadinessAnalyzer.Models;

public class Feature
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Owner { get; set; } = "";
    public bool CodeComplete { get; set; }
    public int TestCoveragePercent { get; set; }
    public bool HasKnownRisk { get; set; }
}