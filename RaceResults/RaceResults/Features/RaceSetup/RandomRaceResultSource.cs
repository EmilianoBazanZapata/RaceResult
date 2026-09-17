using RaceResults.Domain;

namespace RaceResults.Features.RaceSetup;

public sealed class RandomRaceResultSource : IRaceResultSource
{
    private static readonly string[] RacerNames =
    [
        "Emiliano Bazan",
        "Sofia Rossi",
        "Mateo Silva",
        "Lucia Romero",
        "Alex Turner",
        "Valentina Cruz",
        "Nicolas Vega",
        "Camila Torres",
        "Thiago Navarro",
        "Martina Ruiz",
        "Benjamin Castro",
        "Julieta Moreno",
        "Lautaro Herrera",
        "Delfina Acosta",
        "Tomas Medina",
        "Catalina Rojas",
        "Franco Molina",
        "Agustina Luna",
        "Joaquin Peralta",
        "Renata Cabrera"
    ];

    private readonly Random _random;

    public RandomRaceResultSource(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public List<RaceResult> CreateResults(int racerCount)
    {
        RaceSetupRules.ValidateRacerCount(racerCount);

        var usedCarNumbers = new HashSet<int>();
        var usedTotalTimes = new HashSet<int>();
        var racers = new List<RaceResult>(racerCount);

        for (var racerIndex = 0; racerIndex < racerCount; racerIndex++)
        {
            racers.Add(new RaceResult
            {
                CarNumber = NextUniqueNumber(
                    usedCarNumbers,
                    RaceSetupRules.MinimumCarNumber,
                    RaceSetupRules.MaximumCarNumber + 1),
                RacerName = RacerNames[racerIndex],
                TotalTime = NextUniqueNumber(usedTotalTimes, 300, 601),
                FastestLap = _random.Next(68, 91)
            });
        }

        var rankedRacers = racers
            .OrderBy(result => result.TotalTime)
            .ToList();

        for (var index = 0; index < rankedRacers.Count; index++)
        {
            rankedRacers[index].Position = index + 1;
        }

        // Keeping the fastest lap away from the winner demonstrates that the two rankings differ.
        var fastestLapRacerIndex = _random.Next(1, rankedRacers.Count);
        rankedRacers[fastestLapRacerIndex].FastestLap = _random.Next(60, 68);

        Shuffle(rankedRacers);

        if (rankedRacers.Select(result => result.Position).SequenceEqual(Enumerable.Range(1, racerCount)))
        {
            var firstRacer = rankedRacers[0];
            rankedRacers.RemoveAt(0);
            rankedRacers.Add(firstRacer);
        }

        return rankedRacers;
    }

    private int NextUniqueNumber(HashSet<int> usedValues, int minimum, int maximumExclusive)
    {
        int value;

        do
        {
            value = _random.Next(minimum, maximumExclusive);
        }
        while (!usedValues.Add(value));

        return value;
    }

    private void Shuffle(List<RaceResult> results)
    {
        for (var index = results.Count - 1; index > 0; index--)
        {
            var swapIndex = _random.Next(index + 1);
            (results[index], results[swapIndex]) = (results[swapIndex], results[index]);
        }
    }
}
