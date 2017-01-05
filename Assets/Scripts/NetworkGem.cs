using UnityEngine;
using System.Collections;

public class NetworkGem : Photon.MonoBehaviour
{
	const float DISTANCE_THRESHOLD = 0.05f;

	private Vector3 m_HalfDimension;
	private Gem m_Gem = null;
	private bool m_bOtherLinked = false;

	public bool OtherLinked
	{
		get
		{
			return m_bOtherLinked;
		}
	}

	// Use this for initialization
	void Start ()
	{
		m_HalfDimension = Camera.main.ScreenToWorldPoint( new Vector3( ( Screen.width ), ( Screen.height ) ) );
		m_Gem = GetComponent<Gem>();

		m_bOtherLinked = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void LinkNetworkGem( bool linked )
	{
		if ( m_bOtherLinked != linked )
		{
			GemSpawner spawner = GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>();

			// If link, scale up change sprite
			if ( linked )
			{
				spawner.SetLinkGemEffect( m_Gem );
			}
			// If not linked, scale back change sprite
			else
			{
				spawner.UnsetLinkGemEffect( m_Gem );
			}
		}
		m_bOtherLinked = linked;
	}

	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		if ( stream.isWriting )
		{
			// Sending data over
			stream.SendNext( ( transform.position.y - -m_HalfDimension.y ) / ( m_HalfDimension.y * 2.0f ) );
			stream.SendNext( ( GetComponent<Gem>().Linked ) );
		}
		else
		{
			// Receiving data
			float factorO = ( float )stream.ReceiveNext();
			float factorD = ( transform.position.y - -m_HalfDimension.y ) / ( m_HalfDimension.y * 2.0f );

			// Distance too huge (teleport)
			if ( !m_Gem.Linked && Mathf.Abs( factorO - factorD ) > DISTANCE_THRESHOLD )
			{
				Vector3 pos = transform.position;
				pos.y = ( factorO * m_HalfDimension.y * 2.0f ) + -m_HalfDimension.y;
				transform.position = pos;
			}

			bool linked = ( bool )stream.ReceiveNext();
			LinkNetworkGem( linked );
		}
	}

	void OnPhotonInstantiate( PhotonMessageInfo info )
	{
		if ( m_Gem == null )
		{
			Start();
		}

		GemSpawner spawner = GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>();
		int lane = ( int )photonView.instantiationData[0];
		int gemType = ( int )photonView.instantiationData[1];
		int seqIndex = ( int )photonView.instantiationData[2];

		m_Gem.Lane = lane;
		m_Gem.GemType = gemType;
		m_Gem.SequenceIndex = seqIndex;

		Vector3 pos = transform.position;
		pos.x = spawner.GetGemX( lane );
		pos.y = m_HalfDimension.y;
		transform.position = pos;

		if ( !NetworkManager.IsPlayerOne() )
		{
			spawner.AddNetworkGem( this );
			spawner.SetGemSpriteContainer( GetComponent<GemSpriteContainer>(), gemType );
		}
	}
}
