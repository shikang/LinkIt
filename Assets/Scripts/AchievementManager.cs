using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ACHIEVEMENT LIST
 * =================
 *
 * IN TOTAL
 * Played X games
 * Earned X coins in total
 * Scored X points in total
 * Link X of Y-colored gems in total
 *
 * IN ONE GAME
 * Reached X combo
 * Earned X coins in one game
 * Earned X points in one game
 *
 * Recover To perfect health from eed health
 * Linked X gems in one chain
 * 
 * Unlocked Booster: Played X games
 * Unlocked Booster: Earned X points
 * Unlocked Booster: Shared with X FB friends
 *
 * NOT IMPLEMENTED
 * Use X powerups in total
 * Have skills active for X seconds
 * Have 4 skills active together
 * Have X FB friends in game
 */

public struct ACHIEVEMENTSET
{
	public string title;
	public string desc;
	public int count;
#if UNITY_ANDROID
	public string achievementID;
#endif
}

public class AchievementManager : MonoBehaviour
{
	// Display
	List<ACHIEVEMENTSET> m_lAchivements = new List<ACHIEVEMENTSET>();
	//public GameObject m_gDisplayCanvas;
	//public GameObject m_gDisplayTitle;
	//public GameObject m_gDisplayDesc;
	public GameObject m_gPrefab;


	const float CANVAS_ALPHASPEED = 3.0f;
	float m_fdisplayTimer;
	bool isFading = false;

	public bool healthisRed;
	public int maxGemsPerChain;

	// Achievement Arrays
	public int m_noofAchTypes = 12;
	public ACHIEVEMENTSET [] m_array_TotalGamesPlayed;
	public ACHIEVEMENTSET [] m_array_TotalCoinsEarned;
	public ACHIEVEMENTSET [] m_array_TotalScoreEarned;

	public ACHIEVEMENTSET [] m_array_TotalLinkedGems_R;
	public ACHIEVEMENTSET [] m_array_TotalLinkedGems_G;
	public ACHIEVEMENTSET [] m_array_TotalLinkedGems_B;
	public ACHIEVEMENTSET [] m_array_TotalLinkedGems_Y;

	public ACHIEVEMENTSET [] m_array_MaxCombo;
	public ACHIEVEMENTSET [] m_array_PerGameCoinsEarned;
	public ACHIEVEMENTSET [] m_array_PerGameScoreEarned;

	public ACHIEVEMENTSET m_RecoverToPerfectFromRed;
	public ACHIEVEMENTSET [] m_array_LinkGemsInOneChain;

	public ACHIEVEMENTSET m_BoosterPlayedGames;
	public ACHIEVEMENTSET m_BoosterEarnedPoints;
	public ACHIEVEMENTSET m_BoosterSharedFB;

	// Singleton pattern
	static AchievementManager instance;
	public static AchievementManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if (instance != null)
		{
            GetDisplayCanas().GetComponent<CanvasGroup>().alpha = 0.0f;

            Destroy (this.gameObject);
			return;
			//throw new System.Exception("You have more than 1 AchievementManager in the scene.");
		}
		
		LoadAchievements();
		
