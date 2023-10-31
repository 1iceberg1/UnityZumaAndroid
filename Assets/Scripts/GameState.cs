using System.Collections.Generic;

public class GameState
{
	public enum State
	{
		Normal,
		Flying,
		Joining,
		Reversing,
		ReplacingPusher,
		Backwarding
	}

	public static List<State> states;

	public static bool pauseGame;

	public static void Init()
	{
		states = new List<State>();
		states.Add(State.Normal);
	}

	public static void BeginFlying()
	{
		states.Add(State.Flying);
	}

	public static void EndFlying()
	{
		states.Remove(State.Flying);
	}

	public static void BeginJoining()
	{
		states.Add(State.Joining);
	}

	public static void EndJoining()
	{
		states.Remove(State.Joining);
	}

	public static void BeginReversing()
	{
		states.Add(State.Reversing);
	}

	public static void EndReversing()
	{
		states.Remove(State.Reversing);
	}

	public static void BeginReplacingPusher()
	{
		states.Add(State.ReplacingPusher);
	}

	public static void EndReplacingPusher()
	{
		states.Remove(State.ReplacingPusher);
	}

	public static void BeginBackwarding()
	{
		states.Add(State.Backwarding);
	}

	public static void EndBackwarding()
	{
		states.Remove(State.Backwarding);
	}

	public static bool IsNormal()
	{
		return states.Count == 1 && states[0] == State.Normal;
	}

	public static bool IsReversing()
	{
		return states.Contains(State.Reversing);
	}

	public static bool IsReplacingPusher()
	{
		return states.Contains(State.ReplacingPusher);
	}

	public static bool IsBackwarding()
	{
		return states.Contains(State.Backwarding);
	}

	public static bool IsJoining()
	{
		return states.Contains(State.Joining);
	}

	public static State GetLastState()
	{
		return states[states.Count - 1];
	}
}
