using RaceResults.Domain;

namespace RaceResults.Features.Results;

public sealed class RaceResultService
{
    public List<RaceResult> SortResults(IList<RaceResult> results)
    {
        ValidateResults(results);

        var sortedResults = new List<RaceResult>(results);

        for (var index = 1; index < sortedResults.Count; index++)
        {
            var currentResult = sortedResults[index];
            var comparisonIndex = index - 1;

            while (comparisonIndex >= 0 &&
                   sortedResults[comparisonIndex].Position > currentResult.Position)
            {
                sortedResults[comparisonIndex + 1] = sortedResults[comparisonIndex];
                comparisonIndex--;
            }

            sortedResults[comparisonIndex + 1] = currentResult;
        }

        return sortedResults;
    }

    public RaceResult GetFastestCar(IList<RaceResult> results)
    {
        ValidateResults(results);

        if (results.Count == 0)
        {
            throw new ArgumentException(
                "At least one race result is required to determine the fastest car.",
                nameof(results));
        }

        var fastestCar = results[0];

        for (var index = 1; index < results.Count; index++)
        {
            var candidate = results[index];

            if (IsFaster(candidate, fastestCar))
            {
                fastestCar = candidate;
            }
        }

        return fastestCar;
    }

    public List<RaceResult> GetPodium(IList<RaceResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        if (results.Count < 3)
        {
            throw new ArgumentException(
                "At least three race results are required to build a podium.",
                nameof(results));
        }

        var sortedResults = SortResults(results);
        var podium = new List<RaceResult>(3);

        for (var index = 0; index < 3; index++)
        {
            podium.Add(sortedResults[index]);
        }

        return podium;
    }

    private static bool IsFaster(RaceResult candidate, RaceResult currentFastest)
    {
        if (candidate.FastestLap != currentFastest.FastestLap)
        {
            return candidate.FastestLap < currentFastest.FastestLap;
        }

        if (candidate.Position != currentFastest.Position)
        {
            return candidate.Position < currentFastest.Position;
        }

        return candidate.CarNumber < currentFastest.CarNumber;
    }

    private static void ValidateResults(IList<RaceResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var usedPositions = new HashSet<int>();
        var usedCarNumbers = new HashSet<int>();

        foreach (var result in results)
        {
            if (result is null)
            {
                throw new ArgumentException("Race results cannot contain null entries.", nameof(results));
            }

            if (result.Position < 1 || result.Position > results.Count)
            {
                throw new ArgumentException(
                    $"Position must be between 1 and {results.Count}.",
                    nameof(results));
            }

            if (!usedPositions.Add(result.Position))
            {
                throw new ArgumentException("Race result positions must be unique.", nameof(results));
            }

            if (result.CarNumber <= 0)
            {
                throw new ArgumentException("Car numbers must be greater than zero.", nameof(results));
            }

            if (!usedCarNumbers.Add(result.CarNumber))
            {
                throw new ArgumentException("Race result car numbers must be unique.", nameof(results));
            }

            if (string.IsNullOrWhiteSpace(result.RacerName))
            {
                throw new ArgumentException("Racer names cannot be empty.", nameof(results));
            }

            if (!IsFinitePositive(result.TotalTime))
            {
                throw new ArgumentException(
                    "Total race times must be finite numbers greater than zero.",
                    nameof(results));
            }

            if (!IsFinitePositive(result.FastestLap))
            {
                throw new ArgumentException(
                    "Fastest lap times must be finite numbers greater than zero.",
                    nameof(results));
            }

            if (result.FastestLap > result.TotalTime)
            {
                throw new ArgumentException(
                    "A fastest lap cannot be greater than the total race time.",
                    nameof(results));
            }
        }
    }

    private static bool IsFinitePositive(float value) =>
        float.IsFinite(value) && value > 0;
}