		instance = this;
		// Initialize the static class variables
		DontDestroyOnLoad(gameObject);
	}

    static GameObject GetDisplayCanas()
    {
        return GameObject.FindGameObjectWithTag( "Achievement Canvas" );
    }

    static GameObject GetDisplayTitle()
    {
        return GetDisplayCanas().transform.GetChild( 0 ).GetChild( 1 ).gameObject;
    }

    static GameObject GetDisplayDesc()
    {
        return GetDisplayCanas().transform.GetChild( 0 ).GetChild( 2 ).gameObject;
    }

    void Start()
	{
		// Called by someone else
		//SaveLoad.Load();
		m_fdisplayTimer = 0.0f;
        GetDisplayCanas().GetComponent<CanvasGroup>().alpha = 0.0f;
	}

	public void ResetVars ()
	{
		healthisRed = false;
		maxGemsPerChain = 0;
		if(m_lAchivements.Count <= 0)
            GetDisplayCanas().GetComponent<CanvasGroup>().alpha = 0.0f;
	}

	void Update ()
	{
        if (GetDisplayCanas() == null)
            return;

		if(m_lAchivements.Count > 0)
		{
            //GetDisplayCanas().SetActive(true);
            GetDisplayTitle().GetComponent<Text>().text = m_lAchivements[0].title;
            GetDisplayDesc().GetComponent<Text>().text = m_lAchivements[0].desc;

			if(!isFading)
			{
                GetDisplayCanas().GetComponent<CanvasGroup>().alpha += Time.deltaTime * CANVAS_ALPHASPEED;
				m_fdisplayTimer += Time.deltaTime;
				if(m_fdisplayTimer >= 2.0f)
				{
					m_fdisplayTimer = 0.0f;
					isFading = true;
				}
			}
			else
			{
                GetDisplayCanas().GetComponent<CanvasGroup>().alpha -= Time.deltaTime * CANVAS_ALPHASPEED;
				if(GetDisplayCanas().GetComponent<CanvasGroup>().alpha <= 0.0f)
				{
					m_lAchivements.RemoveAt(0);
					isFading = false;
				}
			}
		}
		else
		{
            //GetDisplayCanas().SetActive(false);
            GetDisplayCanas().GetComponent<CanvasGroup>().alpha = 0.0f;
		}
	}

	public void PrintObtainedText(ACHIEVEMENTSET ach)
	{
		m_lAchivements.Add(ach);
		//Debug.Log("ACHIEVEMENT UNLOCKED!\n" + ach.title + ": " + ach.desc);
	}

	void CheckCounters(ACHIEVEMENTSET[] arr, int currCount, ref int nextIndex)
	{
		while(currCount >= arr[nextIndex].count)
		{
			PrintObtainedText(arr[nextIndex]);

#if UNITY_ANDROID
            // This will be multiple times as long as next index did not change
            if ( arr[nextIndex].achievementID != "" )
		    {
			    GooglePlayService.ProgressAcheivement( arr[nextIndex].achievementID, 100.0f );
		    }
#endif

            if(arr.Length > nextIndex + 1)
				++nextIndex;
			else
				break;
		}

#if UNITY_ANDROID
		int currentIndex = Math.Max( 0, nextIndex - 1 ); 
		// This will be multiple times as long as next index did not change
		//if ( arr[currentIndex].achievementID != "" )
		//{
		//	GooglePlayService.ProgressAcheivement( arr[currentIndex].achievementID, 100.0f );
		//}

		while ( arr.Length > currentIndex + 1 && arr[++currentIndex].achievementID == "" );
		if ( arr[currentIndex].achievementID != "" )
		{
			GooglePlayService.ProgressAcheivement( arr[currentIndex].achievementID, (float)currCount / arr[currentIndex].count );
		}
#endif
		//SaveLoad.Save();
		SaveDataLoader.SaveGame();
	}

	// PLAYED X GAMES
	public void AddGamesPlayed()
	{
		GameData.Instance.m_curr_TotalGamesPlayed += 1;
		CheckCounters(m_array_TotalGamesPlayed, GameData.Instance.m_curr_TotalGamesPlayed, ref GameData.Instance.m_next_TotalGamesPlayed);
	}

	// EARNED X COINS IN TOTAL
	public void AddCoinsEarned(int amt)
	{
		GameData.Instance.m_curr_TotalCoinsEarned += amt;
		CheckCounters(m_array_TotalCoinsEarned, GameData.Instance.m_curr_TotalCoinsEarned, ref GameData.Instance.m_next_TotalCoinsEarned);
	}

	// SCORED X POINTS IN TOTAL
	public void AddScoreEarned(int amt)
	{
		GameData.Instance.m_curr_TotalScoreEarned += amt;
		CheckCounters(m_array_TotalScoreEarned, GameData.Instance.m_curr_TotalScoreEarned, ref GameData.Instance.m_next_TotalScoreEarned);
	}

	// LINKED X COLOR GEMS IN TOTAL
	public void AddLinkedGems(int type, int amt)
	{
		if(type == 1)	// Red
		{
			GameData.Instance.m_curr_TotalLinkedGems_R += amt;
			CheckCounters(m_array_TotalLinkedGems_R, GameData.Instance.m_curr_TotalLinkedGems_R, ref GameData.Instance.m_next_TotalLinkedGems_R);
		}
		else if(type == 2)	// Blue
		{
			GameData.Instance.m_curr_TotalLinkedGems_B += amt;
			CheckCounters(m_array_TotalLinkedGems_B, GameData.Instance.m_curr_TotalLinkedGems_B, ref GameData.Instance.m_next_TotalLinkedGems_B);
		}
		else if(type == 3)	// Green
		{
			GameData.Instance.m_curr_TotalLinkedGems_G += amt;
			CheckCounters(m_array_TotalLinkedGems_G, GameData.Instance.m_curr_TotalLinkedGems_G, ref GameData.Instance.m_next_TotalLinkedGems_G);
		}
		else if(type == 4)	// Yellow
		{
			GameData.Instance.m_curr_TotalLinkedGems_Y += amt;
			CheckCounters(m_array_TotalLinkedGems_Y, GameData.Instance.m_curr_TotalLinkedGems_Y, ref GameData.Instance.m_next_TotalLinkedGems_Y);
		}
	}

	// REACHED X COMBO
	public void AddCombo(int amt)
	{
		CheckCounters(m_array_MaxCombo, amt, ref GameData.Instance.m_next_MaxCombo);
	}

	// EARN X COINS IN ONE GAME
	public void AddCoinsEarned_PerGame(int amt)
	{
		GameData.Instance.m_curr_PerGameCoinsEarned = amt;
		CheckCounters(m_array_PerGameCoinsEarned, GameData.Instance.m_curr_PerGameCoinsEarned, ref GameData.Instance.m_next_PerGameCoinsEarned);
	}

	// EARN X POINTS IN ONE GAME
	public void AddScoreEarned_PerGame(int amt)
	{
		GameData.Instance.m_curr_PerGameScoreEarned = amt;
		CheckCounters(m_array_PerGameScoreEarned, GameData.Instance.m_curr_PerGameScoreEarned, ref GameData.Instance.m_next_PerGameScoreEarned);
	}

	// RECOVER TO PERFECT HEALTH FROM RED HEALTH
	public void RecoverToPerfectFromRed()
	{
		GameData.Instance.m_RecoverToPerfectFromRed = 1;
		PrintObtainedText(m_RecoverToPerfectFromRed);
		//SaveLoad.Save();
		SaveDataLoader.SaveGame();
	}

	public bool IsRecoverToPerfectFromRedAchieved()
	{
		if(GameData.Instance.m_RecoverToPerfectFromRed == 1)
			return true;
		return false;
	}

	// LINK_X_GEMS_IN_ONE_CHAIN
	public void LinkGemsInOneChain()
	{
		GameData.Instance.m_curr_LinkGemsInOneChain = maxGemsPerChain;
		CheckCounters(m_array_LinkGemsInOneChain, GameData.Instance.m_curr_LinkGemsInOneChain, ref GameData.Instance.m_next_LinkGemsInOneChain);
	}

	// BOOSTER: GAME PLAYED
	public void BoosterGamesPlayed()
	{
		if (GameData.Instance.m_bUnlock_Games)
			return;
		
		GameData.Instance.m_uUnlock_GamesCount--;
		if(GameData.Instance.m_uUnlock_GamesCount <= 0)
		{
			GameData.Instance.m_bUnlock_Games = true;
			PrintObtainedText(m_BoosterPlayedGames);
			//SaveLoad.Save();
			SaveDataLoader.SaveGame();
		}
	}

	// BOOSTER: POINTS EARNED
	public void BoosterPointsEarned(int score)
	{
		if (GameData.Instance.m_bUnlock_EarnPoints)
			return;
		
		GameData.Instance.m_uUnlock_EarnPointsCount -= score;
		if(GameData.Instance.m_uUnlock_EarnPointsCount <= 0)
		{
			GameData.Instance.m_bUnlock_EarnPoints = true;
			PrintObtainedText(m_BoosterEarnedPoints);
			//SaveLoad.Save();
			SaveDataLoader.SaveGame();
		}
	}

	// BOOSTER: SHARED ON FB
	public void BoosterSharedFB(int amt)
	{
		if (GameData.Instance.m_bUnlock_Share_FB)
			return;

		GameData.Instance.m_uUnlock_Share_FBCount -= amt;
		if(GameData.Instance.m_uUnlock_Share_FBCount <= 0)
		{
			GameData.Instance.m_bUnlock_Share_FB = true;
			PrintObtainedText(m_BoosterSharedFB);
			//SaveLoad.Save();
			SaveDataLoader.SaveGame();
		}
	}

	public void UpdateMaxLinkGemsInOneChain(int chain)
	{
		if(chain > maxGemsPerChain)
			maxGemsPerChain = chain;
	}

	void LoadAchievements()
	{
		m_array_TotalGamesPlayed = new ACHIEVEMENTSET[14];
		m_array_TotalGamesPlayed[0].count = 1;		m_array_TotalGamesPlayed[0].title = "First Steps";			
		m_array_TotalGamesPlayed[1].count = 5;		m_array_TotalGamesPlayed[1].title = "Beginner";				
		m_array_TotalGamesPlayed[2].count = 10;		m_array_TotalGamesPlayed[2].title = "Novice";				
		m_array_TotalGamesPlayed[3].count = 20;		m_array_TotalGamesPlayed[3].title = "Freshman";				
		m_array_TotalGamesPlayed[4].count = 30;		m_array_TotalGamesPlayed[4].title = "Sophomore";			
		m_array_TotalGamesPlayed[5].count = 40;		m_array_TotalGamesPlayed[5].title = "Junior";				
		m_array_TotalGamesPlayed[6].count = 50;		m_array_TotalGamesPlayed[6].title = "Senior";				
		m_array_TotalGamesPlayed[7].count = 70;		m_array_TotalGamesPlayed[7].title = "Graduated";			
		m_array_TotalGamesPlayed[8].count = 100;	m_array_TotalGamesPlayed[8].title = "Gem Destroyer";		
		m_array_TotalGamesPlayed[9].count = 120;	m_array_TotalGamesPlayed[9].title = "Addicted";				
		m_array_TotalGamesPlayed[10].count = 150;	m_array_TotalGamesPlayed[10].title = "Needs Help";			
		m_array_TotalGamesPlayed[11].count = 200;	m_array_TotalGamesPlayed[11].title = "Pro";					
		m_array_TotalGamesPlayed[12].count = 250;	m_array_TotalGamesPlayed[12].title = "Seasoned Pro";		
		m_array_TotalGamesPlayed[13].count = 300;	m_array_TotalGamesPlayed[13].title = "Link Master";

#if UNITY_ANDROID
		m_array_TotalGamesPlayed[0].achievementID = GPGSIds.achievement_play_1_game;
		m_array_TotalGamesPlayed[1].achievementID = GPGSIds.achievement_play_5_games;
		m_array_TotalGamesPlayed[2].achievementID = "";
		m_array_TotalGamesPlayed[3].achievementID = "";
		m_array_TotalGamesPlayed[4].achievementID = GPGSIds.achievement_play_30_games;
		m_array_TotalGamesPlayed[5].achievementID = "";
		m_array_TotalGamesPlayed[6].achievementID = GPGSIds.achievement_play_50_games;
		m_array_TotalGamesPlayed[7].achievementID = "";
		m_array_TotalGamesPlayed[8].achievementID = GPGSIds.achievement_play_100_games;
		m_array_TotalGamesPlayed[9].achievementID = "";
		m_array_TotalGamesPlayed[10].achievementID = "";
		m_array_TotalGamesPlayed[11].achievementID = "";
		m_array_TotalGamesPlayed[12].achievementID = "";
		m_array_TotalGamesPlayed[13].achievementID = GPGSIds.achievement_play_300_games;
#endif

		for (int i = 0; i < m_array_TotalGamesPlayed.Length; ++i)
		{
			m_array_TotalGamesPlayed[i].desc = "Played " + m_array_TotalGamesPlayed[i].count + " Games";
		}

		m_array_TotalCoinsEarned = new ACHIEVEMENTSET[10];
		m_array_TotalCoinsEarned[0].count = 50;		m_array_TotalCoinsEarned[0].title = "Pocket Money";			
		m_array_TotalCoinsEarned[1].count = 200;	m_array_TotalCoinsEarned[1].title = "Part Timer";			
		m_array_TotalCoinsEarned[2].count = 500;	m_array_TotalCoinsEarned[2].title = "Serious Work";			
		m_array_TotalCoinsEarned[3].count = 1000;	m_array_TotalCoinsEarned[3].title = "Shopping Spree";		
		m_array_TotalCoinsEarned[4].count = 5000;	m_array_TotalCoinsEarned[4].title = "Unstoppable";			
		m_array_TotalCoinsEarned[5].count = 10000;	m_array_TotalCoinsEarned[5].title = "Powerup Loader";		
		m_array_TotalCoinsEarned[6].count = 50000;	m_array_TotalCoinsEarned[6].title = "Leaderboard Chaser";	
		m_array_TotalCoinsEarned[7].count = 100000;	m_array_TotalCoinsEarned[7].title = "Money Maker";			
		m_array_TotalCoinsEarned[8].count = 500000;	m_array_TotalCoinsEarned[8].title = "Jackpot";
        m_array_TotalCoinsEarned[9].count = 1000000;m_array_TotalCoinsEarned[9].title = "Millionaire";

#if UNITY_ANDROID
		m_array_TotalCoinsEarned[0].achievementID = "";
		m_array_TotalCoinsEarned[1].achievementID = "";
		m_array_TotalCoinsEarned[2].achievementID = "";
		m_array_TotalCoinsEarned[3].achievementID = "";
		m_array_TotalCoinsEarned[4].achievementID = "";
		m_array_TotalCoinsEarned[5].achievementID = "";
		m_array_TotalCoinsEarned[6].achievementID = "";
		m_array_TotalCoinsEarned[7].achievementID = "";
		m_array_TotalCoinsEarned[8].achievementID = "";
        m_array_TotalCoinsEarned[9].achievementID = "";
#endif

        for (int i = 0; i < m_array_TotalCoinsEarned.Length; ++i)
		{
			m_array_TotalCoinsEarned[i].desc = "Earned " + m_array_TotalCoinsEarned[i].count + " Coins in Total";
		}

		m_array_TotalScoreEarned = new ACHIEVEMENTSET[10];
		m_array_TotalScoreEarned[0].count = 100000;		m_array_TotalScoreEarned[0].title = "Peanuts";					
		m_array_TotalScoreEarned[1].count = 200000;		m_array_TotalScoreEarned[1].title = "More Peanuts";				
		m_array_TotalScoreEarned[2].count = 500000;		m_array_TotalScoreEarned[2].title = "Dedication";				
		m_array_TotalScoreEarned[3].count = 1000000;	m_array_TotalScoreEarned[3].title = "Seasoned";					
		m_array_TotalScoreEarned[4].count = 2000000;	m_array_TotalScoreEarned[4].title = "Achiever";					
		m_array_TotalScoreEarned[5].count = 5000000;	m_array_TotalScoreEarned[5].title = "Need.More.Points";			
		m_array_TotalScoreEarned[6].count = 8000000;	m_array_TotalScoreEarned[6].title = "Multiplier";				
		m_array_TotalScoreEarned[7].count = 10000000;	m_array_TotalScoreEarned[7].title = "Million Dollar Question";	
		m_array_TotalScoreEarned[8].count = 15000000;	m_array_TotalScoreEarned[8].title = "Massive Numbers";			
		m_array_TotalScoreEarned[9].count = 20000000;	m_array_TotalScoreEarned[9].title = "Top Scorer";

#if UNITY_ANDROID
		m_array_TotalScoreEarned[0].achievementID = GPGSIds.achievement_get_5000_points;
		m_array_TotalScoreEarned[1].achievementID = GPGSIds.achievement_get_10000_points;
		m_array_TotalScoreEarned[2].achievementID = "";
		m_array_TotalScoreEarned[3].achievementID = "";
		m_array_TotalScoreEarned[4].achievementID = "";
		m_array_TotalScoreEarned[5].achievementID = "";
		m_array_TotalScoreEarned[6].achievementID = "";
		m_array_TotalScoreEarned[7].achievementID = "";
		m_array_TotalScoreEarned[8].achievementID = "";
		m_array_TotalScoreEarned[9].achievementID = "";
#endif

		for (int i = 0; i < m_array_TotalScoreEarned.Length; ++i)
		{
			m_array_TotalScoreEarned[i].desc = "Scored " + m_array_TotalScoreEarned[i].count + " Points in Total";
		}

		int linkedAch = 7;
		m_array_TotalLinkedGems_R = new ACHIEVEMENTSET[linkedAch];
		m_array_TotalLinkedGems_B = new ACHIEVEMENTSET[linkedAch];
		m_array_TotalLinkedGems_G = new ACHIEVEMENTSET[linkedAch];
		m_array_TotalLinkedGems_Y = new ACHIEVEMENTSET[linkedAch];

		m_array_TotalLinkedGems_R[0].count = m_array_TotalLinkedGems_B[0].count = m_array_TotalLinkedGems_G[0].count = m_array_TotalLinkedGems_Y[0].count = 50;
		m_array_TotalLinkedGems_R[1].count = m_array_TotalLinkedGems_B[1].count = m_array_TotalLinkedGems_G[1].count = m_array_TotalLinkedGems_Y[1].count = 200;
		m_array_TotalLinkedGems_R[2].count = m_array_TotalLinkedGems_B[2].count = m_array_TotalLinkedGems_G[2].count = m_array_TotalLinkedGems_Y[2].count = 500;
		m_array_TotalLinkedGems_R[3].count = m_array_TotalLinkedGems_B[3].count = m_array_TotalLinkedGems_G[3].count = m_array_TotalLinkedGems_Y[3].count = 1000;
		m_array_TotalLinkedGems_R[4].count = m_array_TotalLinkedGems_B[4].count = m_array_TotalLinkedGems_G[4].count = m_array_TotalLinkedGems_Y[4].count = 2000;
		m_array_TotalLinkedGems_R[5].count = m_array_TotalLinkedGems_B[5].count = m_array_TotalLinkedGems_G[5].count = m_array_TotalLinkedGems_Y[5].count = 5000;
		m_array_TotalLinkedGems_R[6].count = m_array_TotalLinkedGems_B[6].count = m_array_TotalLinkedGems_G[6].count = m_array_TotalLinkedGems_Y[6].count = 10000;

		m_array_TotalLinkedGems_R[0].title = "Fire Learner";			
		m_array_TotalLinkedGems_R[1].title = "Fire Wielder";			
		m_array_TotalLinkedGems_R[2].title = "Fire Mage";				
		m_array_TotalLinkedGems_R[3].title = "Fire Bringer";			
		m_array_TotalLinkedGems_R[4].title = "Fire Master";				
		m_array_TotalLinkedGems_R[5].title = "Fire Grand Master";		
		m_array_TotalLinkedGems_R[6].title = "Implosion"; 				

		m_array_TotalLinkedGems_B[0].title = "Water Learner";			
		m_array_TotalLinkedGems_B[1].title = "Water Wielder";			
		m_array_TotalLinkedGems_B[2].title = "Water Mage";				
		m_array_TotalLinkedGems_B[3].title = "Water Bringer";			
		m_array_TotalLinkedGems_B[4].title = "Water Master";			
		m_array_TotalLinkedGems_B[5].title = "Water Grand Master";		
		m_array_TotalLinkedGems_B[6].title = "Tsunami"; 				

		m_array_TotalLinkedGems_G[0].title = "Earth Learner";			
		m_array_TotalLinkedGems_G[1].title = "Earth Wielder";			
		m_array_TotalLinkedGems_G[2].title = "Earth Mage";				
		m_array_TotalLinkedGems_G[3].title = "Earth Bringer";			
		m_array_TotalLinkedGems_G[4].title = "Earth Master";			
		m_array_TotalLinkedGems_G[5].title = "Earth Grand Master";		
		m_array_TotalLinkedGems_G[6].title = "Earthquake";				

		m_array_TotalLinkedGems_Y[0].title = "Wind Learner";			
		m_array_TotalLinkedGems_Y[1].title = "Wind Wielder";			
		m_array_TotalLinkedGems_Y[2].title = "Wind Mage";				
		m_array_TotalLinkedGems_Y[3].title = "Wind Bringer";			
		m_array_TotalLinkedGems_Y[4].title = "Wind Master";				
		m_array_TotalLinkedGems_Y[5].title = "Wind Grand Master";		
		m_array_TotalLinkedGems_Y[6].title = "Hurricane";

#if UNITY_ANDROID
		m_array_TotalLinkedGems_R[0].achievementID = "";
		m_array_TotalLinkedGems_R[1].achievementID = "";
		m_array_TotalLinkedGems_R[2].achievementID = "";
		m_array_TotalLinkedGems_R[3].achievementID = "";
		m_array_TotalLinkedGems_R[4].achievementID = "";
		m_array_TotalLinkedGems_R[5].achievementID = "";
		m_array_TotalLinkedGems_R[6].achievementID = "";
									
		m_array_TotalLinkedGems_B[0].achievementID = "";
		m_array_TotalLinkedGems_B[1].achievementID = "";
		m_array_TotalLinkedGems_B[2].achievementID = "";
		m_array_TotalLinkedGems_B[3].achievementID = "";
		m_array_TotalLinkedGems_B[4].achievementID = "";
		m_array_TotalLinkedGems_B[5].achievementID = "";
		m_array_TotalLinkedGems_B[6].achievementID = "";

		m_array_TotalLinkedGems_G[0].achievementID = "";
		m_array_TotalLinkedGems_G[1].achievementID = "";
		m_array_TotalLinkedGems_G[2].achievementID = "";
		m_array_TotalLinkedGems_G[3].achievementID = "";
		m_array_TotalLinkedGems_G[4].achievementID = "";
		m_array_TotalLinkedGems_G[5].achievementID = "";
		m_array_TotalLinkedGems_G[6].achievementID = "";

		m_array_TotalLinkedGems_Y[0].achievementID = "";
		m_array_TotalLinkedGems_Y[1].achievementID = "";
		m_array_TotalLinkedGems_Y[2].achievementID = "";
		m_array_TotalLinkedGems_Y[3].achievementID = "";
		m_array_TotalLinkedGems_Y[4].achievementID = "";
		m_array_TotalLinkedGems_Y[5].achievementID = "";
		m_array_TotalLinkedGems_Y[6].achievementID = "";
#endif

		for (int i = 0; i < m_array_TotalLinkedGems_R.Length; ++i)
		{
			m_array_TotalLinkedGems_R[i].desc = "Linked " + m_array_TotalLinkedGems_R[i].count + " Red Gems in Total";
			m_array_TotalLinkedGems_B[i].desc = "Linked " + m_array_TotalLinkedGems_B[i].count + " Blue Gems in Total";
			m_array_TotalLinkedGems_G[i].desc = "Linked " + m_array_TotalLinkedGems_G[i].count + " Green Gems in Total";
			m_array_TotalLinkedGems_Y[i].desc = "Linked " + m_array_TotalLinkedGems_Y[i].count + " Yellow Gems in Total";
		}

		m_array_MaxCombo = new ACHIEVEMENTSET[11];
		m_array_MaxCombo[0].count = 50;		m_array_MaxCombo[0].title = "Drawing Circles";
		m_array_MaxCombo[1].count = 75;		m_array_MaxCombo[1].title = "Webbing Along";
		m_array_MaxCombo[2].count = 100;	m_array_MaxCombo[2].title = "C-c-c-combos";
		m_array_MaxCombo[3].count = 125;	m_array_MaxCombo[3].title = "Going On and On";
		m_array_MaxCombo[4].count = 150;	m_array_MaxCombo[4].title = "All Above The Line";
		m_array_MaxCombo[5].count = 180;	m_array_MaxCombo[5].title = "Combo Maker";
		m_array_MaxCombo[6].count = 200;	m_array_MaxCombo[6].title = "Artistic";
		m_array_MaxCombo[7].count = 250;	m_array_MaxCombo[7].title = "Fast Fingers";
		m_array_MaxCombo[8].count = 300;	m_array_MaxCombo[8].title = "Flow Master";
        m_array_MaxCombo[9].count = 500;	m_array_MaxCombo[9].title = "Sensei";
        m_array_MaxCombo[10].count = 1000;	m_array_MaxCombo[10].title = "Demi God!";

#if UNITY_ANDROID
		m_array_MaxCombo[0].achievementID = "";
		m_array_MaxCombo[1].achievementID = "";
		m_array_MaxCombo[2].achievementID = "";
		m_array_MaxCombo[3].achievementID = "";
		m_array_MaxCombo[4].achievementID = "";
		m_array_MaxCombo[5].achievementID = "";
		m_array_MaxCombo[6].achievementID = "";
		m_array_MaxCombo[7].achievementID = "";
		m_array_MaxCombo[8].achievementID = "";
        m_array_MaxCombo[9].achievementID = "";
        m_array_MaxCombo[10].achievementID = "";
#endif

        for (int i = 0; i < m_array_MaxCombo.Length; ++i)
		{
			m_array_MaxCombo[i].desc = "Reached " + m_array_MaxCombo[i].count + " Combo";
		}

		m_array_PerGameCoinsEarned = new ACHIEVEMENTSET[5];
		m_array_PerGameCoinsEarned[0].count = 100;	m_array_PerGameCoinsEarned[0].title = "Coin Picker";
		m_array_PerGameCoinsEarned[1].count = 250;	m_array_PerGameCoinsEarned[1].title = "Cash Cow";
		m_array_PerGameCoinsEarned[2].count = 500;	m_array_PerGameCoinsEarned[2].title = "Money Face";
		m_array_PerGameCoinsEarned[3].count = 1000;	m_array_PerGameCoinsEarned[3].title = "Hauler";
		m_array_PerGameCoinsEarned[4].count = 2000;	m_array_PerGameCoinsEarned[4].title = "Money Machine";

#if UNITY_ANDROID
		m_array_PerGameCoinsEarned[0].achievementID = "";
		m_array_PerGameCoinsEarned[1].achievementID = "";
		m_array_PerGameCoinsEarned[2].achievementID = "";
		m_array_PerGameCoinsEarned[3].achievementID = "";
		m_array_PerGameCoinsEarned[4].achievementID = "";
#endif

		for (int i = 0; i < m_array_PerGameCoinsEarned.Length; ++i)
		{
			m_array_PerGameCoinsEarned[i].desc = "Earned " + m_array_PerGameCoinsEarned[i].count + " Coins in 1 Game";
		}

		m_array_PerGameScoreEarned = new ACHIEVEMENTSET[6];
		m_array_PerGameScoreEarned[0].count = 20000;		m_array_PerGameScoreEarned[0].title = "Warming Up";
		m_array_PerGameScoreEarned[1].count = 50000;	m_array_PerGameScoreEarned[1].title = "Competitive";
		m_array_PerGameScoreEarned[2].count = 100000;	m_array_PerGameScoreEarned[2].title = "Top Dog";
		m_array_PerGameScoreEarned[3].count = 200000;	m_array_PerGameScoreEarned[3].title = "Ace";
		m_array_PerGameScoreEarned[4].count = 500000;	m_array_PerGameScoreEarned[4].title = "MVP";
        m_array_PerGameScoreEarned[5].count = 999999;	m_array_PerGameScoreEarned[5].title = "Grand Master";

#if UNITY_ANDROID
		m_array_PerGameScoreEarned[0].achievementID = "";
		m_array_PerGameScoreEarned[1].achievementID = "";
		m_array_PerGameScoreEarned[2].achievementID = "";
		m_array_PerGameScoreEarned[3].achievementID = "";
		m_array_PerGameScoreEarned[4].achievementID = "";
        m_array_PerGameScoreEarned[5].achievementID = "";
#endif

        for (int i = 0; i < m_array_PerGameScoreEarned.Length; ++i)
		{
			m_array_PerGameScoreEarned[i].desc = "Earned " + m_array_PerGameScoreEarned[i].count + " Points in 1 Game";
		}

		m_RecoverToPerfectFromRed.title = "Second Chance";
		m_RecoverToPerfectFromRed.desc = "Fully Recover from Critcal Health";
		m_RecoverToPerfectFromRed.count = 1;
#if UNITY_ANDROID
		m_RecoverToPerfectFromRed.achievementID = "";
#endif

		m_array_LinkGemsInOneChain = new ACHIEVEMENTSET[5];
		m_array_LinkGemsInOneChain[0].count = 5;		m_array_LinkGemsInOneChain[0].title = "Caterpillar";
		m_array_LinkGemsInOneChain[1].count = 7;		m_array_LinkGemsInOneChain[1].title = "Earthworm";
		m_array_LinkGemsInOneChain[2].count = 9;		m_array_LinkGemsInOneChain[2].title = "Centipede";
		m_array_LinkGemsInOneChain[3].count = 12;		m_array_LinkGemsInOneChain[3].title = "Snake";
		m_array_LinkGemsInOneChain[4].count = 15;		m_array_LinkGemsInOneChain[4].title = "Python";

#if UNITY_ANDROID
		m_array_LinkGemsInOneChain[0].achievementID = "";
		m_array_LinkGemsInOneChain[1].achievementID = "";
		m_array_LinkGemsInOneChain[2].achievementID = "";
		m_array_LinkGemsInOneChain[3].achievementID = "";
		m_array_LinkGemsInOneChain[4].achievementID = "";
#endif

		for (int i = 0; i < m_array_LinkGemsInOneChain.Length; ++i)
		{
			m_array_LinkGemsInOneChain[i].desc = "Linked " + m_array_LinkGemsInOneChain[i].count + " Gems in a Single Chain";
		}

		m_BoosterPlayedGames.title = "Score Multiplier Booster";
		m_BoosterPlayedGames.desc = "Unlocked a score multiplier booster for one-time use!";
		m_BoosterPlayedGames.count = 1;
#if UNITY_ANDROID
		m_BoosterPlayedGames.achievementID = "";
#endif

		m_BoosterPlayedGames.title = "Gold Multiplier Booster";
		m_BoosterPlayedGames.desc = "Unlocked a gold multiplier booster for one-time use!";
		m_BoosterPlayedGames.count = 1;
#if UNITY_ANDROID
		m_BoosterPlayedGames.achievementID = "";
#endif

		m_BoosterSharedFB.title = "Extra Health Booster";
		m_BoosterSharedFB.desc = "Unlocked an extra health booster for one-time use!";
		m_BoosterSharedFB.count = 1;
#if UNITY_ANDROID
		m_BoosterSharedFB.achievementID = "";
#endif
	}
}
