namespace RaceResults.Domain;

public sealed class RaceResult
{
    public int Position { get; set; }

    public int CarNumber { get; set; }

    public string RacerName { get; set; } = string.Empty;

    public float TotalTime { get; set; }

    public float FastestLap { get; set; }
}
