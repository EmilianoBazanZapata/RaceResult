using RaceResults.Features.RaceSetup;

namespace RaceResults.Tests.Features.RaceSetup;

public sealed class ManualRaceResultSourceTests
{
    [Fact]
    public void CreateResults_RePromptsInvalidFieldsAndReturnsValidRace()
    {
        const string inputText = """
            abc
            1
            abc
            27
               
            Emiliano Bazan
            NaN
            -10
            0
            316
            Infinity
            450
            69
            1
            2
            27
            11
            Alex Turner
            320
            70
            3
            44
            Sofia Rossi
            327
            68
            """;

        using var input = new StringReader(inputText);
        using var output = new StringWriter();
        var source = new ManualRaceResultSource(input, output);

        var results = source.CreateResults(3);

        Assert.Equal(3, results.Count);
        Assert.Equal([1, 2, 3], results.Select(result => result.Position));
        Assert.Equal([27, 11, 44], results.Select(result => result.CarNumber));
        Assert.Equal("Emiliano Bazan", results[0].RacerName);
        Assert.Equal(316, results[0].TotalTime);
        Assert.Equal(69, results[0].FastestLap);

        var renderedOutput = output.ToString();
        Assert.Contains("Invalid number. Enter an integer between 1 and 3.", renderedOutput);
        Assert.Contains("Invalid number. Enter an integer between 1 and 999.", renderedOutput);
        Assert.Contains("That position is already used.", renderedOutput);
        Assert.Contains("That car number is already used.", renderedOutput);
        Assert.Contains("Racer name cannot be empty.", renderedOutput);
        Assert.Contains("Time must be a finite number greater than zero.", renderedOutput);
        Assert.Contains("Fastest lap cannot be greater than total race time.", renderedOutput);
    }

    [Fact]
    public void CreateResults_ThrowsForUnsupportedRacerCount()
    {
        using var input = new StringReader(string.Empty);
        using var output = new StringWriter();
        var source = new ManualRaceResultSource(input, output);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => source.CreateResults(2));
    }
}
