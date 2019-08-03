using System.Threading.Tasks;
using Assets.FastRotSprite.Scripts.Enums;
using UnityEngine;
using ScaleMode = Assets.FastRotSprite.Scripts.Enums.ScaleMode;

namespace Assets.FastRotSprite.Scripts
{
	/// <summary>
	/// Performs pixel-by-pixel sprite rotation. Pixels outside the bounds will be ignored.
	/// </summary>
	public abstract class RotationBase : MonoBehaviour
	{
		public Texture2D Source;
		[Header("Parameters")]
		public ScaleMode ScaleMode = ScaleMode.X3;
		public RotateMode RotateMode = RotateMode.Normal;
		public DownscaleMode DownscaleMode = DownscaleMode.ByAvg;
		[Header("Compare colors threshold"), Range(0, 1)]
		public float Threshold = 0.05f;

        public float RotationDegrees
        {
            get => _rotation * Mathf.Rad2Deg;
            set => RotationRadians = value * Mathf.Deg2Rad;
        }

		public float RotationRadians
		{
			get => _rotation;
			set => Rotate(value);
		}

		#region Reusable variables.

		private float _rotation;
		private Rotator _rotator;
		private Texture2D _texture;

		#endregion

		public void Rotate(float radians)
		{
			_rotation = radians;

			if (_rotator == null)
			{
				_rotator = new Rotator(Source.GetPixels32(), Source.width, Source.height);
			}

			ScaleHelper.Threshold = Threshold;


			var rotated = CreateTexture(_rotator.Rotate(radians), _rotator.WidthEx, _rotator.HeightEx);
            var sprite = Sprite.Create(rotated, new Rect(0f, 0f, rotated.width, rotated.height), Vector2.one * 0.5f, 16, 0, SpriteMeshType.FullRect);

			SetSprite(sprite);
		}

		public abstract void SetSprite(Sprite sprite);

		private Texture2D CreateTexture(Color32[] pixels, int width, int height)
		{
			if (_texture == null)
			{
				_texture = new Texture2D(width, height) { filterMode = FilterMode.Point };
			}
			else
			{
				_texture.Resize(width, height);
			}

			_texture.SetPixels32(pixels);
			_texture.Apply();

			return _texture;
		}
	}
}