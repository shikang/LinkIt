using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
	private Text[] m_Text;

	public Color m_TextHighlightColor;
	public Color m_TextPressColor;
	public Color m_TextColor;

	private bool m_bClickInside;

	// Use this for initialization
	void Start ()
	{
		m_Text = GetComponentsInChildren<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		if ( Input.GetMouseButtonUp( 0 ) )
		{
			m_bClickInside = false;
		}
	}

	void OnMouseDown()
	{
		foreach ( Text t in m_Text )
		{
			t.color = m_TextPressColor;
		}
		m_bClickInside = true;
	}

	void OnMouseEnter()
	{
		if ( m_bClickInside )
		{
			foreach ( Text t in m_Text )
			{
				t.color = m_TextPressColor;
			}
		}
		else
		{
			foreach ( Text t in m_Text )
			{
				t.color = m_TextHighlightColor;
			}
		}
	}

	void OnMouseExit()
	{
		foreach ( Text t in m_Text )
		{
			t.color = m_TextColor;
		}
	}

	void OnMouseUp()
	{
		foreach ( Text t in m_Text )
		{
			t.color = m_TextColor;
		}
	}
}
