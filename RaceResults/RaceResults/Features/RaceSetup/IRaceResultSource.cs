using RaceResults.Domain;

namespace RaceResults.Features.RaceSetup;

public interface IRaceResultSource
{
    List<RaceResult> CreateResults(int racerCount);
}
