using UnityEngine;
using System.Collections;

public class GachaLightBurst : MonoBehaviour
{
	public const float DEFAULT_DEGREE_PER_SECOND = 90.0f;

	public float m_fDegreePerSecond = DEFAULT_DEGREE_PER_SECOND;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate( Vector3.forward * m_fDegreePerSecond * Time.deltaTime );
	}
}
