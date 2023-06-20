using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSort : MonoBehaviour
{
    public int num = 10;
    public float distance = 1f;
    public float startX = -5f;
    public float startY = -5f;
    public GameObject prefab;

    public float[] array;
    public GameObject[] objects;

    void Init()
    {
        array = new float[num];
        objects = new GameObject[num];

        for(int i = 0; i < num; i++)
        {
            float h = Random.Range(1, 10);
            array[i] = h;

            GameObject go = Instantiate(prefab, new Vector3(startX + i * distance, startY + h / 2, 0), Quaternion.identity);
            go.transform.localScale = new Vector3(1, h, 1);
            go.SetActive(true);
            objects[i] = go;
        }
    }

    IEnumerator AssignOperation(int src, int dst)
    {
        GameObject go = objects[src];
        if(dst < 0)
        {

        }
    }

    IEnumerator InsertSort()
    {
        for(int i = 1; i < num; i++)
        {
            float h = array[i];
            yield return StartCoroutine(AssignOperation(i, -1));
            int j = i - 1;
            while(j >= 0 && array[j] > h)
            {
                array[j + 1] = array[j];
                j = j - 1;
            }
            array[j + 1] = h;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
