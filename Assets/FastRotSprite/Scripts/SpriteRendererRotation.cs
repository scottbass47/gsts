using UnityEngine;

namespace Assets.FastRotSprite.Scripts
{
	/// <summary>
	/// Performs pixel-by-pixel sprite rotation. Pixels outside the bounds will be ignored.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteRendererRotation : RotationBase
	{
		public void Awake()
		{
			if (Source == null)
			{
				Source = GetComponent<SpriteRenderer>().sprite.texture;
			}
		}

		public override void SetSprite(Sprite sprite)
		{
			GetComponent<SpriteRenderer>().sprite = sprite;
		}
	}
}