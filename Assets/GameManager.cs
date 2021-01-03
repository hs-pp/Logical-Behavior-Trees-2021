using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject CardPrefab;
    public int imanumber;
    public string imaword;

    public void Awake()
    {

    }
    public void Start()
    {

    }
    public void OnEnable()
    {

    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            CreateCard();
        }
    }
    public void OnDisable()
    {

    }
    public void OnDestroy()
    {

    }

    private void CreateCard()
    {
        GameObject newCard = GameObject.Instantiate(CardPrefab);
        newCard.transform.position = new Vector3(Random.Range(-10 , 10), Random.Range(-10, 10), Random.Range(-10, 10));
        newCard.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)));
    }
}
