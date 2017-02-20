using UnityEngine;
using System.Collections;

public class GemDetails : MonoBehaviour
{
	public GemContainerSet m_GemSet;

	GemLibrary m_GemLibrary;

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad( this );

		//m_GemLibrary = GameObject.Find( "Gem Library" ).GetComponent<GemLibrary>();
		m_GemLibrary = GemLibrary.Instance;

		// If gem set is inside the inventory
		if ( GameData.Instance.m_Sets.Contains( GameData.Instance.m_EquippedGemSet ) )
		{
			m_GemSet = m_GemLibrary.GemsSetList[ (int)GameData.Instance.m_EquippedGemSet ];
		}
		else
		{
			EquipGemSet( GemLibrary.GemSet.GEM );
		}
	}

	public void SetGemSpriteContainer( GemSpriteContainer gemSpriteContainer, int gemType )
	{
		gemSpriteContainer.GetComponent<SpriteRenderer>().sprite = m_GemSet.GetGemContainer( gemType )[0];
		gemSpriteContainer.m_Sprites = m_GemSet.GetGemContainer( gemType );
		gemSpriteContainer.m_GlowSprites = m_GemSet.GetGlowGemContainer( gemType );
		gemSpriteContainer.m_StoneSprites = m_GemSet.GetStoneGemContainer( gemType );

		/*
		switch ( gemType )
		{
			case 0:
				gemSpriteContainer.m_Sprites = m_GemSet.m_RedGemSpritesSet;
				gemSpriteContainer.m_GlowSprites = m_GemSet.m_RedGlowGemSpritesSet;
				gemSpriteContainer.m_StoneSprites = m_GemSet.m_RedStoneGemSpritesSet;
				break;
			case 1:
				gemSpriteContainer.m_Sprites = m_GemSet.m_BlueGemSpritesSet;
				gemSpriteContainer.m_GlowSprites = m_GemSet.m_BlueGlowGemSpritesSet;
				gemSpriteContainer.m_StoneSprites = m_GemSet.m_BlueStoneGemSpritesSet;
				break;
			case 2:
				gemSpriteContainer.m_Sprites = m_GemSet.m_GreenGemSpritesSet;
				gemSpriteContainer.m_GlowSprites = m_GemSet.m_GreenGlowGemSpritesSet;
				gemSpriteContainer.m_StoneSprites = m_GemSet.m_GreenStoneGemSpritesSet;
				break;
			case 3:
				gemSpriteContainer.m_Sprites = m_GemSet.m_YellowGemSpritesSet;
				gemSpriteContainer.m_GlowSprites = m_GemSet.m_YellowGlowGemSpritesSet;
				gemSpriteContainer.m_StoneSprites = m_GemSet.m_YellowStoneGemSpritesSet;
				break;
			default:
				break;
		}
		*/
	}

	// Equip (Set gem set)
	public bool EquipGemSet( GemLibrary.GemSet gemSet )
	{
		m_GemSet = m_GemLibrary.GemsSetList[ (int)gemSet ];
		if ( GameData.Instance.m_Sets.Contains( gemSet ) )
		{
			GameData.Instance.m_EquippedGemSet = gemSet;
			SaveLoad.Save();
			return true;
		}

		return false;
	}
}
