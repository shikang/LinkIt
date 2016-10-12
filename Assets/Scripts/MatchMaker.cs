using UnityEngine;
using System.Collections;

public class MatchMaker : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		PhotonNetwork.ConnectUsingSettings( "0.1" );
		//PhotonNetwork.logLevel = PhotonLogLevel.Full;
	}

	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnGUI()
	{
		GUILayout.Label( PhotonNetwork.connectionStateDetailed.ToString() );
	}
}
