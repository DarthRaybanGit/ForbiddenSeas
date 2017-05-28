﻿using System.Collections;
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
    public const float LOW = 0.7f;
    public const float MID = 0.85f;
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

public enum PowerUP
{
    REGEN,
    SPEED_UP,
    DAMAGE_UP
};

public enum FixedDelayInGame
{
    TREASURE_FIRST_SPAWN = 2,
    TREASURE_RESPAWN = 10,
    POWERUP_SPAWN = 60,
    DAMAGEUP_TIME = 10,
    PLAYERS_DELAY = 5,
    COUNTDOWN_START = 5,
    YOHOHO_UPDATE_INTERVAL = 1,
    YOHOHO_FULLFY_SPAN = 120
}

public enum BuffTiming
{
    YOHOHO_DURATION = 5,
    REGEN_DURATION = 20,
    SPEED_UP_DURATION = 20,
    DAMAGE_UP_DURATION = 20,
}

public enum BuffValue
{
    RegenValue = 50,
    DmgUpValue = 20,
    SpeedUpValue = 20,
    YohohoSpeed = 100,
    YohohoRegen = 150
}

public enum DebuffTiming
{
    POISON_DURATION = 9,
    BLIND_DURATION = 6
}

//KEEP IT UPDATE
public enum SpawnIndex
{
    CLASS_VIEWER,
    ORIENTALS_FLAGSHIP,
    PIRATES_FLAGSHIP,
    VENETIANS_FLAGSHIP,
    VIKINGS_FLAGSHIP,
    TREASURE,
    PORTO,
    REGEN,
    SPEED_UP,
    DAMAGE_UP
}

public class Symbols
{
    public const int PLAYER_NOT_SET = -1;
    public const float mainAttackDelay = 0.7f;
    public const float specAttackDelay = 0.7f;
    public const int REGEN_AMOUNT = -50;
}

public class ReputationValues
{
    public const int KILLED = -500;
    public const int SUPPKILLED = -100;
    public const int COIN = 50;
    public const int KILL = 1000;
    public const int SUPPKILL = 500;
    public const int POWERUP = 200;
    public const int TREASURE = 500;
    public const int ARRH = 2000;
}