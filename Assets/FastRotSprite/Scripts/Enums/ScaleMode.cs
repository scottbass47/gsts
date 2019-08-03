namespace Assets.FastRotSprite.Scripts.Enums
{
	/// <summary>
	/// RotSprite uses Scale2x four times (resulting x8 upscale), but it's slow and may produce too large images.
	/// Single Scale3x makes similar result with much better performance.
	/// </summary>
	public enum ScaleMode
	{
		X3 = 3,
		X8 = 8
	}
}