using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuassianBlurEffect : MonoBehaviour
{


    // ぼかし回数。数値が大きいほど、ぼかしの度合いが高くなります
    [Range(0, 4)]
    public int iterations = 3;

    // ぼかし範囲
    [Range(0.2f, 3.0f)]
    public float blurSpread = 0.6f;

    [Range(1, 8)]
    public int downSample = 2;

    // シェーダ
    public Shader curShader;
    private Material curMaterial;


    Material material
    {

        get
        {
            if (curMaterial == null)
            {
                curMaterial = new Material(curShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }

    // マテリアルが使用不可または非アクティブになったらマテリアルが削除される
    void OnDisable()
    {
        if (curMaterial)
        {
            // マテリアル削除
            DestroyImmediate(curMaterial);
        }
    }

    // 画面がレンダリングされた後に呼ばれる
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // シェーダが設定されていた場合は
        if (curShader != null)
        {
            // サンプリング精度を計算
            int rtW = source.width / downSample;
            int rtH = source.height / downSample;

            // バッファを取得
            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            // 画面画像をバッファに移動
            Graphics.Blit(source, buffer0);

            // ぼかす回数分
            for (int i = 0; i < iterations; i++)
            {
                material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

                // バッファ1確保
                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // 垂直チャネルのレンダリング
                Graphics.Blit(buffer0, buffer1, material, 0);

                // バッファ0開放
                RenderTexture.ReleaseTemporary(buffer0);

                // バッファ1を0にコピー
                buffer0 = buffer1;

                // 1にバッファを新しく取得
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // 水平チャネルのレンダリング
                Graphics.Blit(buffer0, buffer1, material, 1);

                // バッファ1を開放
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            // バッファ0から出力に
            Graphics.Blit(buffer0, destination);

            // バッファ開放
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            // 特殊効果なしでソーステクスチャをターゲットテクスチャに直接コピーする
            Graphics.Blit(source, destination);
        }
    }
}
