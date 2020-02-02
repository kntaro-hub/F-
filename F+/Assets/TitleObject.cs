using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleObject : MonoBehaviour
{
    // =--------- // =---------- シリアライズ ----------= // ---------= //

    // タイトルシーンに配置されるオブジェクトプレハブ
    [SerializeField, Tooltip("タイトルシーンに配置されるオブジェクトプレハブ")]
    private MapChip_Wall[] mapChipPrefabs;

    // タイトルシーンに配置される修飾用オブジェクト
    [SerializeField, Tooltip("タイトルシーンに配置される修飾用オブジェクト")]
    private MapObject[] mapDecorations;

    // 生成されたオブジェクトの上に修飾用オブジェクトが生成される確率
    [SerializeField, Tooltip("生成されたオブジェクトの上に修飾用オブジェクトが生成される確率(%,整数型)")]
    private int per_Decoration = 0;

    // タイトルシーンのマップの広さ
    [SerializeField, Tooltip("タイトルシーンのマップの広さ")]
    private int breadth = 1;

    // 修飾用オブジェクトの最大拡大率（1倍～？倍）
    [SerializeField, Tooltip("修飾用オブジェクトの最大拡大率（1倍～？倍）")]
    private int maxScale = 1;

    // =--------- // =---------- 変数宣言 ----------= // ---------= //

    // 生成したオブジェクトリスト
    private List<MapChip_Wall> mapObjectList = new List<MapChip_Wall>();

    // 生成した修飾用オブジェクトリスト
    private  List<MapObject> mapDecorationList = new List<MapObject>();

    // Start is called before the first frame update
    void Start()
    {
        // 指定した数x指定した数
        for(int i = 0; i < breadth; ++i)
        {
            for(int j = 0; j < breadth; ++j)
            {
                // マップチップ生成
                MapChip_Wall mapChip = Instantiate(
                        mapChipPrefabs[Random.Range(0, mapChipPrefabs.Length)],
                        this.transform.position + new Vector3(i * 1.0f, 0.0f, j * 1.0f),
                        Quaternion.identity,
                        this.transform);

                // リストに追加
                mapObjectList.Add(mapChip);

                // per_Decoration%の確立で上に修飾用オブジェクトが生成される
                //if (Percent.Per(per_Decoration))
                //{ 
                //    // 就職用オブジェクトをランダムで選択
                //    int decoNum = Random.Range(0, mapDecorations.Length);

                //    // 選んだオブジェクトを生成
                //    MapObject deco = Instantiate(
                //        mapDecorations[decoNum],
                //        new Vector3( mapChip.transform.position.x,
                //                            mapChip.transform.position.y + 0.5f, 
                //                            mapChip.transform.position.z),
                //        Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f),
                //        this.transform);

                //    // リストに追加
                //    mapDecorationList.Add(deco);

                //    // 拡大率をランダムに設定
                //    float scale = Random.Range(1.0f, maxScale);
                //    deco.transform.localScale = new Vector3(scale, scale, scale);
                //}
            }
        }
    }
}
