using System;

public class CurrencyController
{
	public const string CURRENCY = "coins";

	public const int DEFAULT_CURRENCY = 10;

	public static Action onBalanceChanged;

	public static Action<int> onBallanceIncreased;

	public static int GetBalance()
	{
		return CPlayerPrefs.GetInt("coins", 10);
	}

	public static void SetBalance(int value)
	{
		CPlayerPrefs.SetInt("coins", value);
		CPlayerPrefs.Save();
	}

	public static void CreditBalance(int value)
	{
		int balance = GetBalance();
		SetBalance(balance + value);
		if (onBalanceChanged != null)
		{
			onBalanceChanged();
		}
		if (onBallanceIncreased != null)
		{
			onBallanceIncreased(value);
		}
	}

	public static bool DebitBalance(int value)
	{
		int balance = GetBalance();
		if (balance < value)
		{
			return false;
		}
		SetBalance(balance - value);
		if (onBalanceChanged != null)
		{
			onBalanceChanged();
		}
		return true;
	}
}
