////////////////////////////////////////////////////////////////////////////////
//
// CRYSTAL CLEAR SOFTWARE
// Copyright 2012 Crystal Clear Software. http://ccsoft.ru
// All Rights Reserved. CCsoft Bear Shooter
// @author Osipov Stanislav lacost.20@gmail.com
//
//
// NOTICE: Crystal Soft does not allow to use, modify, or distribute this file
// for any purpose
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class TextureUtil {
	
	
	/**
	 * will create new tiled texutre from source
	 */
	public static Texture2D TileTexture(int width, int height, Texture2D sourcedTexture) {

		Texture2D TiledHPTexture =  new Texture2D(width, height);
		
		
		for(int i = 0; i < TiledHPTexture.width; i++) {
		
		  for (int n = 0; n < TiledHPTexture.height; n++)  {
		     var pixCol = sourcedTexture.GetPixel(i % sourcedTexture.width, n % sourcedTexture.height);
		     TiledHPTexture.SetPixel(i, n, pixCol); 
		  }
			
		}
		
		TiledHPTexture.Apply();
		
		return TiledHPTexture;
	}
}
