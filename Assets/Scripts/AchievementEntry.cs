using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AchievementEntry : MonoBehaviour
{
	public GameObject m_Image;
	public GameObject m_Title;
	public GameObject m_Desc;
	public GameObject m_Points;
	public GameObject m_Overlay;
	public GameObject m_BarFrame;
	public GameObject m_Bar;

	float m_BarLength = 285.0f;

	void Start ()
	{
	
	}

	void Update ()
	{
		
	}

	public void SetEntry(string title_, string desc_, float currPoints_, float maxPoints_)
	{
		m_Title.GetComponent<Text>().text = title_;
		m_Desc.GetComponent<Text>().text = desc_;
		m_Points.GetComponent<Text>().text = currPoints_ + " / " + maxPoints_; 
		m_Bar.GetComponent<RectTransform>().localScale = new Vector3(currPoints_/maxPoints_, m_Bar.GetComponent<RectTransform>().localScale.y, 1.0f);
	}
}

