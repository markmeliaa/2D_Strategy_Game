using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMap : MonoBehaviour
{
    [SerializeField] public GameObject[] maps;

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, maps.Length);
        maps[rand].SetActive(true);
    }
}
