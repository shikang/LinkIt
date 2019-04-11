using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour {

	public GameObject m_BGMSlider;
	public GameObject m_SFXSlider;
	public GameObject m_Vibration;

	// Use this for initialization
	void Start () {

		m_BGMSlider.GetComponent<Slider>().value = GameData.Instance.m_vol_BGM / 100.0f;
		m_SFXSlider.GetComponent<Slider>().value = GameData.Instance.m_vol_SFX / 100.0f;
		m_Vibration.GetComponent<Toggle>().isOn = GameData.Instance.m_VibrationDisable;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
