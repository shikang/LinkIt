using UnityEngine;
using System.Collections;

public class ExitGame : MonoBehaviour
{
	public GameObject m_gPopup;

	void Start ()
	{
		m_gPopup.SetActive (false);
	}

	void Update ()
	{
	
	}

	public void ExitYes()
	{
		m_gPopup.SetActive(false);
		AudioManager.Instance.PlaySoundEvent(SOUNDID.MENU_CLICK);
	}

	public void ExitNo()
	{
		m_gPopup.SetActive(false);
		AudioManager.Instance.PlaySoundEvent(SOUNDID.MENU_CLICK);
	}

	public void ClickToExit()
	{
		m_gPopup.SetActive(true);
		AudioManager.Instance.PlaySoundEvent(SOUNDID.MENU_CLICK);
	}
}

