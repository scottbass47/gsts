using System.Threading.Tasks;
using Assets.FastRotSprite.Scripts.Enums;
using UnityEngine;

namespace Assets.FastRotSprite.Scripts
{
	public class Rotator
	{
		public readonly Color32[] PixelsEx;
		public readonly int WidthEx, HeightEx;
		public readonly float ExRatio;

		private Color32[] _temp, _rotated;

		public Rotator(Color32[] source, int width, int height)
		{
			var max = Mathf.Max(width, height);
			var size = Mathf.CeilToInt(max * Mathf.Sqrt(2));

			if (max % 2 == 0 && size % 2 != 0) size++;
			else if (max % 2 != 0 && size % 2 == 0) size++;

			ExRatio = (float) size / max;

			PixelsEx = new Color32[size * size];

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var x0 = x + Mathf.FloorToInt(size / 2f - width / 2f);
					var y0 = y + Mathf.FloorToInt(size / 2f - height / 2f);

					PixelsEx[x0 + y0 * size] = source[x + y * width];
				}
			}

			WidthEx = HeightEx = size;
		}

		public Color32[] Rotate(float radians)
		{
			ScaleHelper.Threshold = 0.05f * 255f;

			_temp = ScaleHelper.Upscale3X(PixelsEx, WidthEx, HeightEx);

			var width = WidthEx * 3;
			var height = HeightEx * 3;

			_rotated = new Color32[_temp.Length];

			const float offset = 0.5f;

			Parallel.For(0, width, x =>
			{
				for (var y = 0; y < height; y++)
				{
					var x0 = x - width / 2f + offset;
					var y0 = y - height / 2f + offset;

					var angle = Mathf.Atan2(y0, x0) + radians;
					var radius = new Vector2(x0, y0).magnitude;

					var x1 = Mathf.RoundToInt(radius * Mathf.Cos(angle) + width / 2f - offset);
					var y1 = Mathf.RoundToInt(radius * Mathf.Sin(angle) + height / 2f - offset);

					if (x1 < 0 || x1 >= width || y1 < 0 || y1 >= height) continue;

					_rotated[x1 + y1 * width] = _temp[x + y * width];
				}
			});

			_temp = ScaleHelper.Downscale3X(_rotated, width, height, DownscaleMode.ByAvg);

			return _temp;
		}
	}
}