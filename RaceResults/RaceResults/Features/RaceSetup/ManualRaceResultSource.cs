using System.Globalization;
using RaceResults.Domain;

namespace RaceResults.Features.RaceSetup;

public sealed class ManualRaceResultSource : IRaceResultSource
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ManualRaceResultSource(TextReader input, TextWriter output)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public List<RaceResult> CreateResults(int racerCount)
    {
        RaceSetupRules.ValidateRacerCount(racerCount);

        var results = new List<RaceResult>(racerCount);
        var usedPositions = new HashSet<int>();
        var usedCarNumbers = new HashSet<int>();

        for (var racerIndex = 0; racerIndex < racerCount; racerIndex++)
        {
            _output.WriteLine($"Racer {racerIndex + 1} of {racerCount}");
            _output.WriteLine();

            var position = ReadUniqueInteger(
                "Final position: ",
                1,
                racerCount,
                usedPositions,
                $"Invalid number. Enter an integer between 1 and {racerCount}.",
                "That position is already used.");

            var carNumber = ReadUniqueInteger(
                "Car number: ",
                RaceSetupRules.MinimumCarNumber,
                RaceSetupRules.MaximumCarNumber,
                usedCarNumbers,
                $"Invalid number. Enter an integer between {RaceSetupRules.MinimumCarNumber} " +
                $"and {RaceSetupRules.MaximumCarNumber}.",
                "That car number is already used.");

            var racerName = ReadRacerName();
            var totalTime = ReadPositiveTime("Total race time in seconds: ");
            var fastestLap = ReadFastestLap(totalTime);

            results.Add(new RaceResult
            {
                Position = position,
                CarNumber = carNumber,
                RacerName = racerName,
                TotalTime = totalTime,
                FastestLap = fastestLap
            });

            _output.WriteLine();
        }

        return results;
    }

    private int ReadUniqueInteger(
        string prompt,
        int minimum,
        int maximum,
        HashSet<int> usedValues,
        string invalidMessage,
        string duplicateMessage)
    {
        while (true)
        {
            _output.Write(prompt);
            var input = ReadLine();

            if (!int.TryParse(
                    input,
                    NumberStyles.Integer,
                    CultureInfo.CurrentCulture,
                    out var value) ||
                value < minimum ||
                value > maximum)
            {
                _output.WriteLine(invalidMessage);
                continue;
            }

            if (!usedValues.Add(value))
            {
                _output.WriteLine(duplicateMessage);
                continue;
            }

            return value;
        }
    }

    private string ReadRacerName()
    {
        while (true)
        {
            _output.Write("Racer name: ");
            var racerName = ReadLine().Trim();

            if (racerName.Length > 0)
            {
                return racerName;
            }

            _output.WriteLine("Racer name cannot be empty.");
        }
    }

    private float ReadPositiveTime(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            var input = ReadLine();

            if (TryParseFloat(input, out var value) &&
                float.IsFinite(value) &&
                value > 0)
            {
                return value;
            }

            _output.WriteLine("Time must be a finite number greater than zero.");
        }
    }

    private float ReadFastestLap(float totalTime)
    {
        while (true)
        {
            var fastestLap = ReadPositiveTime("Fastest lap in seconds: ");

            if (fastestLap <= totalTime)
            {
                return fastestLap;
            }

            _output.WriteLine("Fastest lap cannot be greater than total race time.");
        }
    }

    private static bool TryParseFloat(string input, out float value)
    {
        if (float.TryParse(
                input,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out value))
        {
            return true;
        }

        return float.TryParse(
            input,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out value);
    }

    private string ReadLine() =>
        _input.ReadLine() ??
        throw new EndOfStreamException("Console input ended before manual entry was complete.");
}
