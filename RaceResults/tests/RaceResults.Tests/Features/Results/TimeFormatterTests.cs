using RaceResults.Features.Results;

namespace RaceResults.Tests.Features.Results;

public sealed class TimeFormatterTests
{
    [Theory]
    [InlineData(0, "00:00")]
    [InlineData(8, "00:08")]
    [InlineData(68, "01:08")]
    [InlineData(316, "05:16")]
    public void Format_ReturnsMinutesAndSeconds(float totalSeconds, string expected)
    {
        var formattedTime = TimeFormatter.Format(totalSeconds);

        Assert.Equal(expected, formattedTime);
    }

    [Fact]
    public void Format_RoundsToNearestWholeSecond()
    {
        var formattedTime = TimeFormatter.Format(68.5f);

        Assert.Equal("01:09", formattedTime);
    }

    [Theory]
    [MemberData(nameof(InvalidTimes))]
    public void Format_ThrowsForNegativeOrNonFiniteValue(float invalidTime)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TimeFormatter.Format(invalidTime));
    }

    public static TheoryData<float> InvalidTimes =>
    [
        -1,
        float.NaN,
        float.PositiveInfinity,
        float.NegativeInfinity
    ];
}
