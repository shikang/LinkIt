using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		string log = GetPlayersInfoPrefix() + "GameStart!\n";
		DebugLog( log );
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	static public bool IsConnected()
	{
		return PhotonNetwork.inRoom;
	}

	static public bool IsPlayerOne()
	{
		return PhotonNetwork.isMasterClient;
	}

	static public void Disconnect()
	{
		PhotonNetwork.Disconnect();
	}

	static public string GetPlayersInfoPrefix()
	{
		if ( IsConnected() )
		{
			return "[1] " + PhotonNetwork.player.name + " (Me)\n" +
				   "[2] " + PhotonNetwork.otherPlayers[0].name + "\n";
		}
		else
		{
			return "Not in room! Playing alone!\n";
		}
	}

	static public void DebugLog( string log )
	{
		Debug.Log( log );
		GameObject debugText = GameObject.Find( "Debug Text" );
		debugText.GetComponent<Text>().text = log;
	}
}
