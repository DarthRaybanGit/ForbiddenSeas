using System.Collections;
using System.Collections.Generic;

public class Pirates
{
    public const string shipName = "Pirate Galleon";
    public const int maxHealth = 1000;
    public const float maxSpeed = 10.5f;
	public const float maneuverability = ManeuverabilityLevel.MID;
    public const int mainAttackDmg = 450;
    public const float mainAttackCD = 2.5f;
    public const int specAttackDmg = 500;
    public const float specAttackCD = 5f;
    public const float defense = 0.15f;
    public const float mainDistance = -20f;
    public const float specialDistance = 5f;


}

public class Egyptians
{
    public const string shipName = "Solar Ship";
    public const int maxHealth = 800;
    public const float maxSpeed = 12f;
	public const float maneuverability = ManeuverabilityLevel.LOW;
    public const int mainAttackDmg = 90;  //FIXME
    public const float mainAttackCD = 2.7f;
    public const int specAttackDmg = 500;
    public const float specAttackCD = 10f;
    public const float defense = 0.05f;
    public const float mainDistance = -22f;
    public const float specialDistance = 5f;


}

public class Vikings
{
    public const string shipName = "Drakkar";
    public const int maxHealth = 900;
    public const float maxSpeed = 10f;
    public const float maneuverability = ManeuverabilityLevel.HIGH;
    public const int mainAttackDmg = 300;
    public const float mainAttackCD = 2.2f;
    public const int specAttackDmg = 500;
    public const float specAttackCD = 5f;
    public const float defense = 0.10f;
    public const float mainDistance = -22f;
    public const float specialDistance = 5f;

}

public class Orientals
{
    public const string shipName = "Chinese Junk";
    public const int maxHealth = 1400;
    public const float maxSpeed = 10f;
    public const float maneuverability = ManeuverabilityLevel.MID;
    public const int mainAttackDmg = 300;
    public const float mainAttackCD = 2.2f;
    public const int specAttackDmg = 85;
    public const float specAttackCD = 10f;
    public const float defense = 0.15f;
    public const float mainDistance = -22f;
    public const float specialDistance = 5f;


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