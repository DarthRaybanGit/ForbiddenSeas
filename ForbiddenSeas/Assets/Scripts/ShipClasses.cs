using System.Collections;
using System.Collections.Generic;

public class Pirates
{
    public const string shipName = "Pirate Galleon";
    public const int maxHealth = 1000;
    public const float maxSpeed = 60f;
    public const float maneuverability = ManeuverabilityLevel.LOW;
    public static Attack mainAttack = Cannon.cannon;
    public static Attack specialAttack = CannonHail.cannonHail;
}

public class Venetians
{
    public const string shipName = "War-type Gondola";
    public const int maxHealth = 800;
    public const float maxSpeed = 80f;
    public const float maneuverability = ManeuverabilityLevel.HIGH;
    public static Attack mainAttack = Cannon.cannon;
    public static Attack specialAttack = Cannon.cannon;
}

public class Vikings
{
    public const string shipName = "Drakkar";
    public const int maxHealth = 900;
    public const float maxSpeed = 70f;
    public const float maneuverability = ManeuverabilityLevel.MID;
    public static Attack mainAttack = Blaze.blaze;
    public static Attack specialAttack = Cannon.cannon;
}

public class Orientals
{
    public const string shipName = "Chinese Junk";
    public const int maxHealth = 1400;
    public const float maxSpeed = 60f;
    public const float maneuverability = ManeuverabilityLevel.LOW;
    public static Attack mainAttack = Cannon.cannon;
    public static Attack specialAttack = Cannon.cannon;
}

public class Attack
{
    public  string name;
    public int damage;
    public  float cooldown;
}

public class Cannon: Attack
{
    public static Cannon cannon = new Cannon();
    public Cannon()
    {
        name = "Cannon";
        damage = 500;
        cooldown = 0.9f;
    }
}

public class CannonHail: Attack
{
    public static CannonHail cannonHail = new CannonHail();
    public CannonHail()
    {
        name = "CannonHail";
        damage = 500;
        cooldown = 0.9f;
    }
}

public class Blaze: Attack
{
    public static Blaze blaze = new Blaze();
    public Blaze()
    {
        name = "CannonHail";
        damage = 500;
        cooldown = 0.9f;
    }
}