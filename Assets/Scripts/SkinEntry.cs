using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class SkinEntry : MonoBehaviour
{
    public const int COST = 20000;

	public GemLibrary.GemSet m_Type;
	int m_bCost;

	public GameObject m_goBtnFX;
	public GameObject m_Image;
	public GameObject m_Title;
	public GameObject m_Desc;
	public GameObject m_Cost;
	public GameObject m_EquipButton;
	public GameObject m_EquipText;
	public GameObject m_Overlay;
	public GameObject m_OverlayText;
	public GameObject m_OverlayLock;

	bool isDimming = true;

	void Start ()
	{
	}

	void Update ()
	{
#if DISABLE_OVERLAY
        if(m_Type == BOOSTERTYPE.ScoreMult_Once)
		{
			m_Overlay.SetActive(!GameData.Instance.m_bUnlock_Games);
			m_OverlayText.SetActive(!GameData.Instance.m_bUnlock_Games);
			m_OverlayLock.SetActive(!GameData.Instance.m_bUnlock_Games);
			m_OverlayText.GetComponent<Text>().text = "PLAY 5 GAMES TO UNLOCK!";
		}
		else if(m_Type == BOOSTERTYPE.GoldMult_Once)
		{
			m_Overlay.SetActive(!GameData.Instance.m_bUnlock_EarnPoints);
			m_OverlayText.SetActive(!GameData.Instance.m_bUnlock_EarnPoints);
			m_OverlayLock.SetActive(!GameData.Instance.m_bUnlock_EarnPoints);
			m_OverlayText.GetComponent<Text>().text = "GET 20,000 POINTS IN ONE GAME TO UNLOCK!";
		}
		else if(m_Type == BOOSTERTYPE.MoreHealth_Once)
		{
			m_Overlay.SetActive(!GameData.Instance.m_bUnlock_Share_FB);
			m_OverlayText.SetActive(!GameData.Instance.m_bUnlock_Share_FB);
			m_OverlayLock.SetActive(!GameData.Instance.m_bUnlock_Share_FB);
			m_OverlayText.GetComponent<Text>().text = "LIKE LINKIT FACEBOOK PAGE TO UNLOCK!";
		}
#endif

		if(m_OverlayText.GetActive())
		{
			Color tmp = m_OverlayText.GetComponent<Text>().color;
			if(isDimming)
			{
				tmp.a -= 0.01f;
				if(tmp.a <= 0.4f)
					isDimming = false;
			}
			else
			{
				tmp.a += 0.01f;
				if(tmp.a >= 1.0f)
					isDimming = true;
			}
			m_OverlayText.GetComponent<Text>().color = tmp;
		}

		if(GameData.Instance.m_Coin < m_bCost)
		{
			m_Cost.GetComponent<Text>().color = Color.red;
			m_EquipButton.GetComponent<Image> ().color = new Color (0.6f, 0.6f, 0.6f);
		}
		else
		{
			m_Cost.GetComponent<Text>().color = Color.white;
			m_EquipButton.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
		}
	}

	public void SetEntry(GemLibrary.GemSet type_)
	{
		m_Type = type_;
		m_Image.GetComponent<Image>().sprite = BoosterManager.Instance.m_AllImages[(int)m_Type];
		m_Title.GetComponent<Text>().text = GemLibrary.Instance.GemsSetList[(int)m_Type].m_sGemContainerSetName;
		m_Desc.GetComponent<Text>().text = GemLibrary.Instance.GemsSetList[(int)m_Type].m_Desc;
		m_Cost.GetComponent<Text>().text = (COST != 0) ? COST.ToString() : "";
		SetButtonText();
		m_Overlay.SetActive(false);
		m_OverlayText.SetActive(false);
		m_OverlayLock.SetActive(false);
		m_bCost = COST;
	}

	public void PressButton()
	{
		int skinCost = m_bCost;

        if ( !GameData.Instance.m_Sets.Contains( m_Type ) )
        {
            if ( BuySkin( skinCost ) )
			{
				GameData.Instance.UnlockGem( m_Type );
                //SaveLoad.Save();
                StartBtnClickFX();
				SaveDataLoader.SaveGame();
			}
        }
        else
        {
            GameObject gemDetails = GameObject.Find( "Gem Details" );
			GemDetails gd = gemDetails.GetComponent<GemDetails>();
			gd.EquipGemSet( m_Type );
        }

		SetButtonText();
	}

	void SetButtonText()
	{
		m_EquipText.GetComponent<Text>().text = "BUY!";

		m_EquipButton.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
        if ( GameData.Instance.m_Sets.Contains( m_Type ) )
        {
            m_Cost.GetComponent<Text>().text = "";

            if ( m_Type == GameData.Instance.m_EquippedGemSet )
		    {
			    m_EquipText.GetComponent<Text>().text = "In Use";
			    m_EquipButton.GetComponent<Image> ().color = new Color (1.0f, 0.87f, 0.0f); // gold
		    }
            else
            {
                m_EquipText.GetComponent<Text>().text = "Equip";
            }
        }
	}

	bool BuySkin(int cost_)
	{
		if(GameData.Instance.m_Coin >= cost_)
		{
			GameData.Instance.m_Coin -= cost_;
			return true;
		}
		return false;
	}

	void StartBtnClickFX()
	{
        if (m_goBtnFX != null)
        {
            GameObject go = (GameObject)GameObject.Instantiate(m_goBtnFX);
            go.transform.parent = m_EquipButton.transform;
            go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
	}
}

