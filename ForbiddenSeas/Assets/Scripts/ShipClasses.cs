using System.Collections;
using System.Collections.Generic;

public class Pirates
{
    public const string shipName = "Pirate Galleon";
    public const int maxHealth = 1000;
    public const float maxSpeed = 60f;
    public const float maneuverability = ManeuverabilityLevel.LOW;
    public Attack mainAttack = new Cannon();
    public Attack specialAttack = new Cannon();
}

public class Venetians
{
    public const string shipName = "War-type Gondola";
    public const int maxHealth = 800;
    public const float maxSpeed = 80f;
    public const float maneuverability = ManeuverabilityLevel.HIGH;
    public Attack mainAttack = Cannon.cannon;
    public Attack specialAttack = Cannon.cannon;
}

public class Vikings
{
    public const string shipName = "Drakkar";
    public const int maxHealth = 900;
    public const float maxSpeed = 70f;
    public const float maneuverability = ManeuverabilityLevel.MID;
    public Attack mainAttack = Cannon.cannon;
    public Attack specialAttack = Cannon.cannon;
}

public class Orientals
{
    public const string shipName = "Chinese Junk";
    public const int maxHealth = 1400;
    public const float maxSpeed = 60f;
    public const float maneuverability = ManeuverabilityLevel.LOW;
    public Attack mainAttack = Cannon.cannon;
    public Attack specialAttack = Cannon.cannon;
}

public class Attack{}

public class Cannon: Attack
{
    public static Cannon cannon = new Cannon();
    public const string name = "Cannon";
    public int damage = 500;
    public const float cooldown = 0.9f;
}