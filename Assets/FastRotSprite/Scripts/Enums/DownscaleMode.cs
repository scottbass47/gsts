namespace Assets.FastRotSprite.Scripts.Enums
{
	public enum DownscaleMode
	{
		/// <summary>
		/// Example: if we have 3 [white], 1 [gray] and 4 [black] pixels, the resulting pixel will be [gray].
		/// </summary>
		ByAvg,

		/// <summary>
		/// Example: if we have 3 [white], 1 [gray] and 4 [black] pixels, the resulting pixel will be [black].
		/// </summary>
		ByMax
	}
}