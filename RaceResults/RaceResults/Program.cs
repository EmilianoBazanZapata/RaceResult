using RaceResults.Features.RaceSetup;
using RaceResults.Features.Results;
using RaceResults.Features.Statistics;

var setupConsole = new RaceSetupConsole(Console.In, Console.Out);
var resultService = new RaceResultService();
var statisticsService = new RaceStatisticsService();
var presenter = new RaceResultsConsolePresenter(Console.Out);

try
{
    while (true)
    {
        Console.Clear();
        setupConsole.ShowWelcome();

        var racerCount = setupConsole.ReadRacerCount();
        var sourceOption = setupConsole.ReadSourceOption();

        IRaceResultSource resultSource = sourceOption switch
        {
            1 => new RandomRaceResultSource(new Random()),
            2 => new ManualRaceResultSource(Console.In, Console.Out),
            _ => throw new InvalidOperationException("Unsupported race result source.")
        };

        var results = resultSource.CreateResults(racerCount);
        var sortedResults = resultService.SortResults(results);
        var podium = resultService.GetPodium(results);
        var fastestCar = resultService.GetFastestCar(results);
        var averageTotalTime = statisticsService.GetAverageTotalTime(results);

        presenter.ShowPodium(podium);
        presenter.ShowResults(sortedResults);
        presenter.ShowFastestCar(fastestCar);
        presenter.ShowStatistics(averageTotalTime);

        Console.WriteLine("Race processing complete.");
        Console.WriteLine();

        if (setupConsole.ReadContinuationOption() == 2)
        {
            Console.WriteLine("Thanks for using Race Results.");
            break;
        }
    }
}
catch (EndOfStreamException exception)
{
    Console.WriteLine();
    Console.WriteLine(exception.Message);
    Console.WriteLine("The application will exit without processing an incomplete race.");
}
