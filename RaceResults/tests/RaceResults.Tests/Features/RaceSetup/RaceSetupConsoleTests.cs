using RaceResults.Features.RaceSetup;

namespace RaceResults.Tests.Features.RaceSetup;

public sealed class RaceSetupConsoleTests
{
    [Fact]
    public void Setup_RePromptsInvalidRacerCountAndSourceOption()
    {
        const string inputText = """
            nope
            2
            3
            9
            1
            """;

        using var input = new StringReader(inputText);
        using var output = new StringWriter();
        var setup = new RaceSetupConsole(input, output);

        var racerCount = setup.ReadRacerCount();
        var sourceOption = setup.ReadSourceOption();

        Assert.Equal(3, racerCount);
        Assert.Equal(1, sourceOption);
        Assert.Contains("Invalid number.", output.ToString());
        Assert.Contains("Invalid option.", output.ToString());
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("2", 2)]
    public void ReadContinuationOption_AcceptsValidOption(string inputText, int expectedOption)
    {
        using var input = new StringReader(inputText);
        using var output = new StringWriter();
        var setup = new RaceSetupConsole(input, output);

        var option = setup.ReadContinuationOption();

        Assert.Equal(expectedOption, option);
    }

    [Fact]
    public void ReadContinuationOption_RePromptsInvalidAndNonNumericOptions()
    {
        const string inputText = """

            0
            3
            abc
            2
            """;

        using var input = new StringReader(inputText);
        using var output = new StringWriter();
        var setup = new RaceSetupConsole(input, output);

        var option = setup.ReadContinuationOption();

        Assert.Equal(2, option);
        Assert.Equal(
            4,
            output.ToString().Split("Invalid option. Enter 1 or 2.").Length - 1);
    }
}
