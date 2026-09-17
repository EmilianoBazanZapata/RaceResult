using System.Globalization;

namespace RaceResults.Features.RaceSetup;

public sealed class RaceSetupConsole
{
    private const string Separator = "========================================";

    private readonly TextReader _input;
    private readonly TextWriter _output;

    public RaceSetupConsole(TextReader input, TextWriter output)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void ShowWelcome()
    {
        _output.WriteLine(Separator);
        _output.WriteLine("RACE RESULTS".PadLeft(26));
        _output.WriteLine(Separator);
        _output.WriteLine();
    }

    public int ReadRacerCount()
    {
        while (true)
        {
            _output.Write(
                $"Number of racers ({RaceSetupRules.MinimumRacerCount}-" +
                $"{RaceSetupRules.MaximumRacerCount}): ");

            var value = ReadLine();

            if (int.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.CurrentCulture,
                    out var racerCount) &&
                racerCount >= RaceSetupRules.MinimumRacerCount &&
                racerCount <= RaceSetupRules.MaximumRacerCount)
            {
                _output.WriteLine();
                return racerCount;
            }

            _output.WriteLine(
                $"Invalid number. Enter an integer between " +
                $"{RaceSetupRules.MinimumRacerCount} and {RaceSetupRules.MaximumRacerCount}.");
        }
    }

    public int ReadSourceOption()
    {
        _output.WriteLine("Choose data source:");
        _output.WriteLine();
        _output.WriteLine("1 - Generate random race");
        _output.WriteLine("2 - Enter racers manually");
        _output.WriteLine();

        return ReadOption();
    }

    public int ReadContinuationOption()
    {
        _output.WriteLine("1 - Continue");
        _output.WriteLine("2 - Exit");
        _output.WriteLine();

        return ReadOption();
    }

    private int ReadOption()
    {
        while (true)
        {
            _output.Write("Option: ");
            var value = ReadLine();

            if (int.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.CurrentCulture,
                    out var option) &&
                option is 1 or 2)
            {
                _output.WriteLine();
                return option;
            }

            _output.WriteLine("Invalid option. Enter 1 or 2.");
        }
    }

    private string ReadLine() =>
        _input.ReadLine() ??
        throw new EndOfStreamException("Console input ended before race setup was complete.");
}
