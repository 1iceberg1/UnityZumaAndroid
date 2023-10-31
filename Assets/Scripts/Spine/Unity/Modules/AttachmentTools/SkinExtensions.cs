using System.Collections.Generic;

namespace Spine.Unity.Modules.AttachmentTools
{
	public static class SkinExtensions
	{
		public static Skin UnshareSkin(this Skeleton skeleton, bool includeDefaultSkin, bool unshareAttachments, AnimationState state = null)
		{
			Skin clonedSkin = skeleton.GetClonedSkin("cloned skin", includeDefaultSkin, unshareAttachments);
			skeleton.SetSkin(clonedSkin);
			if (state != null)
			{
				skeleton.SetToSetupPose();
				state.Apply(skeleton);
			}
			return clonedSkin;
		}

		public static Skin GetClonedSkin(this Skeleton skeleton, string newSkinName, bool includeDefaultSkin = false, bool cloneAttachments = false, bool cloneMeshesAsLinked = true)
		{
			Skin skin = new Skin(newSkinName);
			Skin defaultSkin = skeleton.data.DefaultSkin;
			Skin skin2 = skeleton.skin;
			if (includeDefaultSkin)
			{
				defaultSkin.CopyTo(skin, overwrite: true, cloneAttachments, cloneMeshesAsLinked);
			}
			skin2?.CopyTo(skin, overwrite: true, cloneAttachments, cloneMeshesAsLinked);
			return skin;
		}

		public static Skin GetClone(this Skin original)
		{
			Skin skin = new Skin(original.name + " clone");
			Dictionary<Skin.AttachmentKeyTuple, Attachment> attachments = skin.Attachments;
			foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> attachment in original.Attachments)
			{
				attachments[attachment.Key] = attachment.Value;
			}
			return skin;
		}

		public static void CopyTo(this Skin source, Skin destination, bool overwrite, bool cloneAttachments, bool cloneMeshesAsLinked = true)
		{
			Dictionary<Skin.AttachmentKeyTuple, Attachment> attachments = source.Attachments;
			Dictionary<Skin.AttachmentKeyTuple, Attachment> attachments2 = destination.Attachments;
			if (cloneAttachments)
			{
				if (overwrite)
				{
					foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> item in attachments)
					{
						attachments2[item.Key] = item.Value.GetClone(cloneMeshesAsLinked);
					}
				}
				else
				{
					foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> item2 in attachments)
					{
						if (!attachments2.ContainsKey(item2.Key))
						{
							attachments2.Add(item2.Key, item2.Value.GetClone(cloneMeshesAsLinked));
						}
					}
				}
			}
			else if (overwrite)
			{
				foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> item3 in attachments)
				{
					attachments2[item3.Key] = item3.Value;
				}
			}
			else
			{
				foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> item4 in attachments)
				{
					if (!attachments2.ContainsKey(item4.Key))
					{
						attachments2.Add(item4.Key, item4.Value);
					}
				}
			}
		}
	}
}
