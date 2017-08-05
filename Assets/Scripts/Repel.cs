using UnityEngine;
using System.Collections;

public class Repel : MonoBehaviour
{
	public const float REPEL_START_SCALE = 0.5f;
	public const float REPEL_END_SCALE = 1.5f;
	public const float REPEL_ANIM_TIME = 1.0f;

	private float m_fTimer = 0.0f;

	// Use this for initialization
	void Start ()
	{
		m_fTimer = 0.0f;
		Scale( REPEL_START_SCALE );
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fTimer += Time.deltaTime;
		float fScale = Mathf.SmoothStep( REPEL_START_SCALE, REPEL_END_SCALE, m_fTimer / REPEL_ANIM_TIME );
		Scale( fScale );
	}

	void Scale ( float fScale )
	{
		Transform parent = transform.parent;
		transform.parent = null;
		transform.localScale = fScale * Vector3.one;
		transform.parent = parent;
	}
}
