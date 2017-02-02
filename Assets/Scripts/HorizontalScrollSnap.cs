using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class HorizontalScrollSnap : MonoBehaviour
{
	public const float SNAP_SPEED = 1.0f;

	public RectTransform m_ContentTransform;

	private ShopManager m_ShopManager;

	private List<Vector3> m_Positions;
	private ScrollRect m_ScrollRect;
	private Vector3 m_LerpTarget;
	private bool m_bLerp;
	private bool m_bScrollStop;

	// Use this for initialization
	void Start()
	{
		m_ScrollRect = gameObject.GetComponent<ScrollRect>();
		//m_ScrollRect.inertia = false;
		m_bLerp = false;
		m_bScrollStop = false;

		m_ShopManager = GameObject.FindGameObjectWithTag( "Shop Manager" ).GetComponent<ShopManager>();

		m_Positions = new List<Vector3>();

		int nItemCount = m_ShopManager.ItemIcons.Count;
		if ( nItemCount > 0 )
		{
			Vector3 pos = m_ContentTransform.localPosition;
			for ( int i = 0; i < nItemCount; ++i )
			{
				//m_ScrollRect.horizontalNormalizedPosition = (float)i / (float)(m_ItemCount - 1);
				m_Positions.Add( pos );
				pos.x -= ( m_ShopManager.ItemIconWidth + ShopManager.ITEM_PAD );
			}
		}

		//m_ScrollRect.horizontalNormalizedPosition = (float)(m_StartingIndex) / (float)(m_ItemCount - 1);
	}

	void Update()
	{
		if ( m_bLerp && m_bScrollStop )
		{
			m_ScrollRect.inertia = false;
			m_ContentTransform.localPosition = Vector3.Lerp( m_ContentTransform.localPosition, m_LerpTarget, 10 * Time.deltaTime );
			if ( Vector3.Distance (m_ContentTransform.localPosition, m_LerpTarget ) < 0.001f )
			{
				m_bLerp = false;
				m_bScrollStop = false;
				m_ScrollRect.inertia = true;
			}
		}
	}

	public void DragEnd()
	{
		if ( m_ScrollRect.horizontal )
		{
			if ( m_ScrollRect.velocity.sqrMagnitude <= SNAP_SPEED )
			{
				SetScrollStop();
			}

			m_bLerp = true;

			if ( m_bScrollStop )
				m_LerpTarget = FindClosestFrom( m_ContentTransform.localPosition, m_Positions );
		}
	}

	public void SetScrollStop()
	{
		m_bScrollStop = true;

		if ( m_bLerp )
			m_LerpTarget = FindClosestFrom( m_ContentTransform.localPosition, m_Positions );
	}

	public void OnDrag()
	{
		m_bLerp = false;
		m_bScrollStop = false;
		m_ScrollRect.inertia = true;
	}

	Vector3 FindClosestFrom( Vector3 start, List<Vector3> positions )
	{
		Vector3 closest = Vector3.zero;
		float distance = Mathf.Infinity;

		foreach ( Vector3 position in m_Positions )
		{
			if ( Vector3.Distance( start, position ) < distance )
			{
				distance = Vector3.Distance( start, position );
				closest = position;
			}
		}

		return closest;
	}

	public int FindClosestIndex()
	{
		int closest = 0;
		float distance = Mathf.Infinity;

		for ( int i = 0; i < m_Positions.Count; ++i )
		{
			Vector3 position = m_Positions[i];
			if ( Vector3.Distance( m_ContentTransform.localPosition, position ) < distance )
			{
				distance = Vector3.Distance( m_ContentTransform.localPosition, position );
				closest = i;
			}
		}

		return closest;
	}
}