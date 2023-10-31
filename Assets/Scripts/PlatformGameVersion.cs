public class PlatformGameVersion
{
	public enum Platform
	{
		Android,
		Ios,
		Windowphone,
		Blacberry
	}

	public Platform PlatformType
	{
		get;
		set;
	}

	public string Version
	{
		get;
		set;
	}

	public string StoreUrl
	{
		get;
		set;
	}

	public bool ForceUpdate
	{
		get;
		set;
	}

	public string Message
	{
		get;
		set;
	}
}
