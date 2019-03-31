using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
	private Text[] m_Text;

	public Color m_TextHighlightColor;
	public Color m_TextPressColor;
	public Color m_TextColor;
	public Color m_DisableColor;

	private bool m_bDisable = false;
	private bool m_bClickInside;

	// Use this for initialization
	void Start ()
	{
		m_Text = GetComponentsInChildren<Text>();
		m_bDisable = false;

		Button b = GetComponent<Button>();
		if ( b != null )
			b.onClick.AddListener( delegate { OnClick(); } );
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
		if ( m_bDisable )
			return;

        if ( m_Text != null )
        {
		    foreach ( Text t in m_Text )
		    {
			    t.color = m_TextPressColor;
		    }
        }
		m_bClickInside = true;
	}

	void OnMouseEnter()
	{
		if ( m_bDisable )
			return;

		if ( m_bClickInside )
		{
            if ( m_Text != null )
            {
			    foreach ( Text t in m_Text )
			    {
				    t.color = m_TextPressColor;
			    }
            }
		}
		else
		{
            if ( m_Text != null )
            {
			    foreach ( Text t in m_Text )
			    {
				    t.color = m_TextHighlightColor;
			    }
            }
		}
	}

	void OnMouseExit()
	{
		if ( m_bDisable )
			return;

        if ( m_Text != null )
        {
		    foreach ( Text t in m_Text )
		    {
			    t.color = m_TextColor;
		    }
        }
	}

	void OnMouseUp()
	{
		if ( m_bDisable )
			return;

        if ( m_Text != null )
        {
		    foreach ( Text t in m_Text )
		    {
			    t.color = m_TextColor;
		    }
        }
	}

	public void SetDisable()
	{
		m_bDisable = true;

        if ( m_Text != null )
        {
		    foreach ( Text t in m_Text )
		    {
			    t.color = m_DisableColor;
		    }
        }
	}

	public void SetEnable()
	{
		m_bDisable = false;

        if ( m_Text != null )
        {
		    foreach ( Text t in m_Text )
		    {
			    t.color = m_TextColor;
		    }
        }
	}

	public void OnClick()
	{
		m_bClickInside = false;
	}
}
