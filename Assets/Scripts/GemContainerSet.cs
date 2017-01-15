using UnityEngine;
using System.Collections;

// This shouldn't be a mono behaviour, I'm just lazy
public class GemContainerSet : MonoBehaviour
{
	public const int GEM_SET_NUM = 4;					//!< Red, Blue, Green Yellow
	public const int RED_GEM_CONTAINER_INDEX = 0;		//!< Red
	public const int BLUE_GEM_CONTAINER_INDEX = 1;		//!< Blue
	public const int GREEN_GEM_CONTAINER_INDEX = 2;		//!< Green
	public const int YELLOW_GEM_CONTAINER_INDEX = 3;    //!< Yellow

	public string m_sGemContainerSetName;

	// Sprites
	// Reason why it's not a map of array is because unity reflector doesn't support reflecting a container of a container
	public Sprite[] m_RedGemSpritesSet;
	public Sprite[] m_RedGlowGemSpritesSet;
	public Sprite[] m_RedStoneGemSpritesSet;

	public Sprite[] m_BlueGemSpritesSet;
	public Sprite[] m_BlueGlowGemSpritesSet;
	public Sprite[] m_BlueStoneGemSpritesSet;

	public Sprite[] m_GreenGemSpritesSet;
	public Sprite[] m_GreenGlowGemSpritesSet;
	public Sprite[] m_GreenStoneGemSpritesSet;

	public Sprite[] m_YellowGemSpritesSet;
	public Sprite[] m_YellowGlowGemSpritesSet;
	public Sprite[] m_YellowStoneGemSpritesSet;

	// Explosion
	public GameObject m_Explosion;

	// @todo Sound effect

	public Sprite[] GetGemContainer( int index )
	{
		switch ( index )
		{
			case RED_GEM_CONTAINER_INDEX:
				return m_RedGemSpritesSet;
			case BLUE_GEM_CONTAINER_INDEX:
				return m_BlueGemSpritesSet;
			case GREEN_GEM_CONTAINER_INDEX:
				return m_GreenGemSpritesSet;
			case YELLOW_GEM_CONTAINER_INDEX:
				return m_YellowGemSpritesSet;
			default:
				Debug.Log( "[GetGemContainer] Invalid index: " + index );
				return null;
		}
	}

	public Sprite[] GetGlowGemContainer(int index)
	{
		switch (index)
		{
			case RED_GEM_CONTAINER_INDEX:
				return m_RedGlowGemSpritesSet;
			case BLUE_GEM_CONTAINER_INDEX:
				return m_BlueGlowGemSpritesSet;
			case GREEN_GEM_CONTAINER_INDEX:
				return m_GreenGlowGemSpritesSet;
			case YELLOW_GEM_CONTAINER_INDEX:
				return m_YellowGlowGemSpritesSet;
			default:
				Debug.Log( "[GetGlowGemContainer] Invalid index: " + index );
				return null;
		}
	}

	public Sprite[] GetStoneGemContainer(int index)
	{
		switch (index)
		{
			case RED_GEM_CONTAINER_INDEX:
				return m_RedStoneGemSpritesSet;
			case BLUE_GEM_CONTAINER_INDEX:
				return m_BlueStoneGemSpritesSet;
			case GREEN_GEM_CONTAINER_INDEX:
				return m_GreenStoneGemSpritesSet;
			case YELLOW_GEM_CONTAINER_INDEX:
				return m_YellowStoneGemSpritesSet;
			default:
				Debug.Log( "[GetStoneGemContainer] Invalid index: " + index );
				return null;
		}
	}
}