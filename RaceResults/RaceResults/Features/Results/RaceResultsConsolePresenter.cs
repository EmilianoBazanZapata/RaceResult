using System.Globalization;
using RaceResults.Domain;

namespace RaceResults.Features.Results;

public sealed class RaceResultsConsolePresenter
{
    private const string Separator = "========================================";

    private readonly TextWriter _output;

    public RaceResultsConsolePresenter(TextWriter output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void ShowPodium(IEnumerable<RaceResult> podium)
    {
        ArgumentNullException.ThrowIfNull(podium);

        ShowHeader("PODIUM");

        foreach (var result in podium)
        {
            _output.WriteLine(
                $"{GetOrdinal(result.Position)} | #{result.CarNumber} | " +
                $"{result.RacerName} | {TimeFormatter.Format(result.TotalTime)}");
        }

        _output.WriteLine();
    }

    public void ShowResults(IEnumerable<RaceResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        ShowHeader("RACE RESULTS");

        foreach (var result in results)
        {
            var line = RaceResultTemplates.RacerTable
                .Replace("{pos}", result.Position.ToString(CultureInfo.InvariantCulture))
                .Replace("{car_numb}", result.CarNumber.ToString(CultureInfo.InvariantCulture))
                .Replace("{racer_name}", result.RacerName)
                .Replace("{total_time}", TimeFormatter.Format(result.TotalTime));

            _output.WriteLine(line);
        }

        _output.WriteLine();
    }

    public void ShowFastestCar(RaceResult fastestCar)
    {
        ArgumentNullException.ThrowIfNull(fastestCar);

        ShowHeader("FASTEST LAP");

        var line = RaceResultTemplates.FastestRacer
            .Replace("{car_numb}", fastestCar.CarNumber.ToString(CultureInfo.InvariantCulture))
            .Replace("{fastest_time}", TimeFormatter.Format(fastestCar.FastestLap));

        _output.WriteLine(line);
        _output.WriteLine();
    }

    public void ShowStatistics(float averageTotalTime)
    {
        ShowHeader("STATISTICS");
        _output.WriteLine($"Average race time: {TimeFormatter.Format(averageTotalTime)}");
        _output.WriteLine();
    }

    private void ShowHeader(string title)
    {
        _output.WriteLine(Separator);
        _output.WriteLine(title.PadLeft((Separator.Length + title.Length) / 2));
        _output.WriteLine(Separator);
    }

    private static string GetOrdinal(int position) => position switch
    {
        1 => "1st",
        2 => "2nd",
        3 => "3rd",
        _ => position.ToString(CultureInfo.InvariantCulture)
    };
}
