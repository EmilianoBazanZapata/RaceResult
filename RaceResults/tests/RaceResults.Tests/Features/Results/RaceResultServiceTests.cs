using RaceResults.Domain;
using RaceResults.Features.Results;

namespace RaceResults.Tests.Features.Results;

public sealed class RaceResultServiceTests
{
    private readonly RaceResultService _service = new();

    [Fact]
    public void SortResults_OrdersUnorderedInputByPositionAscending()
    {
        var results = new[]
        {
            TestData.CreateResult(4),
            TestData.CreateResult(1),
            TestData.CreateResult(3),
            TestData.CreateResult(2)
        };

        var sortedResults = _service.SortResults(results);

        Assert.Equal([1, 2, 3, 4], sortedResults.Select(result => result.Position));
        Assert.Equal([4, 1, 3, 2], results.Select(result => result.Position));
        Assert.NotSame(results, sortedResults);
    }

    [Fact]
    public void SortResults_AcceptsArrayInput()
    {
        IList<RaceResult> results = new[]
        {
            TestData.CreateResult(3),
            TestData.CreateResult(1),
            TestData.CreateResult(2)
        };

        var sortedResults = _service.SortResults(results);

        Assert.Equal([1, 2, 3], sortedResults.Select(result => result.Position));
    }

    [Fact]
    public void GetFastestCar_ReturnsLowestFastestLapWhenWinnerIsNotFastest()
    {
        var results = TestData.CreateValidResults();

        var fastestCar = _service.GetFastestCar(results);

        Assert.Equal(3, fastestCar.Position);
        Assert.Equal(44, fastestCar.CarNumber);
        Assert.Equal(68, fastestCar.FastestLap);
    }

    [Fact]
    public void GetFastestCar_AcceptsArrayInput()
    {
        IList<RaceResult> results = new[]
        {
            TestData.CreateResult(1, fastestLap: 69),
            TestData.CreateResult(2, fastestLap: 67),
            TestData.CreateResult(3, fastestLap: 68)
        };

        var fastestCar = _service.GetFastestCar(results);

        Assert.Equal(2, fastestCar.Position);
    }

    [Fact]
    public void GetFastestCar_UsesFinalPositionToBreakFastestLapTie()
    {
        var results = new[]
        {
            TestData.CreateResult(3, 44, fastestLap: 68),
            TestData.CreateResult(1, 27, fastestLap: 69),
            TestData.CreateResult(2, 11, fastestLap: 68)
        };

        var fastestCar = _service.GetFastestCar(results);

        Assert.Equal(2, fastestCar.Position);
        Assert.Equal(11, fastestCar.CarNumber);
    }

    [Fact]
    public void GetFastestCar_ThrowsForEmptyCollection()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => _service.GetFastestCar([]));

        Assert.Contains("At least one", exception.Message);
    }

    [Fact]
    public void GetPodium_ReturnsTopThreeByFinalPositionFromUnorderedInput()
    {
        var results = new[]
        {
            TestData.CreateResult(4),
            TestData.CreateResult(2),
            TestData.CreateResult(1),
            TestData.CreateResult(3)
        };

        var podium = _service.GetPodium(results);

        Assert.Equal([1, 2, 3], podium.Select(result => result.Position));
    }

    [Fact]
    public void GetPodium_AcceptsArrayInput()
    {
        IList<RaceResult> results = new[]
        {
            TestData.CreateResult(4),
            TestData.CreateResult(2),
            TestData.CreateResult(1),
            TestData.CreateResult(3)
        };

        var podium = _service.GetPodium(results);

        Assert.Equal([1, 2, 3], podium.Select(result => result.Position));
    }

    [Fact]
    public void GetPodium_ThrowsWhenFewerThanThreeRacersAreProvided()
    {
        var results = new[]
        {
            TestData.CreateResult(1),
            TestData.CreateResult(2)
        };

        var exception = Assert.Throws<ArgumentException>(
            () => _service.GetPodium(results));

        Assert.Contains("At least three", exception.Message);
    }

    [Fact]
    public void SortResults_ThrowsForDuplicatePositions()
    {
        var results = TestData.CreateValidResults();
        results[2].Position = 2;

        var exception = Assert.Throws<ArgumentException>(
            () => _service.SortResults(results));

        Assert.Contains("positions must be unique", exception.Message);
    }

    [Fact]
    public void SortResults_ThrowsForDuplicateCarNumbers()
    {
        var results = TestData.CreateValidResults();
        results[2].CarNumber = results[0].CarNumber;

        var exception = Assert.Throws<ArgumentException>(
            () => _service.SortResults(results));

        Assert.Contains("car numbers must be unique", exception.Message);
    }

    [Fact]
    public void SortResults_ThrowsForEmptyRacerName()
    {
        var results = TestData.CreateValidResults();
        results[1].RacerName = "   ";

        var exception = Assert.Throws<ArgumentException>(
            () => _service.SortResults(results));

        Assert.Contains("names cannot be empty", exception.Message);
    }

    [Theory]
    [MemberData(nameof(InvalidTimes))]
    public void SortResults_ThrowsForInvalidTotalTime(float invalidTime)
    {
        var results = TestData.CreateValidResults();
        results[0].TotalTime = invalidTime;

        var exception = Assert.Throws<ArgumentException>(
            () => _service.SortResults(results));

        Assert.Contains("Total race times", exception.Message);
    }

    [Theory]
    [MemberData(nameof(InvalidTimes))]
    public void SortResults_ThrowsForInvalidFastestLap(float invalidTime)
    {
        var results = TestData.CreateValidResults();
        results[0].FastestLap = invalidTime;

        var exception = Assert.Throws<ArgumentException>(
            () => _service.SortResults(results));

        Assert.Contains("Fastest lap times", exception.Message);
    }

    [Fact]
    public void SortResults_ThrowsWhenFastestLapExceedsTotalTime()
    {
        var results = TestData.CreateValidResults();
        results[0].FastestLap = results[0].TotalTime + 1;

        var exception = Assert.Throws<ArgumentException>(
            () => _service.SortResults(results));

        Assert.Contains("cannot be greater", exception.Message);
    }

    public static TheoryData<float> InvalidTimes =>
    [
        -1,
        0,
        float.NaN,
        float.PositiveInfinity,
        float.NegativeInfinity
    ];
}
