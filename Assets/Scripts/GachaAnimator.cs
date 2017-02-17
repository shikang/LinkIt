using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaAnimator : MonoBehaviour
{
	const float THROW_COIN_TIME = 0.5f;
	const float COIN_SANK_TIME = 1.0f;
	const float FLASH_TIME = 1.75f;
	const float WISH_TIME = 2.0f;
	const float WISH_FULFILL_TIME = 0.5f;

	enum GachaAnimationPhase
	{
		NONE = 0,
		THROW_COIN,
		MAKE_A_WISH,
		WISH_GRANTED,
		ITEM_RECIEVED,
	}

	public GameObject m_FountainDustPrefab;

	public SpriteRenderer m_Flash;
	public GameObject m_Coin;
	public GameObject m_Fountain;

	private float m_AnimationTimer = 0.0f;
	private int m_AnimationPhase = (int)GachaAnimationPhase.NONE;
	private Vector3 m_CoinStartPos;
	private Vector3 m_CoinEndPos;

	// Use this for initialization
	void Start ()
	{
		m_AnimationPhase = (int)GachaAnimationPhase.NONE;
		m_AnimationTimer = 0.0f;

		m_CoinStartPos = m_Coin.transform.localPosition;
		m_CoinEndPos = m_Fountain.transform.localPosition;
		m_Coin.SetActive( false );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( m_AnimationPhase != (int)GachaAnimationPhase.NONE )
			m_AnimationTimer += Time.deltaTime;

		switch ( m_AnimationPhase )
		{
			case (int)GachaAnimationPhase.THROW_COIN:
				{
					float factor = Mathf.Pow( Mathf.Clamp( m_AnimationTimer / THROW_COIN_TIME, 0.0f, 1.0f ), 2.0f );
					m_Coin.transform.localPosition = Vector3.Lerp( m_CoinStartPos, m_CoinEndPos, factor );

					if ( m_AnimationTimer >= THROW_COIN_TIME )
					{
						m_AnimationPhase = (int)GachaAnimationPhase.MAKE_A_WISH;
						m_AnimationTimer = 0.0f;

						// @todo Play coin drop sound effect
						// @todo Play magical sound effect

						GameObject fountainDust = ( GameObject )Instantiate( m_FountainDustPrefab, Vector3.zero, Quaternion.identity );
						fountainDust.transform.parent = m_Fountain.transform.parent;
						fountainDust.transform.localPosition = m_Fountain.transform.localPosition;
						fountainDust.transform.localScale = Vector3.one;
						ParticleSystem ps = fountainDust.GetComponent<ParticleSystem>();
						Destroy( fountainDust, ps.duration + ps.startLifetime + Time.deltaTime );
					}
				}
				break;

			case (int)GachaAnimationPhase.MAKE_A_WISH:
				{
					float factor = Mathf.Pow( Mathf.Clamp( m_AnimationTimer / COIN_SANK_TIME, 0.0f, 1.0f ), 2.0f );
					SpriteRenderer sr = m_Coin.GetComponent<SpriteRenderer>();

					Color c = sr.color;
					c.a = 1.0f - factor;
					sr.color = c;

					factor = Mathf.Pow( Mathf.Clamp( m_AnimationTimer / FLASH_TIME, 0.0f, 1.0f ), 2.0f );
					c = m_Flash.color;
					c.a = factor;
					m_Flash.color = c;

					if ( m_AnimationTimer >= WISH_TIME )
					{
						m_AnimationPhase = (int)GachaAnimationPhase.WISH_GRANTED;
						m_AnimationTimer = 0.0f;

						c.a = 1.0f;
						sr.color = c;
						m_Coin.SetActive( false );
					}
				}
				break;

			case (int)GachaAnimationPhase.WISH_GRANTED:
				{ 
					float factor = Mathf.Pow( Mathf.Clamp( m_AnimationTimer / WISH_FULFILL_TIME, 0.0f, 1.0f ), 2.0f );
					Color c = m_Flash.color;
					c.a = 1.0f - factor;
					m_Flash.color = c;

					if ( m_AnimationTimer >= WISH_FULFILL_TIME )
					{
						m_AnimationPhase = (int)GachaAnimationPhase.ITEM_RECIEVED;
						m_AnimationTimer = 0.0f;
					}
				}
				break;

			case (int)GachaAnimationPhase.ITEM_RECIEVED:
				{
					m_AnimationPhase = (int)GachaAnimationPhase.NONE;
					m_AnimationTimer = 0.0f;
				}
				break;

			case (int)GachaAnimationPhase.NONE:
			default:
				// Do nothing
				break;
		}
	}
	
	public void StartGachaAnimation()
	{
		m_AnimationPhase = (int)GachaAnimationPhase.THROW_COIN;

		m_Coin.transform.localPosition = m_CoinStartPos;
		m_Coin.SetActive( true );
		m_AnimationTimer = 0.0f;
	}

	public bool IsAnimating()
	{
		return m_AnimationPhase != (int)GachaAnimationPhase.NONE;
	}
}
