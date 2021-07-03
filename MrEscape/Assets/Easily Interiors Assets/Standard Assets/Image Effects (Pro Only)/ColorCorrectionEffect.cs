using UnityEngine;

namespace Easily_Interiors_Assets.Standard_Assets
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Ramp)")]
	public class ColorCorrectionEffect : ImageEffectBase {
		public Texture  textureRamp;

		// Called by camera to apply image effect
		void OnRenderImage (RenderTexture source, RenderTexture destination) {
			material.SetTexture ("_RampTex", textureRamp);
			Graphics.Blit (source, destination, material);
		}
	}
}
