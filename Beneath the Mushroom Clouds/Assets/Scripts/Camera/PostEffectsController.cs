//Post Effects Controller for Box Blurring attached to FOW cameras
//Based on: https://www.youtube.com/watch?v=ahplcYCmfG0
/*
NOTE:
Unused in the current version of the game. Ommited from the documentation.
*/
using UnityEngine;


[ExecuteInEditMode]
public class PostEffectsController : MonoBehaviour
{
    public Shader postShader;
    Material postEffectMaterial;
    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        //Create post effect material based on the post effect shader
        if(postEffectMaterial == null)
        {
            postEffectMaterial = new Material(postShader);
        }


        
        //Get dimensions of the source image (what the camera sees)
        int width = src.width;
        int height = src.height;

        //1st Blit
        //Create a temporary texture and store the source image into it
        //Apply blur (blur now is weak)
        RenderTexture firstBlurTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
        Graphics.Blit(src, firstBlurTexture, postEffectMaterial, 0);
        RenderTexture secondBlurTexture = firstBlurTexture;

        //2nd Blit
        //Downscale the texture and apply blur again (texture is more blurred but it's in lower resolution)
        width /= 2;
        height /= 2;
        firstBlurTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
        Graphics.Blit(secondBlurTexture, firstBlurTexture, postEffectMaterial, 0);
        RenderTexture.ReleaseTemporary(secondBlurTexture);
        secondBlurTexture = firstBlurTexture;

        //3rd Blit
        //Upscale the texture and apply blur again (texture is even more blurred and the right dimensions)
        width *= 2;
        height *= 2;
        firstBlurTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
        Graphics.Blit(secondBlurTexture, firstBlurTexture, postEffectMaterial, 0);
        RenderTexture.ReleaseTemporary(secondBlurTexture);
        secondBlurTexture = firstBlurTexture;

        //Apply the blurred texture as the output of the camera
        Graphics.Blit(secondBlurTexture, dst);
        RenderTexture.ReleaseTemporary(secondBlurTexture);



    }
}
