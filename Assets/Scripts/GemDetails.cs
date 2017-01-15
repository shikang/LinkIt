using UnityEngine;
using System.Collections;

public class GemDetails : MonoBehaviour
{
	public GemContainerSet m_GemSet;

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad( this );

		// @todo Load once in main menu.
		SaveLoad.Load();
		GemLibrary gemLibrary = GameObject.Find( "Gem Library" ).GetComponent<GemLibrary>();
		m_GemSet = gemLibrary.m_GemsSetList[ (int)GameData.Instance.m_EquippedGemSet ];
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

	// @todo Equip (Set gem set)
}
