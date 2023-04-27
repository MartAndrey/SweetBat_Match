using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    public List<GameObject> FruitList { get { return fruitList; } set { fruitList = value; } }

    [Tooltip("All fruit prefabs")]
    [SerializeField] List<GameObject> fruitPrefabs;
    [Tooltip("Fruit list for object pooler")]
    [SerializeField] List<GameObject> fruitList;
    [Tooltip("Amount of fruits for each prefab at start")]
    [SerializeField] int fruitSize;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        AddNewFruitToPool(fruitSize);
    }

    // Method in charge of generating the fruits and adding it to the object pooler
    void AddNewFruitToPool(int amount, bool random = false)
    {
        if (!random)
        {
            for (int i = 0; i < fruitPrefabs.Count; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    CreateFruit(i);
                }
            }
        }
        else
        {
            int rn = Random.Range(0, fruitPrefabs.Count);

            CreateFruit(rn);
        }
    }

    // Method in charge of delivering or returning a fruit from the grouper of objects.
    public GameObject GetFruitToPool(int fruit, Transform parent)
    {
        GameObject newFruit = fruitList.Find(x => x.GetComponent<Fruit>().Id == fruit && !x.activeSelf);
        if (newFruit != null)
        {
            newFruit.transform.parent = parent;
            return newFruit;
        }

        AddNewFruitToPool(1, true);

        newFruit = fruitList[fruitList.Count - 1];
        newFruit.transform.parent = parent;
        return newFruit;
    }

    void CreateFruit(int id)
    {
        GameObject fruit = Instantiate(fruitPrefabs[id], gameObject.transform);

        fruit.SetActive(false);
        fruit.GetComponent<Fruit>().Id = id;
        fruitList.Add(fruit);
    }
}