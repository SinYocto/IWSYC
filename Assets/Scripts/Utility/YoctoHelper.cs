//----------------------------------------------
// Desc: 辅助方法类
// Date: 2015-04-22
// Author: Siny
// CopyRight: Siny
// ---------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;

public static class YoctoHelper 
{
	public static Texture2D CreateTexture(int width, int height, Color color)
	{
		Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
		
		Color[] cols = new Color[width * height];
		for(int i = 0; i < cols.Length; ++i)
		{
			cols[i] = color;
		}
		
		tex.SetPixels(cols);
		tex.Apply();
		
		return tex;
	}
	
	// 保存Texture2D为png文件
	public static void SaveTextureToFile(Texture2D tex, string saveFilePath)
	{
		byte[] bytes = tex.EncodeToPNG();
		File.WriteAllBytes(saveFilePath, bytes);
	}
	
	public static void SetTextureColor(Texture2D tex, Color col)
	{
		if (tex == null)
			return;
		
		Color[] cols = new Color[tex.width * tex.height];
		for (int i = 0; i < cols.Length; ++i)
		{
			cols[i] = col;
		}
		
		tex.SetPixels(cols);
	}

	public static int Layer(string layerName)
	{
		return 1 << LayerMask.NameToLayer(layerName);
	}
}
