using UnityEngine;

public class HoopSpawner : MonoBehaviour
{
    [SerializeField] GameObject hoopPrefab;
    [SerializeField] GameObject[] hoopPrefabs;
    [SerializeField] HoopData[] hoopData;
    [SerializeField] Transform playerTransform;

    private GameObject hoop;
    public void SpawnHoop()
    {

        int randomHoopIndex = ChooseRandomIndex();
        HoopData data = hoopData[randomHoopIndex];


        Vector3 playerPos = playerTransform.position;
        //get rand position
        float xPos = Random.Range(playerPos.x + data.xMinLimit, playerPos.x + data.xMaxLimit);
        float yPos = Random.Range(playerPos.y + data.yMinLimit, playerPos.y + data.yMaxLimit);

        if (data.hasRotation)
        {
            float randomRotation = Random.Range(data.minRotation, data.maxRotation);  
            hoop = Instantiate(hoopPrefabs[randomHoopIndex], new Vector3(xPos, yPos, 0), Quaternion.Euler(0, 0, 0));
            hoop.transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 0, randomRotation);
        }
        else
        {
            hoop = Instantiate(hoopPrefabs[randomHoopIndex], new Vector3(xPos, yPos, 0), Quaternion.Euler(0, 0, 0));
        }

        if (data.hasScale)
        {
            float randomScale = Random.Range(data.minScale, data.maxScale);
            hoop.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }
    }

    private int ChooseRandomIndex()
    {
     /*  order 0- Basic hoop ||| 40%
         1,2- Left,Right wall hoop ||| 20%
         3- Moving hoop ||| 20%
         4- RightBounce hoop ||| 10%
         5- Upsidedown Bounce hoop ||| 10%
     */

        float probability = Random.value;

        if(probability <= 0.4)
        {
            return 0;
        }
        else if(probability <= 0.6)
        {
            return Random.Range(1, 3);
        }
        else if(probability <= 0.8)
        {
            return 3;
        }
        else
        {
            return Random.Range(4, 6);
        }

    }
}
