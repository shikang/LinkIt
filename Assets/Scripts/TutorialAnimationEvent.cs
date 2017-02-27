using UnityEngine;
using System.Collections;

public class TutorialAnimationEvent : MonoBehaviour
{
	private int m_nCounter = 1;
	private Animator m_Animator = null;
	// Use this for initialization
	void Start ()
	{
		m_nCounter = 1;
		m_Animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		AnimatorStateInfo state = m_Animator.GetCurrentAnimatorStateInfo( 0 );

		if( state.IsName( "Finish" ) )
		{
			gameObject.SetActive( false );
		}
	}

	public void IncrementAnimationCounter()
	{
		m_Animator.SetInteger( "Counter", ++m_nCounter );
	}
}
