using UnityEngine;
using UnityEngine.UI;

namespace Assets.FastRotSprite.Scripts
{
	/// <summary>
	/// Performs pixel-by-pixel sprite rotation. Pixels outside the bounds will be ignored.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class ImageRotation : RotationBase
	{
		public void Awake()
		{
			if (Source == null)
			{
				Source = GetComponent<Image>().sprite.texture;
			}
		}

		public override void SetSprite(Sprite sprite)
		{
			GetComponent<Image>().sprite = sprite;
		}
	}
}