using UnityEngine;
using System.Collections;

public class LevelUpAnimation : MonoBehaviour
{
	public const float ANIM_TIME = 0.5f;       //!< In seconds

	private float m_fSpeed;

	// Use this for initialization
	void Start ()
	{
		Vector2 halfDimension = Camera.main.ScreenToWorldPoint( new Vector3( ( Screen.width ), ( Screen.height ) ) );
		m_fSpeed = halfDimension.y * 2.0f / ANIM_TIME;

		Vector3 pos = transform.position;
		pos.y = -halfDimension.y;
		pos.z = 2 * GemSpawner.FRONT_OFFSET.z;
		transform.position = pos;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 pos = transform.position;
		pos.y += m_fSpeed * Time.deltaTime;
		transform.position = pos;
	}
}
