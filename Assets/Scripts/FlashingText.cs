using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlashingText : MonoBehaviour
{
	public Color[] m_Colors;
	public float m_OneCycleInSeconds;

	private Text m_Text = null;
	private float m_fTimer = 0.0f;

	// Use this for initialization
	void Start ()
	{
		m_Text = GetComponent<Text>();
		m_fTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fTimer += Time.deltaTime;

		int index = (int)( m_fTimer / m_OneCycleInSeconds * m_Colors.Length ) % m_Colors.Length;
		m_Text.color = m_Colors[index];
	}
}
