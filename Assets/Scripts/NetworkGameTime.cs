using UnityEngine;
using System.Collections;

public class NetworkGameTime : Photon.MonoBehaviour
{
	public const float LAGGY_TIME_DIFF = 1.5f;

	const float TIME_DIFF_THRESHOLD = 1.0f;	//!< In seconds

	private float m_fGameTime = 0.0f;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fGameTime += Time.deltaTime;
	}

	public float GetGameTime()
	{
		return m_fGameTime;
	}

	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		if ( stream.isWriting )
		{
			// Sending data over
			stream.SendNext( m_fGameTime );
		}
		else
		{
			// Receiving data
			float gameTime = ( float )stream.ReceiveNext();

			// Time diff too huge
			if ( Mathf.Abs( gameTime - m_fGameTime ) > TIME_DIFF_THRESHOLD )
			{
				m_fGameTime = gameTime;
			}
		}
	}

	void OnPhotonInstantiate( PhotonMessageInfo info )
	{
		if ( !NetworkManager.IsPlayerOne() )
		{
			GemSpawner spawner = GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>();
			spawner.AddNetworkGameTimer( this );
		}
	}
}
