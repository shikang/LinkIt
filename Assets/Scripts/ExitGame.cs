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
	}

	public void ExitNo()
	{
		m_gPopup.SetActive(false);
	}

	public void ClickToExit()
	{
		m_gPopup.SetActive(true);
	}
}

