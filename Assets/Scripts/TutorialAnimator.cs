using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialAnimator : MonoBehaviour
{
	const float START_SHOWING_DELAY = 2.0f;
	public const float TAP_DURATION = 1.5f;
	readonly float[] LINK_DURATION = { 1.0f, 1.0f };
	public const float LINK_PAUSE = 0.25f;
	public const float UNLINK_WAIT = 0.5f;

	public const float HIDING_TIME = 0.5f;

	enum TutorialAnimationPhase
	{
		TUTORIAL_PHASE_START = 0,

		TAP_ANIMATION = TUTORIAL_PHASE_START,
		LINK_ANIMATION_1,
		LINK_ANIMATION_2,
		UNLINK_ANIMATION,
		UNLINK_ANIMATION_WAIT,

		TUTORIAL_PHASE_END,
	}

	public int m_nLoopTimes = 4;
	public GameObject m_TutorialText1;
	public GameObject m_TutorialText2;
	public Sprite m_InitialImage;
	public Sprite m_TapImage;
	public Vector3[] m_LinkPos;
	public GameObject[] m_LinkedGems;
	public GameObject[] m_OtherGems;
	public GameObject m_ExplosionPrefab;

	private bool m_bStartShowing = false;
	private bool m_bStartHiding = false;
	private int m_nLoopCounter = 0;
	private TutorialAnimationPhase m_eCurrentAnimationPhase = TutorialAnimationPhase.TUTORIAL_PHASE_START;
	private float m_fTimer = 0.0f;
	private List<Vector3> m_CopyLinkPos;

	// Use this for initialization
	void Start ()
	{
		if ( GameData.Instance.m_SeenTutorial )
		{
			DisableTutorial();
			return;
		}

#if LINKIT_COOP
		if ( NetworkManager.IsConnected() )
		{
			DisableTutorial();
			return;
		}
#endif	// LINKIT_COOP

		m_nLoopCounter = 0;

		m_CopyLinkPos = new List<Vector3>();
		m_CopyLinkPos.Add( transform.position );
		for ( int i = 0; i < m_LinkPos.Length; ++i )
		{
			m_CopyLinkPos.Add( m_LinkPos[i] );
		}

		m_bStartShowing = false;
		m_bStartHiding = false;

		StartAnimation();
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fTimer += Time.deltaTime;
		if ( !m_bStartShowing )
		{
			if ( m_fTimer >= START_SHOWING_DELAY )
			{
				m_fTimer -= START_SHOWING_DELAY;
				m_bStartShowing = true;
			}
			else
			{
				return;
			}
		}

		if ( m_bStartHiding )
		{
			Hiding();

			return;
		}

		switch ( m_eCurrentAnimationPhase )
		{
			case TutorialAnimationPhase.TUTORIAL_PHASE_START:
				AnimateTap();
				break;

			case TutorialAnimationPhase.LINK_ANIMATION_1:
			case TutorialAnimationPhase.LINK_ANIMATION_2:
				AnimateLink( (int)m_eCurrentAnimationPhase - (int)TutorialAnimationPhase.LINK_ANIMATION_1 + 1 );
				break;

			case TutorialAnimationPhase.UNLINK_ANIMATION:
				AnimateUnlink();
				break;

			case TutorialAnimationPhase.UNLINK_ANIMATION_WAIT:
				UnlinkWait();
				break;

			case TutorialAnimationPhase.TUTORIAL_PHASE_END:
			default:
				EndAnimation();
				break;
		}
	}

	void StartAnimation()
	{
		if ( m_InitialImage != null )
		{
			GetComponent<SpriteRenderer>().sprite = m_InitialImage;
		}

		transform.position = m_CopyLinkPos[0];
		m_eCurrentAnimationPhase = TutorialAnimationPhase.TUTORIAL_PHASE_START;
		m_fTimer = 0.0f;

		TrailRenderer trailRenderer = GetComponentInChildren<TrailRenderer>();
		Material trail = trailRenderer.material;
		trail.SetColor( "_Color", GameObject.FindGameObjectWithTag( "Gem Spawner" ).GetComponent<GemSpawner>().m_NeutralColor );

		for ( int i = 0; i < m_LinkedGems.Length; ++i )
		{
			m_LinkedGems[i].GetComponent<SpriteRenderer>().sprite = m_LinkedGems[i].GetComponent<GemSpriteContainer>().m_Sprites[0];
			Color c = m_LinkedGems[i].GetComponent<SpriteRenderer>().color;
			c.a = 1.0f;
			m_LinkedGems[i].GetComponent<SpriteRenderer>().color = c;
		}
	}

	void EndAnimation()
	{
		if ( ++m_nLoopCounter < m_nLoopTimes )
		{
			if ( m_nLoopCounter >= m_nLoopTimes / 2 )
			{
				m_TutorialText1.SetActive( false );
				m_TutorialText2.SetActive( true );
			}

			GetComponentInChildren<TrailRenderer>().Clear();
			StartAnimation();
		}
		else
		{
			m_bStartHiding = true;
			m_fTimer = 0.0f;
		}
	}

	void AnimateTap()
	{
		if ( m_fTimer >= TAP_DURATION * 0.5f )
		{
			GetComponent<SpriteRenderer>().sprite = m_TapImage;
		}

		// End timer
		if ( m_fTimer >= TAP_DURATION )
		{
			GoNextPhase();
		}
	}

	void AnimateLink( int index )
	{
		float factor = Mathf.Clamp( m_fTimer / ( LINK_DURATION[index - 1] - LINK_PAUSE ), 0.0f, 1.0f );

		if ( index % 2 == 1 )
		{
			transform.position = Vector3.Lerp( m_CopyLinkPos[index - 1], m_CopyLinkPos[index], factor );
		}
		else
		{
			transform.position = Vector3.Slerp( m_CopyLinkPos[index - 1], m_CopyLinkPos[index], factor );
		}

		if ( index == 1 && m_fTimer >= LINK_DURATION[index - 1] * 0.4f )
		{
			m_LinkedGems[0].GetComponent<SpriteRenderer>().sprite = m_LinkedGems[0].GetComponent<GemSpriteContainer>().m_GlowSprites[0];

			TrailRenderer trailRenderer = GetComponentInChildren<TrailRenderer>();
			Material trail = trailRenderer.material;
			trail.SetColor( "_Color", GameObject.FindGameObjectWithTag( "Gem Spawner" ).GetComponent<GemSpawner>().m_LinkColours[0] );
		}

		if ( m_fTimer >= LINK_DURATION[index - 1] - LINK_PAUSE )
		{
			m_LinkedGems[index].GetComponent<SpriteRenderer>().sprite = m_LinkedGems[index].GetComponent<GemSpriteContainer>().m_GlowSprites[0];
		}

		// End timer
		if ( m_fTimer >= LINK_DURATION[index - 1] )
		{
			GoNextPhase();
		}
	}

	void AnimateUnlink()
	{
		if ( m_InitialImage != null )
		{
			GetComponent<SpriteRenderer>().sprite = m_InitialImage;
		}

		for ( int i = 0; i < m_LinkedGems.Length; ++i )
		{
			m_LinkedGems[i].GetComponent<SpriteRenderer>().sprite = m_LinkedGems[i].GetComponent<GemSpriteContainer>().m_Sprites[0];
			Color c = m_LinkedGems[i].GetComponent<SpriteRenderer>().color;
			c.a = 0.0f;
			m_LinkedGems[i].GetComponent<SpriteRenderer>().color = c;

			GameObject explosion = ( GameObject )Instantiate( m_ExplosionPrefab, m_LinkedGems[i].transform.position, Quaternion.identity );
			ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
			ps.startColor = GameObject.FindGameObjectWithTag( "Gem Spawner" ).GetComponent<GemSpawner>().m_LinkColours[0];
			Destroy( explosion, ps.duration + ps.startLifetime + Time.deltaTime );
		}

		GoNextPhase();
	}

	void UnlinkWait()
	{
		// End timer
		if ( m_fTimer >= UNLINK_WAIT )
		{
			GoNextPhase();
		}
	}

	void GoNextPhase()
	{
		++m_eCurrentAnimationPhase;
		m_fTimer = 0.0f;
	}

	void Hiding()
	{
		float factor = Mathf.Clamp( m_fTimer / HIDING_TIME, 0.0f, 1.0f );

		if ( m_fTimer >= HIDING_TIME )
		{
			GameData.Instance.m_SeenTutorial = true;
			//SaveLoad.Save();
			SaveDataLoader.SaveGame();
			DisableTutorial();
		}
		else
		{
			/*
			for ( int i = 0; i < m_LinkedGems.Length; ++i )
			{
				Color c = m_LinkedGems[i].GetComponent<SpriteRenderer>().color;
				c.a = 1.0f - factor;
				m_LinkedGems[i].GetComponent<SpriteRenderer>().color = c;
			}
			*/

			for ( int i = 0; i < m_OtherGems.Length; ++i )
			{
				Color c = m_OtherGems[i].GetComponent<SpriteRenderer>().color;
				c.a = 1.0f - factor;
				m_OtherGems[i].GetComponent<SpriteRenderer>().color = c;
			}

			{
				Color c = m_TutorialText2.GetComponent<Text>().color;
				c.a = 1.0f - factor;
				m_TutorialText2.GetComponent<Text>().color = c;
			}

			{
				Color c = GetComponent<SpriteRenderer>().color;
				c.a = 1.0f - factor;
				GetComponent<SpriteRenderer>().color = c;
			}
		}
	}

	void DisableTutorial()
	{
		for ( int i = 0; i < m_LinkedGems.Length; ++i )
		{
			m_LinkedGems[i].SetActive( false );
		}

		for ( int i = 0; i < m_OtherGems.Length; ++i )
		{
			m_OtherGems[i].SetActive( false );
		}

		transform.parent.gameObject.SetActive( false );

		m_TutorialText1.SetActive( false );
		m_TutorialText2.SetActive( false );
		gameObject.SetActive( false );
	}
}
