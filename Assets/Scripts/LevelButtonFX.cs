using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButtonFX : MonoBehaviour
{
	void Start ()
	{
	
	}

	void Update ()
	{
		if (GetComponent<Image> ().color.a <= 0.05f)
			Destroy (this.gameObject);
		
		transform.localScale *= 1.07f;

		Color tmp = GetComponent<Image>().color;
		tmp.a -= Time.deltaTime * 5.0f;
		GetComponent<Image> ().color = tmp;
	}
}

