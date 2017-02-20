using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
	public const int GACHA_COST = 100;
	public const float PRIZE_MOVE_TIME = 0.5f;

	public GameObject m_PrizeButton;
	public GameObject m_GachaButton;
	public GameObject m_BackButton;
	public GameObject m_ReceiveText;

	private bool m_AllowGacha = false;
	private GachaAnimator m_GachaAnimator;

	private Vector3 m_PrizeOutPosition;
	private Vector3 m_PrizePosition;
	private Vector3 m_BackPosition;
	private Vector3 m_ReceivePosition;

	private float m_fPrizeTimer = 0.0f;

	// Use this for initialization
	void Start ()
	{
		m_GachaAnimator = GetComponent<GachaAnimator>();

		m_AllowGacha = GameData.Instance.m_Coin >= GACHA_COST;
		m_PrizeButton.SetActive( m_AllowGacha );
		m_PrizeOutPosition = m_PrizePosition = m_PrizeButton.transform.localPosition;
		m_PrizeOutPosition.x = -( m_PrizeButton.GetComponent<RectTransform>().sizeDelta.x * 0.5f + m_PrizeButton.transform.parent.GetComponent<RectTransform>().sizeDelta.x * 0.5f );
		m_PrizeButton.transform.localPosition = m_PrizeOutPosition;

		m_fPrizeTimer = 0.0f;

		//m_GachaButton.SetActive( GameData.Instance.m_Coin >= GACHA_COST );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( m_AllowGacha )
		{
			m_fPrizeTimer += Time.deltaTime;

			float factor = Mathf.Pow( Mathf.Clamp( m_fPrizeTimer / PRIZE_MOVE_TIME, 0.0f, 1.0f ), 2.0f );
			m_PrizeButton.transform.localPosition = Vector3.Lerp( m_PrizeOutPosition, m_PrizePosition, factor );

			if ( m_fPrizeTimer >= PRIZE_MOVE_TIME )
			{
				m_AllowGacha = false;
			}
		}
	}

	public void Gacha()
	{
		if ( GameData.Instance.m_Coin < GACHA_COST )
			return;

		GameData.Instance.m_Coin -= GACHA_COST;
		int itemReceived = Random.Range( (int)GemLibrary.GemSet.GEM, (int)GemLibrary.GemSet.TOTAL );
		GemLibrary.GemSet gemType = (GemLibrary.GemSet)itemReceived;
		GameData.Instance.UnlockGem( gemType );
		SaveLoad.Save();

		m_ReceiveText.GetComponent<Text>().text = "Received " + GemLibrary.Instance.GemsSetList[itemReceived].m_sGemContainerSetName + "!";

		m_GachaAnimator.OnAfterAnimation = AfterGacha;
		m_GachaAnimator.StartGachaAnimation( gemType );
		m_GachaButton.SetActive( false );
		m_BackButton.SetActive( false );

		m_ReceivePosition = m_ReceiveText.transform.localPosition;
		m_ReceiveText.transform.parent = null;

		m_BackPosition = m_BackButton.transform.localPosition;
		m_BackButton.transform.parent = null;
	}

	void AfterGacha()
	{
		m_ReceiveText.transform.parent = m_GachaButton.transform.parent;
		m_ReceiveText.transform.localPosition = m_ReceivePosition;
		m_ReceiveText.SetActive( true );

		m_BackButton.transform.parent = m_GachaButton.transform.parent;
		m_BackButton.transform.localPosition = m_BackPosition;
		m_BackButton.SetActive( true );

		m_PrizeButton.SetActive( false );
	}
}
