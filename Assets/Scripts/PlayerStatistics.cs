using UnityEngine;
using System.Collections;

public class PlayerStatistics : MonoBehaviour
{
	public GameObject[] m_aGems;
	public int[] m_aDestroyCount;
	public int m_nLeakCount = 0;
	public int m_nScore = 0;
	public int m_nMaxCombo = 0;

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad( this );
	}
}
