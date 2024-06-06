using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Fruts _frut;
    Fructs _fruct;
    public Fruts FrutBlock { get => _frut;}
    public Fructs Fruct {  get => _fruct;}

    public void Create(Fruts frut, Fructs fruct)
    {
        _frut = frut;
        _fruct = fruct;
    }

    public void Delete()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _fruct.gameObject.SetActive(false);
    }
}
