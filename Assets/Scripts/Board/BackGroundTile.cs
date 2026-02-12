using UnityEngine;

public class BackGroundTile : MonoBehaviour
{
    // 나중에 특별 타일 같은게 필요하면 작업
    // .. 
    void Start()
    {
            
    }

    void Update()
    {
        
    }

    // 타일 제거
    public void DestroyTile()
    {
        Destroy(gameObject);
    }

}
