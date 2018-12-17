using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Popup : MonoBehaviour
{
	public const float FADE_TIME = 0.25f;				//!< In Seconds
	public const float FADE_RATE = 1.0f / FADE_TIME;

	public GameObject m_MainCanvas;
	public List<GameObject> m_OtherSprites;

	private CanvasGroup m_CanvasGroup;

	enum PopupState
	{
		NONE,
		FADING_OUT_MAIN,
		FADING_IN_POP,
		SHOWING_POP,
		FADING_OUT_POP,
		FADING_IN_MAIN,
		RE_FADING_OUT_POP,
		RE_FADING_IN_POP,
	};

	private PopupState m_State = PopupState.NONE;

	// Use this for initialization
	void Start ()
	{
		m_CanvasGroup = GetComponent<CanvasGroup>();
		m_CanvasGroup.alpha = 0.0f;

		// m_MainCanvas.GetComponent<CanvasGroup>().alpha = 1.0f;

		m_State = PopupState.NONE;
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch ( m_State )
		{
			case PopupState.FADING_OUT_MAIN:
				m_MainCanvas.GetComponent<CanvasGroup>().alpha -= FADE_RATE * Time.deltaTime;

				foreach ( GameObject obj in m_OtherSprites )
				{
					SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
					if ( sr != null )
					{
						Color c = sr.color;
						c.a -= FADE_RATE * Time.deltaTime;
						c.a = Math.Max( 0.0f, c.a );
						sr.color = c;
					}
				}

				if ( m_MainCanvas.GetComponent<CanvasGroup>().alpha <= 0.0f )
				{
					m_MainCanvas.GetComponent<CanvasGroup>().alpha = 0.0f;
					m_MainCanvas.SetActive( false );
					m_State = PopupState.FADING_IN_POP;
				}
				break;
			case PopupState.FADING_IN_POP:
			case PopupState.RE_FADING_IN_POP:
				m_CanvasGroup.GetComponent<CanvasGroup>().alpha += FADE_RATE * Time.deltaTime;

				if ( m_CanvasGroup.GetComponent<CanvasGroup>().alpha >= 1.0f )
				{
					m_CanvasGroup.GetComponent<CanvasGroup>().alpha = 1.0f;
					m_State = PopupState.SHOWING_POP;
				}
				break;
			case PopupState.SHOWING_POP:
				// Detect on tap than move to next state
				// @todo check if tap skip is enable

				// iterate reverse so we can remove entries
				for ( int i = Input.touches.Length - 1 ; i >= 0; --i ) 
				{
					Touch touch = Input.touches[i];

					if ( touch.phase == TouchPhase.Ended)
					{
						m_State = PopupState.FADING_OUT_POP;
						break;
					}
				}
				break;
			case PopupState.FADING_OUT_POP:
			case PopupState.RE_FADING_OUT_POP:
				m_CanvasGroup.GetComponent<CanvasGroup>().alpha -= FADE_RATE * Time.deltaTime;

				if ( m_CanvasGroup.GetComponent<CanvasGroup>().alpha <= 0.0f )
				{
					m_CanvasGroup.GetComponent<CanvasGroup>().alpha = 0.0f;
					m_MainCanvas.SetActive( true );
					m_State = ( m_State == PopupState.FADING_OUT_POP ) ? PopupState.FADING_IN_MAIN : PopupState.RE_FADING_IN_POP;
				}
				break;
			case PopupState.FADING_IN_MAIN:
				m_MainCanvas.GetComponent<CanvasGroup>().alpha += FADE_RATE * Time.deltaTime;

				foreach ( GameObject obj in m_OtherSprites )
				{
					SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
					if ( sr != null )
					{
						Color c = sr.color;
						c.a += FADE_RATE * Time.deltaTime;
						c.a = Math.Min( 1.0f, c.a );
						sr.color = c;
					}
				}

				if ( m_MainCanvas.GetComponent<CanvasGroup>().alpha >= 1.0f )
				{
					m_MainCanvas.GetComponent<CanvasGroup>().alpha = 1.0f;
					m_State = PopupState.NONE;
				}
				break;
		}
	}

	public void ShowPopup( string message )
	{
		if ( m_State == PopupState.NONE || m_State == PopupState.SHOWING_POP )
		{
			m_State = ( m_State == PopupState.SHOWING_POP ) ? PopupState.RE_FADING_OUT_POP : PopupState.FADING_OUT_MAIN;
			Text text = GetComponentInChildren<Text>();
			text.text = message;
		}
	}

	public static Popup GetPopup()
	{
		GameObject popObj = GameObject.FindGameObjectWithTag( "Popup" );
		if ( popObj != null )
		{
			return popObj.GetComponent<Popup>();
		}
		else
		{
			Debug.Log( "Cannot find popup" );
			return null;
		}
	}
}
