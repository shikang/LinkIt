using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
	private GachaAnimator m_GachaAnimator;

	// Use this for initialization
	void Start ()
	{
		m_GachaAnimator = GetComponent<GachaAnimator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void Gacha()
	{
		m_GachaAnimator.StartGachaAnimation();
	}
}
