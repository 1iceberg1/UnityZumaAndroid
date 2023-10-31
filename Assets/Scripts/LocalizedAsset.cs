using UnityEngine;

public class LocalizedAsset : MonoBehaviour
{
	public UnityEngine.Object localizeTarget;

	public void Awake()
	{
		LocalizeAsset(localizeTarget);
	}

	public void LocalizeAsset()
	{
		LocalizeAsset(localizeTarget);
	}

	public static void LocalizeAsset(UnityEngine.Object target)
	{
		if (target == null)
		{
			UnityEngine.Debug.LogError("LocalizedAsset target is null");
		}
		
		else if (target.GetType() == typeof(Material))
		{
			Material material = (Material)target;
			if (material.mainTexture != null)
			{
				Texture texture2 = (Texture)Language.GetAsset(material.mainTexture.name);
				if (texture2 != null)
				{
					material.mainTexture = texture2;
				}
			}
		}
		else if (target.GetType() == typeof(MeshRenderer))
		{
			MeshRenderer meshRenderer = (MeshRenderer)target;
			if (meshRenderer.material.mainTexture != null)
			{
				Texture texture3 = (Texture)Language.GetAsset(meshRenderer.material.mainTexture.name);
				if (texture3 != null)
				{
					meshRenderer.material.mainTexture = texture3;
				}
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Could not localize this object type: " + target.GetType());
		}
	}
}
