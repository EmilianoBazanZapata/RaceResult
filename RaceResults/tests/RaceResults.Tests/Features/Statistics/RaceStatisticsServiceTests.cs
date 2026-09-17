using RaceResults.Features.Statistics;

namespace RaceResults.Tests.Features.Statistics;

public sealed class RaceStatisticsServiceTests
{
    private readonly RaceStatisticsService _service = new();

    [Fact]
    public void GetAverageTotalTime_ReturnsArithmeticMean()
    {
        var results = new[]
        {
            TestData.CreateResult(1, totalTime: 300),
            TestData.CreateResult(2, totalTime: 360),
            TestData.CreateResult(3, totalTime: 420)
        };

        var average = _service.GetAverageTotalTime(results);

        Assert.Equal(360, average);
    }

    [Fact]
    public void GetAverageTotalTime_ThrowsForEmptyCollection()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => _service.GetAverageTotalTime([]));

        Assert.Contains("At least one", exception.Message);
    }

    [Theory]
    [MemberData(nameof(InvalidTimes))]
    public void GetAverageTotalTime_ThrowsForInvalidTotalTime(float invalidTime)
    {
        var results = TestData.CreateValidResults();
        results[0].TotalTime = invalidTime;

        Assert.Throws<ArgumentException>(
            () => _service.GetAverageTotalTime(results));
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
