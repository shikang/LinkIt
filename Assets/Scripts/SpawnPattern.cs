using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPattern : MonoBehaviour
{
	List< List< int > > m_lSpawnSequenceList;
	List< int > m_lWeightList;

	public enum DifficultyLevel
	{
		Easy,
		Intermediate,
		Advanced,
		Expert,
		Hardcore,
		Godly,
		
		Total
	}

	List<int> m_lDifficultyMarker;
	List<int> m_lTotalWeight;

	// Use this for initialization
	void Start ()
	{
		int weight = 0;
		int totalWeight = 0;
		m_lSpawnSequenceList = new List< List<int> >();
		m_lWeightList = new List<int>();
		m_lDifficultyMarker = new List<int>( ( int )DifficultyLevel.Total );
		m_lTotalWeight = new List<int>( ( int )DifficultyLevel.Total );

		// ---- 1 colour ----
		// SPAWN_0_0_0
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 0 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_0_0,			//!< 0, 0, 0, 0
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 0, 0 } );
		weight = 100;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_0_0_0,			//!< 0, 0, 0, 0, 0
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 0, 0, 0 } );
		weight = 5;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_0_0_0_0,			//!< 0, 0, 0, 0, 0, 0
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 0, 0, 0, 0 } );
		weight = 3;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_7,			//!< 0, 0, 0, 0, 0, 0, 0
		m_lSpawnSequenceList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0 });
		weight = 2;
		m_lWeightList.Add( weight );
		totalWeight += weight;

		// DifficultyLevel.Easy
		m_lDifficultyMarker.Add( m_lSpawnSequenceList.Count );
		m_lTotalWeight.Add( totalWeight );

		// ---- 2 colours (3) ----
		// SPAWN_0_0_1_0_1_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 1, 0, 1, 1 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_1_1_0_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 1, 1, 0, 1 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_1_0_0_1_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 0, 0, 1, 1 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_1_1_0_0_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 1, 0, 0, 1 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_1_1_1_0_0
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 1, 1, 0, 0 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_1_1_1_0
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 1, 1, 1, 0 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;

		// DifficultyLevel.Intermediate
		m_lDifficultyMarker.Add( m_lSpawnSequenceList.Count );
		m_lTotalWeight.Add( totalWeight );

		// ---- 2 colours (4) ----
		// SPAWN_0_0_1_0_0_1_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 1, 0, 0, 1, 1 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_0_1_0_1_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 0, 1, 0, 1, 1 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_1_1_0_0_1_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 1, 0, 0, 1, 1 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_1_1_0_0_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 1, 1, 0, 0, 1 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_1_0_0_1_1_1
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 0, 0, 1, 1, 1 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;

		// DifficultyLevel.Advanced
		m_lDifficultyMarker.Add( m_lSpawnSequenceList.Count );
		m_lTotalWeight.Add( totalWeight );

		// ---- 2 colours (4) ----
		// SPAWN_0_0_1_1_1_0_0
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 1, 1, 1, 0, 0 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_1_1_1_0_0_0
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 1, 1, 0, 0, 0 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;

		// ---- 3 colours (3) ----
		// SPAWN_0_1_2_0_0_1_1_2_2
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 2, 0, 0, 1, 1, 2, 2 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_1_0_2_1_1_2_2
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 1, 0, 2, 1, 1, 2, 2 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_0_1_2_0_1_1_2_2
		m_lSpawnSequenceList.Add( new List<int> { 0, 0, 1, 2, 0, 1, 1, 2, 2 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;
		// SPAWN_0_1_0_1_2_0_1_2_2
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 0, 1, 2, 0, 1, 2, 2 } );
		weight = 200;
		m_lWeightList.Add( weight );
		totalWeight += weight;

		// DifficultyLevel.Expert
		m_lDifficultyMarker.Add( m_lSpawnSequenceList.Count );
		m_lTotalWeight.Add( totalWeight );

		// ---- 3 colours (3) ----
		// SPAWN_0_1_2_0_1_0_1_2_2
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 2, 0, 1, 0, 1, 2, 2 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;

		// DifficultyLevel.Hardcore
		m_lDifficultyMarker.Add( m_lSpawnSequenceList.Count );
		m_lTotalWeight.Add( totalWeight );

		// ---- 3 colours (3) ----
		// SPAWN_0_1_2_0_1_2_0_1_2
		m_lSpawnSequenceList.Add( new List<int> { 0, 1, 2, 0, 1, 2, 0, 1, 2 } );
		weight = 1000;
		m_lWeightList.Add( weight );
		totalWeight += weight;

		// DifficultyLevel.Godly
		m_lDifficultyMarker.Add( m_lSpawnSequenceList.Count );
		m_lTotalWeight.Add( totalWeight );
	}

	public List<int> GetSequence ( int nLevel, ref int index )
	{
		int difficulty = GetDifficulty( nLevel );
		int size = m_lDifficultyMarker[difficulty];
		int totalWeight = m_lTotalWeight[difficulty];

		int prob = Random.Range( 0, totalWeight );
		int i = 0;
		for ( i = 0; i < size; ++i )
		{
			if ( prob <= m_lWeightList[i] )
				break;
			else
				prob -= m_lWeightList[i];
		}

		// Log
		/*
		string log = "[ ";
		for ( int j = 0; j < m_lSpawnSequenceList[i].Count; ++j )
		{
			log += m_lSpawnSequenceList[i][j] + " ";
		}
		log += "]";
		Debug.Log( "Get sequence at "+ i + ": " + log );
		*/

		index = i;
		return m_lSpawnSequenceList[i];
	}

	int GetDifficulty ( int nLevel )
	{
		return nLevel < 3 * ( (int)DifficultyLevel.Total - 1 ) ? nLevel / 3 : (int)DifficultyLevel.Total - 1;
	}

	public int GetSpawnSeqencesNum()
	{
		return m_lSpawnSequenceList.Count;
	}
}
