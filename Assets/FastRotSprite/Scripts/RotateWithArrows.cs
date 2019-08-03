using UnityEngine;

namespace Assets.FastRotSprite.Scripts
{
	/// <summary>
	/// Rotates sprite with arrows (Input).
	/// </summary>
	public class RotateWithArrows : MonoBehaviour
	{
		public ImageRotation ImageRotation;

		private float _radians;

		public void Update()
		{
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
			{
				_radians += 100 * (Input.GetKey(KeyCode.LeftArrow) ? -Time.deltaTime : Time.deltaTime) * Mathf.Deg2Rad;
				
				ImageRotation.RotationRadians = _radians;
			}
		}
	}
}