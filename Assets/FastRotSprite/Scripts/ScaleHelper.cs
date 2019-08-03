using System;
using System.Linq;
using System.Threading.Tasks;
using Assets.FastRotSprite.Scripts.Enums;
using UnityEngine;

namespace Assets.FastRotSprite.Scripts
{
	/// <summary>
	/// Performs pixel art scaling. The main requirement: no new colors should be introduced.
	/// </summary>
	public static class ScaleHelper
	{
		/// <summary>
		/// Threshold when comparing colors. Used in modified upscaling algorithms.
		/// </summary>
		public static float Threshold = 0.05f * 255f;

		/// <summary>
		/// Performs x2 upscaling by applying Scale2x algorithm.
		/// </summary>
		public static Color32[] Upscale2X(Color32[] sourcePixels, int width, int height)
		{
			var pixels = new Color32[4 * sourcePixels.Length];

			Color32 GetPixel(int x, int y)
			{
				return sourcePixels[x + y * width];
			}

			Func<Color32, Color32, bool> equal;

			if (Mathf.Approximately(Threshold, 0))
			{
				equal = (a, b) => a.Equals(b);
			}
			else
			{
				equal = (a, b) => Mathf.Abs(a.r - b.r) <= Threshold && Mathf.Abs(a.g - b.g) <= Threshold && Mathf.Abs(a.b - b.b) <= Threshold && Mathf.Abs(a.a - b.a) <= Threshold;
			}

			Parallel.For(0, width, x =>
			{
				for (var y = 0; y < height; y++)
				{
					var b = y == height - 1 ? GetPixel(x, y) : GetPixel(x, y + 1);
					var d = x == 0 ? GetPixel(x, y) : GetPixel(x - 1, y);
					var e = GetPixel(x, y);
					var f = x == width - 1 ? GetPixel(x, y) : GetPixel(x + 1, y);
					var h = y == 0 ? GetPixel(x, y) : GetPixel(x, y - 1);

					Color e0, e1, e2, e3;

					if (!equal(b, h) && !equal(d, f))
					{
						e0 = equal(d, b) ? d : e;
						e1 = equal(b, f) ? f : e;
						e2 = equal(d, h) ? d : e;
						e3 = equal(h, f) ? f : e;
					}
					else
					{
						e0 = e;
						e1 = e;
						e2 = e;
						e3 = e;
					}

					pixels[2 * x + (2 * y + 1) * width * 2] = e0;
					pixels[2 * x + 1 + (2 * y + 1) * width * 2] = e1;
					pixels[2 * x + 2 * y * width * 2] = e2;
					pixels[2 * x + 1 + 2 * y * width * 2] = e3;
				}
			});

			return pixels;
		}

		/// <summary>
		/// Performs x8 upscaling by applying Scale2x algorithm 8 times.
		/// </summary>
		public static Color32[] Upscale8X(Texture2D texture)
		{
			return Upscale2X(Upscale2X(Upscale2X(texture.GetPixels32(), texture.width, texture.height), texture.width * 2, texture.height * 2), texture.width * 4, texture.height * 4);
		}

		/// <summary>
		/// Performs x3 upscaling by applying Scale3x algorithm.
		/// </summary>
		public static Color32[] Upscale3X(Color32[] pixels, int width, int height)
		{
			var scaled = new Color32[9 * pixels.Length];

			Color32 GetPixel(int x, int y)
			{
				return pixels[x + y * width];
			}

			Func<Color32, Color32, bool> equal;

			if (Mathf.Approximately(Threshold, 0))
			{
				equal = (a, b) => a.Equals(b);
			}
			else
			{
				equal = (a, b) => Mathf.Abs(a.r - b.r) <= Threshold && Mathf.Abs(a.g - b.g) <= Threshold && Mathf.Abs(a.b - b.b) <= Threshold && Mathf.Abs(a.a - b.a) <= Threshold;
			}

			Parallel.For(0, width, x =>
			{
				for (var y = 0; y < height; y++)
				{
					var a = x == 0 || y == height - 1 ? GetPixel(x, y) : GetPixel(x - 1, y + 1);
					var b = y == height - 1 ? GetPixel(x, y) : GetPixel(x, y + 1);
					var c = x == width - 1 || y == height - 1 ? GetPixel(x, y) : GetPixel(x + 1, y + 1);
					var d = x == 0 ? GetPixel(x, y) : GetPixel(x - 1, y);
					var e = GetPixel(x, y);
					var f = x == width - 1 ? GetPixel(x, y) : GetPixel(x + 1, y);
					var g = x == 0 || y == 0 ? GetPixel(x, y) : GetPixel(x - 1, y - 1);
					var h = y == 0 ? GetPixel(x, y) : GetPixel(x, y - 1);
					var i = x == width - 1 || y == 0 ? GetPixel(x, y) : GetPixel(x + 1, y - 1);

					Color e0, e1, e2, e3, e4, e5, e6, e7, e8;

					if (!equal(b, h) && !equal(d, f))
					{
						e0 = equal(d, b) ? d : e;
						e1 = equal(d, b) && !equal(e, c) || equal(b, f) && !equal(e, a) ? b : e;
						e2 = equal(b, f) ? f : e;
						e3 = equal(d, b) && !equal(e, g) || equal(d, h) && !equal(e, a) ? d : e;
						e4 = e;
						e5 = equal(b, f) && !equal(e, i) || equal(h, f) && !equal(e, c) ? f : e;
						e6 = equal(d, h) ? d : e;
						e7 = equal(d, h) && !equal(e, i) || equal(h, f) && !equal(e, g) ? h : e;
						e8 = equal(h, f) ? f : e;
					}
					else
					{
						e0 = e;
						e1 = e;
						e2 = e;
						e3 = e;
						e4 = e;
						e5 = e;
						e6 = e;
						e7 = e;
						e8 = e;
					}

					scaled[3 * x + (3 * y + 2) * width * 3] = e0;
					scaled[3 * x + 1 + (3 * y + 2) * width * 3] = e1;
					scaled[3 * x + 2 + (3 * y + 2) * width * 3] = e2;

					scaled[3 * x + (3 * y + 1) * width * 3] = e3;
					scaled[3 * x + 1 + (3 * y + 1) * width * 3] = e4;
					scaled[3 * x + 2 + (3 * y + 1) * width * 3] = e5;

					scaled[3 * x + 3 * y * width * 3] = e6;
					scaled[3 * x + 1 + 3 * y * width * 3] = e7;
					scaled[3 * x + 2 + 3 * y * width * 3] = e8;
				}
			});

			return scaled;
		}

