using UnityEngine;

public class FlockGlobals: MonoBehaviour
{
	public static FlockGlobals instance;
		
    public float m_sight_radius = 2f;
    public float m_alignment =  1f;
    public float m_cohesion = 1f;
    public float m_separation = 0.1f;
    public float limit = 9f;

    [Range(-1f, 1f)]
    public float m_visionAngle = -0.7f;
    public float m_speed = 10f;
    public float m_rotSpeed = 5f;

	void Awake()
    {
		instance = this;
    }
}
