using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Fruts _frut;
    float _cost;
    Fructs _fruct;
    public Fruts FrutBlock { get => _frut;}
    public Fructs Fruct {  get => _fruct;}
    public float Cost { get => _cost; }

    public void Create(Fruts frut,float cost, Fructs fruct)
    {
        _frut = frut;
        _fruct = fruct;
        _cost = cost;
    }

    public void Delete()
    {
        _fruct.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

}
