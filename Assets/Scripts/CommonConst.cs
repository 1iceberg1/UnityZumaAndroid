public class CommonConst
{
	public const string GP_PAGE_LINK = "https://plus.google.com/106625749530285902481/posts";

	//public const iTween.DimensionMode ITWEEN_MODE = iTween.DimensionMode.mode2D;

	public const string FEED_LINK = "https://play.google.com/store/apps/details?id=com.superpow.fishshooter";

	public const string FEED_PICTURE = "http://s13.postimg.org/e5ztzvglj/banner1024x500.jpg";

	public static readonly int[] START_FRIEND_LEVELS = new int[5]
	{
		3,
		5,
		7,
		12,
		18
	};

	public const bool HAS_INVITE_FRIEND = true;

	public const int MIN_INVITE_FRIEND = 40;

	public const int MAX_INVITE_FRIEND = 50;

	public const bool ENCRYPTION_PREFS = true;

	public const int MIN_LEVEL_TO_RATE = 3;

	public const int MAX_FRIEND_IN_MAP = 15;

	public const int FACE_AVATAR_SIZE = 100;

	public const int TOTAL_LEVELS = 50;

	public const int NOTIFICATION_DAILY_GIFT = 0;

	public static readonly string[] LEADERBOARD = new string[2]
	{
		"CgkIx92ynI8eEAIQBw",
		"CgkIx92ynI8eEAIQCA"
	};

	public const int MAX_AUTO_SIGNIN = 2;

	public static int GetTargetScore(int level)
	{
		return 1000;
	}
}
