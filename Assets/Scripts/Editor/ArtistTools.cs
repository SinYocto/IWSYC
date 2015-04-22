//----------------------------------------------
// Desc: 编辑器工具
// Date: 2015-04-22
// Author: Siny
// CopyRight: Siny
// ---------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CreateTextureWindow : EditorWindow
{
	private int mWidth;
	private int mHeight;
	private Color mColor;

	[MenuItem("Yocto/Artist Tools/Create Texture Asset")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(CreateTextureWindow));
	}
	
	void OnGUI()
	{
		mWidth = EditorGUI.IntField(new Rect(0, 0, position.width, 15), "Width", mWidth);
		mHeight = EditorGUI.IntField(new Rect(0, 20, position.width, 15), "Height", mHeight);
		mColor = EditorGUI.ColorField(new Rect(0, 40, position.width, 15), "Color", mColor);

		GUILayout.Space(60);
		if (GUILayout.Button("Create"))
		{
			string saveFilePath = EditorUtility.SaveFilePanelInProject(
				"Create texture file as PNG",
				"NewTexture",
				"png",
				"Please enter the new texture file name");

			Texture2D tex = YoctoHelper.CreateTexture(mWidth, mHeight, mColor);
			YoctoHelper.SaveTextureToFile(tex, saveFilePath);
			AssetDatabase.Refresh();
		}
	}
}

// 讲选中的sprite贴图合成到一张大的multiple sprite里面，自动生成每个single sprite
public class SpriteSheetMakerWindow : EditorWindow
{
	private int mWidth;
	private int mHeight;

	[MenuItem("Yocto/Artist Tools/Generate Sprite Sheet")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(SpriteSheetMakerWindow));
	}

	void OnGUI()
	{
		mWidth = EditorGUI.IntField(new Rect(0, 0, position.width, 15), "Width", mWidth);
		mHeight = EditorGUI.IntField(new Rect(0, 20, position.width, 15), "Height", mHeight);

		GUILayout.Space(50);
		if (GUILayout.Button("Generate"))
		{
			// 先创建一个sprite sheet贴图文件
			Texture2D spriteSheet = new Texture2D(mWidth, mHeight, TextureFormat.ARGB32, false);
			YoctoHelper.SetTextureColor(spriteSheet, Color.clear);

			// 讲选中的texture都合成写入到创建好的sprite sheet中
			List<SpriteMetaData> spriteList = new List<SpriteMetaData>();

			int curX = 0;	//当前可进行像素填充的坐标
			int curY = 0;

			int curLineMaxHeight = 0;

			foreach (Object obj in Selection.objects)
			{
				Texture2D tex = (Texture2D)obj;

				if (tex != null)
				{
					string assetPath = AssetDatabase.GetAssetPath(obj);
					TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);

					bool saveIsReadable = importer.isReadable;
					TextureImporterFormat saveFormat = importer.textureFormat;

					importer.isReadable = true;
					importer.textureFormat = TextureImporterFormat.ARGB32;

					importer.SaveAndReimport();

					if (tex.width > spriteSheet.width)
						continue;

					if (curX + tex.width > spriteSheet.width)	// 换行
					{
						curX = 0;
						curY += curLineMaxHeight;
						curLineMaxHeight = 0;
					}

					if (curY + tex.height > spriteSheet.height)	// 超出高度
						continue;

					Color[] cols = tex.GetPixels();
					spriteSheet.SetPixels(curX, curY, tex.width, tex.height, cols);

					// 记录当前sprite在sprite sheet中的信息
					SpriteMetaData sprite = new SpriteMetaData();
					sprite.name = tex.name;
					sprite.rect = new Rect(curX, curY, tex.width, tex.height);
					sprite.pivot = new Vector2(0.5f, 0.5f);
					spriteList.Add(sprite);

					curX += tex.width;

					if (tex.height > curLineMaxHeight)
						curLineMaxHeight = tex.height;

					importer.isReadable = saveIsReadable;
					importer.textureFormat = saveFormat;
					
					importer.SaveAndReimport();
				}
			}

			spriteSheet.Apply();

			// 保存sprite sheet贴图到文件并在导入设置中关联好子sprite信息
			string saveFilePath = EditorUtility.SaveFilePanelInProject(
				"Create texture file as PNG",
				"NewTexture",
				"png",
				"Please enter the new texture file name");
			YoctoHelper.SaveTextureToFile(spriteSheet, saveFilePath);
			AssetDatabase.Refresh();

			TextureImporter atlasImporter = (TextureImporter)TextureImporter.GetAtPath(saveFilePath);
			atlasImporter.isReadable = false;
			atlasImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
			atlasImporter.textureType = TextureImporterType.Sprite;
			atlasImporter.spriteImportMode = SpriteImportMode.Multiple;
			atlasImporter.spritesheet = spriteList.ToArray();
			atlasImporter.SaveAndReimport();
		}
	}
}

