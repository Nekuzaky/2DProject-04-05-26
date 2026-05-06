using System;

public static class GameRunSummary
{
    #region Properties
    public static bool HasData { get; private set; }
    public static int Kills { get; private set; }
    public static int Difficulty { get; private set; }
    public static TimeSpan Time { get; private set; }
    #endregion

    #region Public Methods
    public static void Save(int kills, int difficulty, TimeSpan time)
    {
        Kills = kills;
        Difficulty = difficulty;
        Time = time;
        HasData = true;
    }

    public static void Clear()
    {
        HasData = false;
        Kills = 0;
        Difficulty = 0;
        Time = TimeSpan.Zero;
    }
    #endregion
}
