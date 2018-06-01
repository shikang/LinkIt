using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Splash : MonoBehaviour
{
	public GameObject [] screens;
	public GameObject [] logo;
	int currScreen;
	float currTime;

	float showScreenTime;
	float fadeInSpeed;
	float fadeOutSpeed;
	int stage;

	void Start ()
	{
		currScreen = 0;
		currTime = 0.0f;
		fadeInSpeed = 0.4f;
		fadeOutSpeed = 1.5f;
		showScreenTime = 1.0f;
		stage = 1;

		Color tmp;
		for(int i = 0; i < screens.Length; ++i)
		{
			tmp = screens [i].GetComponent<Image> ().color;
			tmp.a = 0.0f;
			screens [i].GetComponent<Image> ().color = tmp;

			tmp = logo [i].GetComponent<Image> ().color;
			tmp.a = 0.0f;
			logo [i].GetComponent<Image> ().color = tmp;

			screens[i].SetActive(false);
			logo [i].SetActive (false);
		}
	}

	void Update ()
	{
		if(Input.anyKey)
			SceneManager.LoadScene ("MainMenu");
		
		if(currScreen < screens.Length)
		{
			if(stage == 1)
			{
				screens[currScreen].SetActive(true);
				logo[currScreen].SetActive(true);

				Color tmp = screens [currScreen].GetComponent<Image> ().color;
				tmp.a += fadeInSpeed * Time.deltaTime;
				screens [currScreen].GetComponent<Image> ().color = tmp;

				tmp = logo [currScreen].GetComponent<Image> ().color;
				tmp.a += fadeInSpeed * Time.deltaTime;
				logo [currScreen].GetComponent<Image> ().color = tmp;

				if (tmp.a >= 1.0f)
					stage = 2;
			}
			else if(stage == 2)
			{
				currTime += Time.deltaTime;
				if (currTime >= showScreenTime)
					stage = 3;
			}
			else if(stage == 3)
			{
				Color tmp = screens [currScreen].GetComponent<Image> ().color;
				tmp.a -= fadeInSpeed * Time.deltaTime;
				screens [currScreen].GetComponent<Image> ().color = tmp;

				tmp = logo [currScreen].GetComponent<Image> ().color;
				tmp.a -= fadeInSpeed * Time.deltaTime;
				logo [currScreen].GetComponent<Image> ().color = tmp;

				if (tmp.a <= 0.0f)
				{
					screens[currScreen].SetActive(false);
					logo[currScreen].SetActive(false);

					currScreen++;
					currTime = 0.0f;
					stage = 1;
				}
			}
		}
		else
		{
			SceneManager.LoadScene ("MainMenu");
		}
	}
}

