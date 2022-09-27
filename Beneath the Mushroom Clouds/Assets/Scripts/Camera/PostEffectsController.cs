using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostEffectsController : MonoBehaviour
{
    public Shader postShader;
    Material postEffectMaterial;
    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if(postEffectMaterial == null)
        {
            postEffectMaterial = new Material(postShader);
        }


        

        int width = src.width;
        int height = src.height;

        //1st Blit
        RenderTexture firstBlurTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
        Graphics.Blit(src, firstBlurTexture, postEffectMaterial, 0);
        RenderTexture secondBlurTexture = firstBlurTexture;

        //2nd Blit
        width /= 2;
        height /= 2;
        firstBlurTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
        Graphics.Blit(secondBlurTexture, firstBlurTexture, postEffectMaterial, 0);
        RenderTexture.ReleaseTemporary(secondBlurTexture);
        secondBlurTexture = firstBlurTexture;

        
        width *= 2;
        height *= 2;
        firstBlurTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
        Graphics.Blit(secondBlurTexture, firstBlurTexture, postEffectMaterial, 0);
        RenderTexture.ReleaseTemporary(secondBlurTexture);
        secondBlurTexture = firstBlurTexture;






        Graphics.Blit(secondBlurTexture, dst);
        RenderTexture.ReleaseTemporary(secondBlurTexture);



    }
}
