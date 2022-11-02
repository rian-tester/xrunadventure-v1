using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation;
using System;

public class MultipleMapPlacement : MonoBehaviour
{
    //[SerializeField] AbstractMap map;
    [SerializeField] Transform parent;
    [SerializeField] GameObject prefab;
    [SerializeField] LayerMask mapLayer;

    MultipleCoinPlacement.ServerCoinData serverCoinData;

    public void PopulateMap(MultipleCoinPlacement.ServerCoinData serverData)
    {
        serverCoinData = serverData;
        
        if (parent.childCount > 0)
        {
            foreach (Transform go in parent)
            {
                Destroy(go.gameObject);
            }
        }

        if (serverCoinData == null) return;

        foreach(CoinData coin in serverCoinData.data)
        {
            double latitude = Convert.ToDouble(coin.Lat) ;
            double longitude= Convert.ToDouble(coin.Lng) ;
            //var pos = map.GeoToWorldPosition(new Mapbox.Utils.Vector2d(latitude, longitude), true);
            //var worldPos = new Vector2(pos.x, pos.z);

            var placedGO = Instantiate(prefab, parent);
            //placedGO.transform.position = worldPos;
        }
    }
}
