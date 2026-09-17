using RaceResults.Features.RaceSetup;
using RaceResults.Features.Results;

namespace RaceResults.Tests.Features.RaceSetup;

public sealed class RandomRaceResultSourceTests
{
    [Fact]
    public void CreateResults_WithDeterministicSeed_ProducesValidCoherentRace()
    {
        const int racerCount = 6;
        var source = new RandomRaceResultSource(new Random(12345));

        var results = source.CreateResults(racerCount);

        Assert.Equal(racerCount, results.Count);
        Assert.Equal(racerCount, results.Select(result => result.CarNumber).Distinct().Count());
        Assert.Equal(racerCount, results.Select(result => result.Position).Distinct().Count());
        Assert.Equal(
            Enumerable.Range(1, racerCount),
            results.Select(result => result.Position).Order());
        Assert.All(results, result => Assert.True(result.TotalTime > 0));
        Assert.All(results, result => Assert.True(float.IsFinite(result.TotalTime)));
        Assert.All(results, result => Assert.True(result.FastestLap > 0));
        Assert.All(results, result => Assert.True(float.IsFinite(result.FastestLap)));
        Assert.All(results, result => Assert.True(result.FastestLap <= result.TotalTime));
        Assert.All(results, result => Assert.False(string.IsNullOrWhiteSpace(result.RacerName)));

        var rankedByTime = results.OrderBy(result => result.TotalTime).ToList();
        Assert.Equal(
            Enumerable.Range(1, racerCount),
            rankedByTime.Select(result => result.Position));

        Assert.False(
            results.Select(result => result.Position).SequenceEqual(Enumerable.Range(1, racerCount)));

        var service = new RaceResultService();
        var sortedResults = service.SortResults(results);
        Assert.Equal(Enumerable.Range(1, racerCount), sortedResults.Select(result => result.Position));
    }

    [Fact]
    public void CreateResults_ProducesFastestLapForRacerOtherThanWinner()
    {
        var source = new RandomRaceResultSource(new Random(12345));
        var service = new RaceResultService();

        var results = source.CreateResults(6);
        var fastestCar = service.GetFastestCar(results);

        Assert.NotEqual(1, fastestCar.Position);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(21)]
    public void CreateResults_ThrowsForRacerCountOutsideSupportedRange(int racerCount)
    {
        var source = new RandomRaceResultSource(new Random(12345));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => source.CreateResults(racerCount));
    }
}
