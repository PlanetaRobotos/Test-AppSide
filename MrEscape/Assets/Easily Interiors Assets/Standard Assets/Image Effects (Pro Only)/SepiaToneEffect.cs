using UnityEngine;

namespace Easily_Interiors_Assets.Standard_Assets
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Color Adjustments/Sepia Tone")]
	public class SepiaToneEffect : ImageEffectBase {

		// Called by camera to apply image effect
		void OnRenderImage (RenderTexture source, RenderTexture destination) {
			Graphics.Blit (source, destination, material);
		}
	}
}
