using UnityEngine;

namespace Assets.FastRotSprite.Scripts
{
	/// <summary>
	/// Rotates sprite on Start by specified degree.
	/// </summary>
	[RequireComponent(typeof(ImageRotation))]
	public class RotateOnStart : MonoBehaviour
	{
		public float StartRotationDeg;

		public void Start()
		{
			if (!Mathf.Approximately(StartRotationDeg, 0))
			{
				GetComponent<RotationBase>().Rotate(StartRotationDeg * Mathf.Deg2Rad);
			}
		}
	}
}