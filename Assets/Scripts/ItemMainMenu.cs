using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemMainMenu : MonoBehaviour
{
	bool m_bIsActivated;
	string m_sName;
	string m_sDesc;
	uint m_uCost;

	public GameObject m_ItemName;
	public GameObject m_Cost;
	public GameObject m_Desc;

	Color m_cActive = new Color(0.0f, 0.5f, 1.0f, 1.0f);
	Color m_cInactive = Color.grey;

	void Start()
	{
	}

	void Update()
	{
	}

	public void SetData(string name_, uint cost_, string desc_)
	{
		SetName(name_);
		SetCost(cost_);
		SetDesc(desc_);
		m_bIsActivated = false;
		GetComponent<Image>().color = m_cInactive;
	}

	public void SetName (string name_)
	{
		m_sName = name_;
		m_ItemName.GetComponent<Text>().text = m_sName;
	}

	public void SetCost (uint cost_)
	{
		m_uCost = cost_;
		m_Cost.GetComponent<Text>().text = m_uCost.ToString();
	}

	public void SetDesc (string desc_)
	{
		m_sDesc = desc_;
		m_Desc.GetComponent<Text>().text = m_sDesc;
	}

	public void SetActive()
	{
		m_bIsActivated = !m_bIsActivated;

		if(m_bIsActivated)
			GetComponent<Image>().color = m_cActive;
		else
			GetComponent<Image>().color = m_cInactive;
	}

	public string GetName()
	{
		return m_sName;
	}

	public uint GetCost()
	{
		return m_uCost;
	}

	public string GetDesc()
	{
		return m_sDesc;
	}

	public bool IsActive()
	{
		return m_bIsActivated;
	}
}
