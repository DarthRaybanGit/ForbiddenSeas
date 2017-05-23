using System.Collections;
using System.Collections.Generic;

public class Pirates
{
    public const string shipName = "Pirate Galleon";
    public const int maxHealth = 1000;
    public const float maxSpeed = 120f;
    public const float maneuverability = ManeuverabilityLevel.LOW;
    public const int mainAttackDmg = 500;
    public const float mainAttackCD = 2.5f;
    public const int specAttackDmg = 500;
    public const float specAttackCD = 10f;
    public const float defense = 0.15f;

}

public class Egyptians
{
    public const string shipName = "War-type Gondola";
    public const int maxHealth = 800;
    public const float maxSpeed = 80f;
    public const float maneuverability = ManeuverabilityLevel.HIGH;
    public const int mainAttackDmg = 500;
    public const float mainAttackCD = 1.5f;
    public const int specAttackDmg = 500;
    public const float specAttackCD = 1.5f;
    public const float defense = 0.05f;

}

public class Vikings
{
    public const string shipName = "Drakkar";
    public const int maxHealth = 900;
    public const float maxSpeed = 70f;
    public const float maneuverability = ManeuverabilityLevel.MID;
    public const int mainAttackDmg = 500;
    public const float mainAttackCD = 1.5f;
    public const int specAttackDmg = 500;
    public const float specAttackCD = 1.5f;
    public const float defense = 0.10f;

}

public class Orientals
{
    public const string shipName = "Chinese Junk";
    public const int maxHealth = 1400;
    public const float maxSpeed = 60f;
    public const float maneuverability = ManeuverabilityLevel.LOW;
    public const int mainAttackDmg = 500;
    public const float mainAttackCD = 1.5f;
    public const int specAttackDmg = 500;
    public const float specAttackCD = 1.5f;
    public const float defense = 0.15f;

}

public enum BuffStatus
{
    
    regen,
    dmgUp,
    speedUp,
    yohoho
}

public enum DebuffStatus
{
    poison,
    blind
}