using UnityEngine;
using System.Collections;

public class HighComboColor : MonoBehaviour
{
	public const float INTERPOLATE_DURATION = 0.5f;

	public Color[] m_ComboColor;
    public Color[] m_ComboParticleColor;
    SpriteRenderer m_SpriteRenderer;

	bool m_bInterpolate = false;
	float m_Timer = 0.0f;
	int nIndex = 0;

	// Use this for initialization
	void Start ()
	{
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( m_bInterpolate )
		{
			m_Timer += Time.deltaTime;
			float factor = Mathf.Clamp( m_Timer / INTERPOLATE_DURATION, 0.0f, 1.0f );
			m_SpriteRenderer.color = Color.Lerp( m_ComboColor[nIndex - 1], m_ComboColor[nIndex], factor );

			m_bInterpolate = m_Timer < INTERPOLATE_DURATION;
		}
	}

	public void SetComboColor( int index )
	{
		if ( m_SpriteRenderer == null )
			m_SpriteRenderer = GetComponent<SpriteRenderer>();

		if ( index == 0 )
		{
			m_SpriteRenderer.color = m_ComboColor[index];
			m_bInterpolate = false;
			AudioManager.Instance.UpdateHighComboIndex(index);
			AudioManager.Instance.PlaySoundEvent(SOUNDID.FEVER_ENTER);
		}
		else
		{
			// Start interpolation
			m_bInterpolate = true;
			m_Timer = 0.0f;
			nIndex = index;
			AudioManager.Instance.PlaySoundEvent(SOUNDID.FEVER_SUSTAIN);
		}
	}

    public Color GetComboParticleColor( int index )
    {
        return m_ComboParticleColor[index];
    }
}
