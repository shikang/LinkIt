using UnityEngine;
using System.Collections;

public class NewHighScoreFX : MonoBehaviour
{
	bool m_isScaleUp = true;

	void Start ()
	{
	
	}

	void Update ()
	{
		if(m_isScaleUp)
		{
			transform.localScale *= 1.015f;
			if (transform.localScale.x > 1.25f)
				m_isScaleUp = !m_isScaleUp;
		}
		else
		{
			transform.localScale *= 0.985f;
			if (transform.localScale.x < 1.0f)
				m_isScaleUp = !m_isScaleUp;
		}
	}
}

