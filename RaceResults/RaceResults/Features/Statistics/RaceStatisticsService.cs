using RaceResults.Domain;

namespace RaceResults.Features.Statistics;

public sealed class RaceStatisticsService
{
    public float GetAverageTotalTime(IEnumerable<RaceResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var resultList = results.ToList();

        if (resultList.Count == 0)
        {
            throw new ArgumentException(
                "At least one race result is required to calculate an average.",
                nameof(results));
        }

        if (resultList.Any(result =>
                result is null ||
                !float.IsFinite(result.TotalTime) ||
                result.TotalTime <= 0))
        {
            throw new ArgumentException(
                "Total race times must be finite numbers greater than zero.",
                nameof(results));
        }

        return resultList.Average(result => result.TotalTime);
    }
}
