using ReleaseReadinessAnalyzer.Models;

namespace ReleaseReadinessAnalyzer.Services;

public class ReleaseManager
{
    private readonly JsonStorageService<Feature> _featureStorage;
    private readonly JsonStorageService<Defect> _defectStorage;

    public List<Feature> Features { get; private set; }
    public List<Defect> Defects { get; private set; }

    public ReleaseManager()
    {
        _featureStorage = new JsonStorageService<Feature>("features.json");
        _defectStorage = new JsonStorageService<Defect>("defects.json");

        Features = _featureStorage.Load();
        Defects = _defectStorage.Load();
    }

    public void AddFeature(Feature feature)
    {
        feature.Id = Features.Count == 0 ? 1 : Features.Max(f => f.Id) + 1;
        Features.Add(feature);
        _featureStorage.Save(Features);
    }

    public void AddDefect(Defect defect)
    {
        defect.Id = Defects.Count == 0 ? 1 : Defects.Max(d => d.Id) + 1;
        Defects.Add(defect);
        _defectStorage.Save(Defects);
    }

    public ReleaseStatus CalculateReleaseStatus()
    {
        bool hasCriticalOpenDefect = Defects.Any(d =>
            d.Severity.Equals("Critical", StringComparison.OrdinalIgnoreCase)
            && !d.IsResolved);

        bool hasIncompleteFeature = Features.Any(f => !f.CodeComplete);

        bool hasLowCoverage = Features.Any(f => f.TestCoveragePercent < 70);

        bool hasKnownRisk = Features.Any(f => f.HasKnownRisk);

        if (hasCriticalOpenDefect || hasIncompleteFeature)
        {
            return ReleaseStatus.DoNotShip;
        }

        if (hasLowCoverage || hasKnownRisk)
        {
            return ReleaseStatus.NeedsReview;
        }

        return ReleaseStatus.Ready;
    }

    public int GetAverageTestCoverage()
    {
        if (Features.Count == 0)
        {
            return 0;
        }

        return (int)Features.Average(f => f.TestCoveragePercent);
    }

    public int GetOpenDefectCount()
    {
        return Defects.Count(d => !d.IsResolved);
    }
}