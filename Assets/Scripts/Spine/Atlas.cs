using System;
using System.Collections.Generic;
using System.IO;

namespace Spine
{
	public class Atlas
	{
		private List<AtlasPage> pages = new List<AtlasPage>();

		private List<AtlasRegion> regions = new List<AtlasRegion>();

		private TextureLoader textureLoader;

		public Atlas(TextReader reader, string dir, TextureLoader textureLoader)
		{
			Load(reader, dir, textureLoader);
		}

		public Atlas(List<AtlasPage> pages, List<AtlasRegion> regions)
		{
			this.pages = pages;
			this.regions = regions;
			textureLoader = null;
		}

		private void Load(TextReader reader, string imagesDir, TextureLoader textureLoader)
		{
			if (textureLoader == null)
			{
				throw new ArgumentNullException("textureLoader cannot be null.");
			}
			this.textureLoader = textureLoader;
			string[] array = new string[4];
			AtlasPage atlasPage = null;
			while (true)
			{
				string text = reader.ReadLine();
				if (text == null)
				{
					break;
				}
				if (text.Trim().Length == 0)
				{
					atlasPage = null;
					continue;
				}
				if (atlasPage == null)
				{
					atlasPage = new AtlasPage();
					atlasPage.name = text;
					if (ReadTuple(reader, array) == 2)
					{
						atlasPage.width = int.Parse(array[0]);
						atlasPage.height = int.Parse(array[1]);
						ReadTuple(reader, array);
					}
					atlasPage.format = (Format)Enum.Parse(typeof(Format), array[0], ignoreCase: false);
					ReadTuple(reader, array);
					atlasPage.minFilter = (TextureFilter)Enum.Parse(typeof(TextureFilter), array[0], ignoreCase: false);
					atlasPage.magFilter = (TextureFilter)Enum.Parse(typeof(TextureFilter), array[1], ignoreCase: false);
					string a = ReadValue(reader);
					atlasPage.uWrap = TextureWrap.ClampToEdge;
					atlasPage.vWrap = TextureWrap.ClampToEdge;
					if (a == "x")
					{
						atlasPage.uWrap = TextureWrap.Repeat;
					}
					else if (a == "y")
					{
						atlasPage.vWrap = TextureWrap.Repeat;
					}
					else if (a == "xy")
					{
						atlasPage.uWrap = (atlasPage.vWrap = TextureWrap.Repeat);
					}
					textureLoader.Load(atlasPage, Path.Combine(imagesDir, text));
					pages.Add(atlasPage);
					continue;
				}
				AtlasRegion atlasRegion = new AtlasRegion();
				atlasRegion.name = text;
				atlasRegion.page = atlasPage;
				atlasRegion.rotate = bool.Parse(ReadValue(reader));
				ReadTuple(reader, array);
				int num = int.Parse(array[0]);
				int num2 = int.Parse(array[1]);
				ReadTuple(reader, array);
				int num3 = int.Parse(array[0]);
				int num4 = int.Parse(array[1]);
				atlasRegion.u = (float)num / (float)atlasPage.width;
				atlasRegion.v = (float)num2 / (float)atlasPage.height;
				if (atlasRegion.rotate)
				{
					atlasRegion.u2 = (float)(num + num4) / (float)atlasPage.width;
					atlasRegion.v2 = (float)(num2 + num3) / (float)atlasPage.height;
				}
				else
				{
					atlasRegion.u2 = (float)(num + num3) / (float)atlasPage.width;
					atlasRegion.v2 = (float)(num2 + num4) / (float)atlasPage.height;
				}
				atlasRegion.x = num;
				atlasRegion.y = num2;
				atlasRegion.width = Math.Abs(num3);
				atlasRegion.height = Math.Abs(num4);
				if (ReadTuple(reader, array) == 4)
				{
					atlasRegion.splits = new int[4]
					{
						int.Parse(array[0]),
						int.Parse(array[1]),
						int.Parse(array[2]),
						int.Parse(array[3])
					};
					if (ReadTuple(reader, array) == 4)
					{
						atlasRegion.pads = new int[4]
						{
							int.Parse(array[0]),
							int.Parse(array[1]),
							int.Parse(array[2]),
							int.Parse(array[3])
						};
						ReadTuple(reader, array);
					}
				}
				atlasRegion.originalWidth = int.Parse(array[0]);
				atlasRegion.originalHeight = int.Parse(array[1]);
				ReadTuple(reader, array);
				atlasRegion.offsetX = int.Parse(array[0]);
				atlasRegion.offsetY = int.Parse(array[1]);
				atlasRegion.index = int.Parse(ReadValue(reader));
				regions.Add(atlasRegion);
			}
		}

		private static string ReadValue(TextReader reader)
		{
			string text = reader.ReadLine();
			int num = text.IndexOf(':');
			if (num == -1)
			{
				throw new Exception("Invalid line: " + text);
			}
			return text.Substring(num + 1).Trim();
		}

		private static int ReadTuple(TextReader reader, string[] tuple)
		{
			string text = reader.ReadLine();
			int num = text.IndexOf(':');
			if (num == -1)
			{
				throw new Exception("Invalid line: " + text);
			}
			int i = 0;
			int num2 = num + 1;
			for (; i < 3; i++)
			{
				int num3 = text.IndexOf(',', num2);
				if (num3 == -1)
				{
					break;
				}
				tuple[i] = text.Substring(num2, num3 - num2).Trim();
				num2 = num3 + 1;
			}
			tuple[i] = text.Substring(num2).Trim();
			return i + 1;
		}

		public void FlipV()
		{
			int i = 0;
			for (int count = regions.Count; i < count; i++)
			{
				AtlasRegion atlasRegion = regions[i];
				atlasRegion.v = 1f - atlasRegion.v;
				atlasRegion.v2 = 1f - atlasRegion.v2;
			}
		}

		public AtlasRegion FindRegion(string name)
		{
			int i = 0;
			for (int count = regions.Count; i < count; i++)
			{
				if (regions[i].name == name)
				{
					return regions[i];
				}
			}
			return null;
		}

		public void Dispose()
		{
			if (textureLoader != null)
			{
				int i = 0;
				for (int count = pages.Count; i < count; i++)
				{
					textureLoader.Unload(pages[i].rendererObject);
				}
			}
		}
	}
}
