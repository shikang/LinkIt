using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
	public const int GACHA_COST = 100;

	public GameObject m_GachaButton;
	public GameObject m_BackButton;

	private GachaAnimator m_GachaAnimator;

	private Vector3 m_BackPosition;

	// Use this for initialization
	void Start ()
	{
		m_GachaAnimator = GetComponent<GachaAnimator>();
		
		//m_GachaButton.SetActive( GameData.Instance.m_Coin >= GACHA_COST );
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void Gacha()
	{
		//if ( GameData.Instance.m_Coin < GACHA_COST )
		//	return;

		m_GachaAnimator.OnAfterAnimation = AfterGacha;
		m_GachaAnimator.StartGachaAnimation();
		m_GachaButton.SetActive( false );
		m_BackButton.SetActive( false );

		m_BackPosition = m_BackButton.transform.localPosition;
		m_BackButton.transform.parent = null;
	}

	void AfterGacha()
	{
		m_BackButton.transform.parent = m_GachaButton.transform.parent;
		m_BackButton.transform.localPosition = m_BackPosition;
		m_BackButton.SetActive( true );
	}
}
