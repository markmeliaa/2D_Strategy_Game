using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMap : MonoBehaviour
{
    [SerializeField] public GameObject[] maps;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        int rand = Random.RandomRange(0, maps.Length);
        maps[rand].SetActive(true);
    }
}
