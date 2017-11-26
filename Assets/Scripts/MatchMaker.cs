using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

#if LINKIT_COOP
public class MatchMaker : Photon.PunBehaviour
{
	public GameObject m_PasswordInputField;
	public GameObject m_OnlineButton;

	private string m_sPassword = "";

	private bool m_bConnecting = false;
	private bool m_bJoinedLobby = false;
	private bool m_bJoiningRoom = false;
	private bool m_bRequestRoom = false;
	private bool m_bJoinedRoom = false;

	public UnityEvent m_ContinueWithGame;
	public UnityEvent m_StopGame;

	// Use this for initialization
	void Start ()
	{
		m_sPassword = "";
		m_bConnecting = false;
		m_bJoinedLobby = false;
		m_bJoiningRoom = false;
		m_bRequestRoom = false;
		m_bJoinedRoom = false;

		//PhotonNetwork.ConnectUsingSettings( "0.1" );
		//PhotonNetwork.logLevel = PhotonLogLevel.Full;

		PhotonNetwork.player.name = "Player2";
	}

	// Update is called once per frame
	void Update ()
	{
		if ( m_bConnecting && m_bJoinedLobby && m_bJoiningRoom && !m_bRequestRoom && !m_bJoinedRoom )
		{
			if ( m_sPassword == "" )
			{
				JoinRandomRoom();
			}
			else
			{
				JoinPrivateRoom();
			}
		}
		if ( m_bJoinedRoom )
		{
			// Timerlogic, if exceed, quit room, disconnect self
		}
	}

	void DebugLog( string log )
	{
		Debug.Log( log );
		GameObject debugText = GameObject.Find( "Debug Text" );
		debugText.GetComponent<Text>().text = log;
	}

	// @todo Comment this out to stop showing debug stuff
	void OnGUI()
	{
		GUILayout.Label( PhotonNetwork.connectionStateDetailed.ToString() );
	}

	void JoinRandomRoom()
	{
		string log = "Joining random room\n";
		DebugLog(log);

		PhotonNetwork.JoinRandomRoom();

		m_bRequestRoom = true;
	}

	void JoinPrivateRoom()
	{
		string log = "Joining " + m_sPassword + " room\n";
		DebugLog(log);

		// Join or create
		RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 2 };
		PhotonNetwork.JoinOrCreateRoom( m_sPassword, roomOptions, TypedLobby.Default );

		m_bRequestRoom = true;
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();

		m_bJoinedLobby = true;
	}

	void OnPhotonRandomJoinFailed()
	{
		string log = "Can't join random room!\n";
		DebugLog( log );

		CreateRandomRoom();
	}

	void CreateRandomRoom()
	{
		RoomOptions roomOptions = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };
		PhotonNetwork.CreateRoom( null, roomOptions, TypedLobby.Default );
	}

	public override void OnJoinedRoom()
	{
		string log = "OnJoinedRoom() : You Have joined a Room : " + PhotonNetwork.room.name + "\n";
		DebugLog( log );

		m_bJoinedRoom = true;
		int count = 1 + PhotonNetwork.otherPlayers.Length;
		PhotonNetwork.player.name = "Player" + count;

		m_OnlineButton.GetComponentInChildren<Text>().text = "Waiting...";

		ProceedToGame();
	}

	public override void OnPhotonPlayerConnected( PhotonPlayer newPlayer )
	{
		string log = "OnPhotonPlayerConnected() : " + newPlayer.name + " has joined!";
		DebugLog( log );

		ProceedToGame();
	}

	public override void OnPhotonPlayerDisconnected( PhotonPlayer otherPlayer )
	{
		if ( m_StopGame != null )
			m_StopGame.Invoke();

		m_OnlineButton.GetComponentInChildren<Text>().text = "Waiting again...";
	}

	void ProceedToGame()
	{
		if ( PhotonNetwork.otherPlayers.Length == 1 )
		{
			// Proceed
			string log = "Proceeding to game...\n" +
						 "[1] " + PhotonNetwork.player.name + " (Me)\n" +
						 "[2] " + PhotonNetwork.otherPlayers[0].name + "\n";
			DebugLog( log );

			m_OnlineButton.GetComponentInChildren<Text>().text = "Starting...";

			if ( m_ContinueWithGame != null )
				m_ContinueWithGame.Invoke();
		}
	}

	public void JoiningRoom()
	{
		if ( m_bRequestRoom || m_bJoinedRoom )
			return;

		if ( m_PasswordInputField != null )
		{
			m_sPassword = m_PasswordInputField.GetComponent<InputField>().text;
		}

		m_bJoiningRoom = true;
		m_PasswordInputField.GetComponent<InputField>().interactable = false;
		m_PasswordInputField.GetComponent<ButtonScript>().SetDisable();
		m_OnlineButton.GetComponent<Button>().interactable = false;
		m_OnlineButton.GetComponent<ButtonScript>().SetDisable();
		m_OnlineButton.GetComponentInChildren<Text>().text = "Joining...";
	}

	public void LeaveRoom()
	{
		if ( PhotonNetwork.inRoom )
			PhotonNetwork.LeaveRoom();

		m_bJoiningRoom = false;
		m_bRequestRoom = false;
		m_bJoinedRoom = false;

		m_PasswordInputField.GetComponent<InputField>().interactable = true;
		m_PasswordInputField.GetComponent<ButtonScript>().SetEnable();
		m_PasswordInputField.GetComponent<InputField>().text = "";
		m_OnlineButton.GetComponent<Button>().interactable = true;
		m_OnlineButton.GetComponent<ButtonScript>().SetEnable();
		m_OnlineButton.GetComponentInChildren<Text>().text = MainMenuManager.PLAY_WITH_STRANGER_TEXT;
	}

	public void StartConnecting()
	{
		PhotonNetwork.ConnectUsingSettings("0.1");
		m_bConnecting = true;
	}

	public void StartDisconnecting()
	{
		PhotonNetwork.Disconnect();
		m_bConnecting = false;
		m_bJoinedLobby = false;
	}

	static public bool IsPlayerOne()
	{
		return PhotonNetwork.isMasterClient;
	}
}
#else	// !LINKIT_COOP
public class MatchMaker : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		// Destroy unneeded photon view
		Destroy( GetComponent<PhotonView>() );
	}

	public void JoiningRoom()
	{
		// Empty
	}

	public void LeaveRoom()
	{
		// Empty
	}

	public void StartConnecting()
	{
		// Empty
	}

	public void StartDisconnecting()
	{
		// Empty
	}

	static public bool IsPlayerOne()
	{
		return true;
	}
}
#endif	// LINKIT_COOP
