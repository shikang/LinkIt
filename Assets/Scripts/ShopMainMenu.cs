using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopMainMenu : MonoBehaviour
{
	string m_sName;
	string m_sDesc;
	uint m_uCoin;
	string m_sCost;

	public GameObject m_ItemName;
	public GameObject m_Cost;
	public GameObject m_Desc;

	void Start()
	{
	}

	void Update()
	{
	}

	public void SetData(string name_, uint coin_, string cost_, string desc_)
	{
		SetName(name_);
		SetCoin(coin_);
		SetCost(cost_);
		SetDesc(desc_);

		// Set in app purchase

	}

	public void SetName (string name_)
	{
		m_sName = name_;
		m_ItemName.GetComponent<Text>().text = m_sName;
	}

	public void SetCoin (uint coin_)
	{
		m_uCoin = coin_;
	}

	public void SetCost (string cost_)
	{
		//m_fCost = cost_;
		//m_Cost.GetComponent<Text>().text = "$" + m_fCost.ToString();
		m_sCost = cost_;
		m_Cost.GetComponent<Text>().text = cost_;
	}

	public void SetDesc (string desc_)
	{
		m_sDesc = desc_.Replace("%d", m_uCoin.ToString());
		m_Desc.GetComponent<Text>().text = m_sDesc;
	}

	public string GetName()
	{
		return m_sName;
	}

	public uint GetCoin()
	{
		return m_uCoin;
	}

	public string GetCost()
	{
		return m_sCost;
	}

	public string GetDesc()
	{
		return m_sDesc;
	}
}
