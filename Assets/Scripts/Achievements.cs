using SimpleJSON;
using Superpow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements
{
	public List<Achievement> achievements;

	public int maxShown = int.MaxValue;

	public Action onAchievementComplete;

	public Action onRewardReceived;

	protected int lastIndex;

	public Achievements()
	{
		achievements = new List<Achievement>();
	}

	public void Increasement(string id, int step)
	{
		int index;
		Achievement achievement = GetAchievement(id, out index);
		achievement.currentStep = Mathf.Clamp(achievement.currentStep + step, 0, achievement.totalStep);
		if (achievement.currentStep == achievement.totalStep)
		{
			achievement.isUnlocked = true;
			if (onAchievementComplete != null)
			{
				onAchievementComplete();
			}
		}
		achievements[index] = achievement;
		SaveAchievement(achievement);
	}

	public Achievement GetAchievement(string id)
	{
		return achievements.Find((Achievement x) => x.id == id);
	}

	public Achievement GetAchievement(string id, out int index)
	{
		index = -1;
		for (int i = 0; i < achievements.Count; i++)
		{
			if (achievements[i].id == id)
			{
				index = i;
				return achievements[i];
			}
		}
		return null;
	}

	public Achievement GetAchievementFromPref(string id)
	{
		string @string = CPlayerPrefs.GetString("achievement_" + id);
		if (string.IsNullOrEmpty(@string))
		{
			return null;
		}
		return ParseAchievement(@string);
	}

	public Achievement ParseAchievement(string json)
	{
		JSONNode n = JSON.Parse(json);
		return ParseAchievement(n);
	}

	public Achievement ParseAchievement(JSONNode N)
	{
		Achievement achievement = new Achievement();
		achievement.id = N["id"];
		achievement.description = N["description"];
		achievement.icon = N["icon"];
		achievement.isUnlocked = N["isUnlocked"].AsBool;
		achievement.totalStep = ((!(N["totalStep"] != null)) ? 1 : N["totalStep"].AsInt);
		achievement.currentStep = ((N["currentStep"] != null) ? N["currentStep"].AsInt : 0);
		achievement.type = N["type"];
		Reward reward = new Reward();
		reward.type = N["reward_type"];
		reward.number = N["reward_number"].AsInt;
		reward.received = N["isRewardReceived"].AsBool;
		achievement.reward = reward;
		return achievement;
	}

	public List<Achievement> GetAchievementsToShow()
	{
		List<Achievement> list = achievements.FindAll((Achievement x) => !x.reward.received);
		int count = Mathf.Min(maxShown, list.Count);
		return list.GetRange(0, count);
	}

	public virtual void ReceiveReward(string achivementID)
	{
		int index;
		Achievement achievement = GetAchievement(achivementID, out index);
		achievements[index].reward.received = true;
		Reward(achievement);
		if (onRewardReceived != null)
		{
			onRewardReceived();
		}
	}

	protected virtual void Reward(Achievement ch)
	{
	}

	protected virtual void Load(string json)
	{
		achievements.Clear();
		JSONArray asArray = JSON.Parse(json).AsArray;
		IEnumerator enumerator = asArray.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JSONNode n = (JSONNode)enumerator.Current;
				Achievement item = ParseAchievement(n);
				achievements.Add(item);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void LoadFile(string filePath)
	{
		string json = CUtils.ReadFileContent(filePath);
		Load(json);
	}

	public void UpdateData()
	{
		for (int i = 0; i < achievements.Count; i++)
		{
			Achievement achievement = achievements[i];
			string key = "achievement_" + achievement.id;
			if (CPlayerPrefs.HasKey(key))
			{
				JSONNode jSONNode = JSON.Parse(CPlayerPrefs.GetString(key));
				achievement.isUnlocked = jSONNode["isUnlocked"].AsBool;
				achievement.reward.received = jSONNode["isRewardReceived"].AsBool;
				achievement.currentStep = jSONNode["currentStep"].AsInt;
				achievements[i] = achievement;
			}
		}
	}

	public void SaveAchievement(string id)
	{
		int index;
		Achievement achievement = GetAchievement(id, out index);
		SaveAchievement(achievement);
	}

	public void SaveAchievement(Achievement ach)
	{
		JSONClass jSONClass = new JSONClass();
		jSONClass["id"] = ach.id;
		jSONClass["description"] = ach.description;
		jSONClass["icon"] = ach.icon;
		jSONClass["isUnlocked"].AsBool = ach.isUnlocked;
		jSONClass["type"] = ach.type;
		jSONClass["currentStep"].AsInt = ach.currentStep;
		jSONClass["totalStep"].AsInt = ach.totalStep;
		jSONClass["reward_type"] = ach.reward.type;
		jSONClass["reward_number"].AsInt = ach.reward.number;
		jSONClass["isRewardReceived"].AsBool = ach.reward.received;
		CPlayerPrefs.SetString("achievement_" + ach.id, jSONClass.ToString());
		CPlayerPrefs.Save();
	}

	public void SaveAllAchievements()
	{
		foreach (Achievement achievement in achievements)
		{
			SaveAchievement(achievement);
		}
	}

	protected virtual string BuildType(object type, object additionalParam)
	{
		return null;
	}

	protected virtual List<Achievement> GetMatchAchievement(string strType)
	{
		return null;
	}

	public void Check(object type, int step = 1, object additionalParam = null)
	{
		string strType = BuildType(type, additionalParam);
		List<Achievement> matchAchievement = GetMatchAchievement(strType);
		foreach (Achievement item in matchAchievement)
		{
			if (item != null)
			{
				Increasement(item.id, step);
			}
		}
	}
}
