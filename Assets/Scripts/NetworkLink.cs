using UnityEngine;
using System.Collections;

public class NetworkLink : Photon.MonoBehaviour
{
	private const float LINK_ALPHA = 0.25f;

	private Vector3 m_HalfDimension;
	private Link m_Link;

	// Use this for initialization
	void Start ()
	{
		m_HalfDimension = Camera.main.ScreenToWorldPoint( new Vector3( ( Screen.width ), ( Screen.height ) ) );
		m_Link = GetComponent<Link>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		if ( stream.isWriting )
		{
			// Sending data over
			stream.SendNext( ( transform.position.x - -m_HalfDimension.x ) / ( m_HalfDimension.x * 2.0f ) );
			stream.SendNext( ( transform.position.y - -m_HalfDimension.y ) / ( m_HalfDimension.y * 2.0f ) );
			stream.SendNext( ( GetComponent<Link>().Linking ) );
			stream.SendNext( ( GetComponent<Link>().LinkType ) );
		}
		else
		{
			// Receiving data
			float factorX = ( float )stream.ReceiveNext();
			float factorY = ( float )stream.ReceiveNext();

			bool linking = ( bool )stream.ReceiveNext();
			int linkType = ( int )stream.ReceiveNext();

			if ( linkType != m_Link.LinkType )
			{
				if ( linkType == GemSpawner.INVALID_GEM )
				{
					// Ignore
				}
				else
				{
					m_Link.ChangeLinkColor( linkType );
					m_Link.SetLinkOpacity( LINK_ALPHA );
				}
			}

			if ( linking != m_Link.Linking )
			{
				if ( !linking )
				{
					m_Link.BreakLink();
					m_Link.SetLinkOpacity( LINK_ALPHA );
				}
				else
				{
					m_Link.StartLink();
				}
			}

			Vector3 pos = transform.position;
			pos.x = ( factorX * m_HalfDimension.x * 2.0f ) + -m_HalfDimension.x;
			pos.y = ( factorY * m_HalfDimension.y * 2.0f ) + -m_HalfDimension.y;
			transform.position = pos;
		}
	}

	void OnPhotonInstantiate( PhotonMessageInfo info )
	{
		if( !photonView.isMine )
		{
			m_HalfDimension = Camera.main.ScreenToWorldPoint( new Vector3( ( Screen.width ), ( Screen.height ) ) );
			m_Link = GetComponent<Link>();
			m_Link.SetLinkOpacity( LINK_ALPHA );
		}
	}

}
