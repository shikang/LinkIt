using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterHeader : MonoBehaviour {

	public GameObject m_pBoosterText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetText(string s)
	{
		m_pBoosterText.GetComponent<Text>().text = s;
	}
}
