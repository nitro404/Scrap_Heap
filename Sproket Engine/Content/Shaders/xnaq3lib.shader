textures/xnaq3lib/xnaq3libsky
{
	surfaceparm noimpact
	surfaceparm nolightmap

	q3map_sun	1 1 1 100 220 50
	q3map_surfacelight 120

	qer_editorimage textures/xnaq3lib/clouds2.tga
	skyparms - 512 -
	{
		map textures/xnaq3lib/clouds.tga
		tcMod scale 3 2
		tcMod scroll 0.15 0.15
		depthWrite
	}
	{
		map textures/xnaq3lib/clouds2.tga
		blendFunc GL_ONE GL_ONE
		tcMod scale 3 3
		tcMod scroll 0.05 0.05
	}
}