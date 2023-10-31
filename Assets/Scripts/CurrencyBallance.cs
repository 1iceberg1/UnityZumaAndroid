using System;
using UnityEngine;

public class CurrencyBallance : MonoBehaviour
{
	private void Start()
	{
		UpdateBalance();
		CurrencyController.onBalanceChanged = (Action)Delegate.Combine(CurrencyController.onBalanceChanged, new Action(OnBalanceChanged));
	}

	private void UpdateBalance()
	{
		base.gameObject.SetText(CurrencyController.GetBalance().ToString());
	}

	private void OnBalanceChanged()
	{
		UpdateBalance();
	}

	private void OnDestroy()
	{
		CurrencyController.onBalanceChanged = (Action)Delegate.Remove(CurrencyController.onBalanceChanged, new Action(OnBalanceChanged));
	}
}
