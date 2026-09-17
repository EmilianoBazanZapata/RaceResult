using System.Globalization;

namespace RaceResults.Features.Results;

public static class TimeFormatter
{
    public static string Format(float totalSeconds)
    {
        if (!float.IsFinite(totalSeconds) || totalSeconds < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalSeconds),
                "Time must be a finite number greater than or equal to zero.");
        }

        var roundedSeconds = Math.Round(totalSeconds, MidpointRounding.AwayFromZero);

        if (roundedSeconds > long.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalSeconds),
                "Time is too large to format.");
        }

        var wholeSeconds = checked((long)roundedSeconds);
        var minutes = wholeSeconds / 60;
        var seconds = wholeSeconds % 60;

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{minutes:D2}:{seconds:D2}");
    }
}
