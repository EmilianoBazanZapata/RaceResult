namespace RaceResults.Features.RaceSetup;

public static class RaceSetupRules
{
    public const int MinimumRacerCount = 3;
    public const int MaximumRacerCount = 20;
    public const int MinimumCarNumber = 1;
    public const int MaximumCarNumber = 999;

    public static void ValidateRacerCount(int racerCount)
    {
        if (racerCount < MinimumRacerCount || racerCount > MaximumRacerCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(racerCount),
                $"Racer count must be between {MinimumRacerCount} and {MaximumRacerCount}.");
        }
    }
}
