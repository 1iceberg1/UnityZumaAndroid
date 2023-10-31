using Superpow;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BoosterNumber : MonoBehaviour
{
	public int boosterID;

	private void Start()
	{
		Utils.onBoosterNumberChanged = (Action<int>)Delegate.Combine(Utils.onBoosterNumberChanged, new Action<int>(OnBoosterNumberChanged));
		UpdateUI();
	}

	private void UpdateUI()
	{
		GetComponent<Text>().text = Utils.GetBoosterNumber(boosterID).ToString();
		
	}

	private void OnBoosterNumberChanged(int boosterID)
	{
		if (this.boosterID == boosterID)
		{
			
			UpdateUI();
		}
	}

	private void OnDestroy()
	{
		Utils.onBoosterNumberChanged = (Action<int>)Delegate.Remove(Utils.onBoosterNumberChanged, new Action<int>(OnBoosterNumberChanged));
	}
}
