public class InvitableFriend
{
	public string name;

	public string id;

	public string avatarUrl;

	public bool shouldInvite = true;

	public InvitableFriend(string id, string name, string avatarUrl)
	{
		this.id = id;
		this.name = name;
		this.avatarUrl = avatarUrl;
	}
}
