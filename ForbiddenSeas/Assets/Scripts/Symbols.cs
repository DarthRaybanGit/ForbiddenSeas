using System.Collections;
using System.Collections.Generic;

public class SpeedLevel
{
    public const float STOP = 0f;
    public const float SLOW = 0.25f;
    public const float HALF = 0.5f;
    public const float FULL = 1f;
}

public class ManeuverabilityLevel
{
    public const float LOW = 0.3f;
    public const float MID = 0.6f;
    public const float HIGH = 1f;
}

public class SpeedLevelRatio
{
    public const float SLOW = 1f;
    public const float HALF = 1.5f;
    public const float FULL = 2f;
}

public enum PlayerInfo
{
    ID,
    SPAWN_POSITION,
    IS_LOADED
};

public enum FixedDelayInGame
{
    TREASURE_FIRST_SPAWN = 2,
    TREASURE_RESPAWN = 10,
    POWERUP_SPAWN = 60,
    PLAYERS_DELAY = 5,
    COUNTDOWN_START = 5
}

//KEEP IT UPDATE
public enum SpawnIndex
{
    CLASS_VIEWER,
    ORIENTALS_FLAGSHIP,
    PIRATES_FLAGSHIP,
    VENETIANS_FLAGSHIP,
    VIKINGS_FLAGSHIP,
    TREASURE
}

public class Symbols
{
    public const int PLAYER_NOT_SET = -1;
    public const float mainAttackDelay = 0.5f;
    public const float specAttackDelay = 0.3f;
}