		/// <summary>
		/// Performs x3 upscaling by applying Scale3x algorithm.
		/// </summary>
		public static Color32[] Upscale3X(Texture2D texture)
		{
			return Upscale3X(texture.GetPixels32(), texture.width, texture.height);
		}

		/// <summary>
		/// Performs x3 downscaling. Can be used for fast downscale, but overall quality may be not good.
		/// </summary>
		public static Color32[] Downscale2X(Color32[] pixels, int width, int height, DownscaleMode mode)
		{
			var scaled = new Color32[width * height / 4];

			Parallel.For(0, width / 2, x =>
			{
				for (var y = 0; y < height / 2; y++)
				{
					var x2 = 2 * x;
					var y2 = 2 * y;
					var matrix = new[]
					{
						pixels[x2 + y2 * width], pixels[x2 + 1 + y2 * width],
						pixels[x2 + (y2 + 1) * width], pixels[x2 + 1 + (y2 + 1) * width],
					};

					scaled[x + y * width / 2] = mode == DownscaleMode.ByAvg ? DownscaleByAvg(matrix) : DownscaleByMax(matrix);
				}
			});

			return scaled;
		}

		/// <summary>
		/// Performs x8 downscaling.
		/// </summary>
		public static Color32[] Downscale8X(Color32[] pixels, int width, int height, DownscaleMode mode)
		{
			return Downscale2X(Downscale2X(Downscale2X(pixels, width, height, mode), width / 2, height / 2, mode), width / 4, height / 4, mode);
		}

		/// <summary>
		/// Can be used for fast downscaling, but overall quality is not good.
		/// </summary>
		public static Color32[] Downscale3X(Color32[] pixels, int width, int height, DownscaleMode mode)
		{
			var scaled = new Color32[width * height / 9];

			Parallel.For(0, width / 3, x =>
			{
				for (var y = 0; y < height / 3; y++)
				{
					var x3 = 3 * x;
					var y3 = 3 * y;
					var matrix = new[]
					{
						pixels[x3 + y3 * width], pixels[x3 + 1 + y3 * width], pixels[x3 + 2 + y3 * width],
						pixels[x3 + (y3 + 1) * width], pixels[x3 + 1 + (y3 + 1) * width], pixels[x3 + 2 + (y3 + 1) * width],
						pixels[x3 + (y3 + 2) * width], pixels[x3 + 1 + (y3 + 2) * width], pixels[x3 + 2 + (y3 + 2) * width]
					};

					scaled[x + y * width / 3] = mode == DownscaleMode.ByAvg ? DownscaleByAvg(matrix) : DownscaleByMax(matrix);
				}
			});

			return scaled;
		}

		private static Color32 DownscaleByAvg(Color32[] matrix)
		{
			float r = 0f, g = 0f, b = 0f, a = 0f;

			for (var i = 0; i < matrix.Length; i++)
			{
				r += matrix[i].r;
				g += matrix[i].g;
				b += matrix[i].b;
				a += matrix[i].a;
			}

			r = Mathf.RoundToInt(r / matrix.Length);
			g = Mathf.RoundToInt(g / matrix.Length);
			b = Mathf.RoundToInt(b / matrix.Length);
			a = Mathf.RoundToInt(a / matrix.Length);

			var avg = new Color32((byte) r, (byte) g, (byte) b, (byte) a);
			var nearest = matrix[0];
			var distance = Mathf.Abs(nearest.r - avg.r) + Mathf.Abs(nearest.g - avg.g) + Mathf.Abs(nearest.b - avg.b) + Mathf.Abs(nearest.a - avg.a);

			if (distance == 0) return nearest;

			for (var i = 0; i < matrix.Length; i++)
			{
				var d = Mathf.Abs(matrix[i].r - avg.r) + Mathf.Abs(matrix[i].g - avg.g) + Mathf.Abs(matrix[i].b - avg.b) + Mathf.Abs(matrix[i].a - avg.a);

				if (d >= distance) continue;

				nearest = matrix[i];
				distance = d;

				if (distance == 0) break;
			}

			return nearest;
		}

		private static Color32 DownscaleByMax(Color32[] matrix)
		{
			var max = matrix[0];
			var count = 1;
			var threshold = Mathf.CeilToInt(matrix.Length / 2f);

			for (var i = 1; i < matrix.Length; i++)
			{
				var p = matrix[i];
				var c = matrix.Count(j => j.Equals(p));

				if (c <= count) continue;

				if (c >= threshold)
				{
					max = p;
					break;
				}

				max = p;
				count = c;
			}

			return max;
		}
	}
}