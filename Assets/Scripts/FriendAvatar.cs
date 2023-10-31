using UnityEngine;

public class FriendAvatar : Photo
{
	public int index;

	private void Start()
	{
		SetSize(100, 100);
		SetRealSize(100, 100);
		if (CGameState.friendAvatars[index] != null)
		{
			SetPhoto(CGameState.friendAvatars[index]);
		}
		else
		{
			Load();
		}
	}

	protected override void OnPhotoLoaded(Sprite sprite)
	{
		base.OnPhotoLoaded(sprite);
		CGameState.friendAvatars[index] = sprite;
	}
}
