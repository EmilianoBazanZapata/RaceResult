using RaceResults.Features.Results;

namespace RaceResults.Tests.Features.Results;

public sealed class RaceResultsConsolePresenterTests
{
    [Fact]
    public void Presenter_RendersRequiredTemplatesWithEveryTokenReplaced()
    {
        using var output = new StringWriter();
        var presenter = new RaceResultsConsolePresenter(output);
        var results = TestData.CreateValidResults();

        presenter.ShowResults(results);
        presenter.ShowFastestCar(results[2]);

        var renderedOutput = output.ToString();

        Assert.Contains("1 | 27 - Emiliano Bazan | 05:16", renderedOutput);
        Assert.Contains("The fastest car is 44 with a time of 01:08", renderedOutput);
        Assert.DoesNotContain("{pos}", renderedOutput);
        Assert.DoesNotContain("{car_numb}", renderedOutput);
        Assert.DoesNotContain("{racer_name}", renderedOutput);
        Assert.DoesNotContain("{total_time}", renderedOutput);
        Assert.DoesNotContain("{fastest_time}", renderedOutput);
    }
}
