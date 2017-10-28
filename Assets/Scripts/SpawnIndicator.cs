using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnIndicator : MonoBehaviour
{
	public Sprite[] m_GemTypes;
	public Sprite[] m_ChosenGemTypes;

	private List<GameObject> m_SequenceList;

	// Use this for initialization
	void Start ()
	{
		m_SequenceList = new List<GameObject>();

		int children = transform.childCount;
		for ( int i = 0; i < children; ++i )
		{
			GameObject gem = transform.GetChild( i ).gameObject;
			m_SequenceList.Add( gem );
			gem.SetActive( false );
		}
	}

	public void SetGemTypeCheck( int index, int type, bool active, bool chosen )
	{
		GameObject gem = m_SequenceList[index];
		
		if ( active )
		{
			gem.SetActive( true );
			gem.GetComponent<SpriteRenderer>().sprite = chosen ? m_ChosenGemTypes[type] : m_GemTypes[type];
			Color c = gem.GetComponent<SpriteRenderer>().color;
			c.a = chosen ? 1.0f : 0.7f;
			gem.GetComponent<SpriteRenderer>().color = c;
		}
		else
		{
			gem.SetActive( false );
		}
	}
}
