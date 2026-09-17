using RaceResults.Domain;

namespace RaceResults.Tests;

internal static class TestData
{
    public static RaceResult CreateResult(
        int position,
        int? carNumber = null,
        string? racerName = null,
        float? totalTime = null,
        float? fastestLap = null) =>
        new()
        {
            Position = position,
            CarNumber = carNumber ?? position * 10,
            RacerName = racerName ?? $"Racer {position}",
            TotalTime = totalTime ?? 300 + position,
            FastestLap = fastestLap ?? 60 + position
        };

    public static List<RaceResult> CreateValidResults() =>
    [
        CreateResult(1, 27, "Emiliano Bazan", 316, 69),
        CreateResult(2, 11, "Alex Turner", 320, 70),
        CreateResult(3, 44, "Sofia Rossi", 327, 68)
    ];
}
