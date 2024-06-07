using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GamePole : MonoBehaviour
{
    [SerializeField] Vector2 size;
    [SerializeField] Vector2 foot = new Vector2(150, 150);
    [SerializeField] Spawner spawner;
    [SerializeField] SpriteImage[] sprites;
    private List<Block>[] blocks;
    private List<Block> blocksFromDelete;
    [SerializeField] private float stavka;
    [SerializeField] private int _valueOfFreeResets;
    [SerializeField] private float _valueOfPointCombination = 0;
    [SerializeField] private float _valueOfPoint = 0;
    [SerializeField] private float _Bank = 0;
    private int _isReset;
    private int _valueOfFruckType = 11;
    private int _otstup = 25;

    bool isMove = true;
    bool TaskVork = true;
    bool VorkButton = true;
    bool StartSpawn = true;

    private int[] typeBlocks;
    private void Awake()
    {
        typeBlocks = new int[_valueOfFruckType];
        blocks = new List<Block>[(int)size.x];
        for (int i = 0; i < (int)size.x; i++)
        {
            blocks[i] = new List<Block>();
        }
        blocksFromDelete = new List<Block>();
    }
    private void Start()
    {
        (transform as RectTransform).sizeDelta = new Vector2(size.x * foot.x, size.y * foot.y + _otstup);
        transform.position += Vector3.up * _otstup;

        Create();
        TaskUpdate();
        FindUpdate();
    }

    private void OnDisable()
    {
        TaskVork = false;
    }

    private void OnEnable()
    {
        if (!TaskVork)
        {
            TaskVork = true;
            TaskUpdate();
            FindUpdate();
        }
    }
    public async void Create()
    {
        StartSpawn = false;
        TaskVork = true;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Spawn(x, y);
            }
            await Task.Delay(100);
        }
        StartSpawn = true;
    }

    private void Spawn(int x, int y)
    {
        int i = UnityEngine.Random.Range(0, 9);
        float cost = (i + 1) * stavka;
        if (UnityEngine.Random.Range(0, 30) == 0) { i = 9; cost = 0; }
        if (UnityEngine.Random.Range(0, 50) == 0)
        {
            i = 10; cost = (UnityEngine.Random.Range(1, 3) +
                            UnityEngine.Random.Range(1, 3) +
                            UnityEngine.Random.Range(1, 3) +
                            UnityEngine.Random.Range(1, 3) +
                            UnityEngine.Random.Range(1, 3)) * 5;
        }

        var item = spawner.Spawn(0, (Vector2)transform.position);
        item.transform.position += (Vector3)new Vector2(x * foot.x - (size.x - 1) * foot.x / 2,
                                                        y * foot.y - (size.y - 1) * foot.y / 2 - _otstup);
        item.transform.SetParent(transform);

        blocks[x].Add(item.GetComponent<Block>());
        item = spawner.Spawn(1, (Vector2)blocks[x][y].transform.position + Vector2.up * foot * (y / 2 + size.y));
        item.transform.SetParent(transform);
        var fr = item.GetComponent<Fructs>();
        fr.Create(sprites[i].sprite, blocks[x][y].transform, sprites[i].size);

        blocks[x][y].Create((Fruts)i, cost, fr);
        typeBlocks[i]++;

    }

    private async void TaskUpdate()
    {
        while (TaskVork)
        {
            await Gravitation();
        }
    }

    private async void SpawnUpdate()
    {
        while (TaskVork)
        {
            await Gravitation();
        }
    }

    private async void FindUpdate()
    {
        while (TaskVork)
        {
            await FindCombination();
            if (_isReset < 10)
                _isReset++;
        }
    }

    private void Update()
    {

        if (TaskVork && StartSpawn)
            for (int x = 0; x < size.x; x++)
            {
                if (blocks[x].Count < size.y)
                {
                    Spawn(x, (blocks[x].Count));
                }
            }
        bool move = false;
        isMove = true;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < blocks[x].Count; y++)
            {

                if (!move)
                    move = blocks[x][y].Fruct.isMoving;
            }
        }
        isMove = move;
        if (_valueOfFreeResets > 0 && _isReset > 4)
        {
            DeletePole();
            _isReset = 0;
            _valueOfFreeResets--;
        }
        if (_isReset > 3)
        {
            _Bank += _valueOfPoint;
            _valueOfPoint = 0;
        }
    }

    private async Task FindCombination()
    {
        for (int t = 0; t < 2; t++)
        {
            for (int i = 0; i < typeBlocks.Length; i++)
            {
                switch (i)
                {
                    case 9:
                        await Comb4(i);
                        break;
                    case 10:
                        await Bombs(i);
                        break;
                    default:
                        await Comb8(i);
                        break;
                }

                VorkButton = true;
                await Task.Delay(10);
            }
        }
        _valueOfPoint += _valueOfPointCombination;
        _valueOfPointCombination = 0;
        StartSpawn = true;
    }

    private async Task Comb8(int i)
    {
        if (typeBlocks[i] > 7)
        {
            _isReset = 0;
            if (isMove)
            {
                await Task.Delay(10);
                return;
            }
            VorkButton = false;
            FindCombination(i);
            List<Vector2> cordinateCombination = new List<Vector2>();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < blocks[x].Count; y++)
                {
                    if ((int)blocks[x][y].FrutBlock == i)
                    {
                        StartSpawn = false;
                        cordinateCombination.Add(new Vector2(x, y));
                        StartCoroutine(blocks[x][y].Fruct.CombinationAnimationAndDisable());
                        blocksFromDelete.Add(blocks[x][y]);
                        typeBlocks[i]--;
                    }
                }
            }

            await Task.Delay(2000);

            _valueOfPointCombination += AddPoints(cordinateCombination);
            for (int x = 0; x < blocksFromDelete.Count; x++)
            {
                blocksFromDelete[x].gameObject.SetActive(false);
                isMove = true;
            }
            blocksFromDelete.Clear();
        }
    }
    private async Task Comb4(int i)
    {
        if (typeBlocks[i] > 3)
        {
            _isReset = 0;
            if (isMove)
            {
                await Task.Delay(10);
                return;
            }
            VorkButton = false;
            FindCombination(i);
            List<Vector2> cordinateCombination = new List<Vector2>();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < blocks[x].Count; y++)
                {
                    if ((int)blocks[x][y].FrutBlock == i)
                    {
                        StartSpawn = false;
                        cordinateCombination.Add(new Vector2(x, y));
                        StartCoroutine(blocks[x][y].Fruct.CombinationAnimationAndDisable());
                        blocksFromDelete.Add(blocks[x][y]);
                        typeBlocks[i]--;
                    }
                }
            }

            await Task.Delay(2000);

            Debug.Log("Add 10 free resets");
            _valueOfFreeResets += 10;
            for (int x = 0; x < blocksFromDelete.Count; x++)
            {
                blocksFromDelete[x].gameObject.SetActive(false);
                isMove = true;
            }
            blocksFromDelete.Clear();
        }

    }

    private async Task Bombs(int i)
    {
        if (typeBlocks[i] > 0 && _valueOfPointCombination != 0)
        {
            _isReset = 0;
            if (isMove)
            {
                await Task.Delay(10);
                return;
            }
            VorkButton = false;
            FindCombination(i);
            List<Vector2> cordinateCombination = new List<Vector2>();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < blocks[x].Count; y++)
                {
                    if ((int)blocks[x][y].FrutBlock == i)
                    {
                        StartSpawn = false;
                        cordinateCombination.Add(new Vector2(x, y));
                        StartCoroutine(blocks[x][y].Fruct.CombinationAnimationAndDisable());
                        blocksFromDelete.Add(blocks[x][y]);
                        typeBlocks[i]--;
                        await Task.Delay(500);
                        _valueOfPointCombination *= blocks[x][y].Cost;
                    }
                }
            }

            await Task.Delay(2000);


            for (int x = 0; x < blocksFromDelete.Count; x++)
            {
                Debug.Log("Multipl: " + blocksFromDelete[x].Cost);
                blocksFromDelete[x].gameObject.SetActive(false);
                isMove = true;
            }
            blocksFromDelete.Clear();
        }

    }

    private float AddPoints(List<Vector2> cordinateCombination)
    {
        float points = 0;
        //Vector2 r = new Vector2(-2, -2);
        for (int i = 0; i < cordinateCombination.Count; i++)
        {
            /*for (int j = 0; j < i; j++)
            {
                r = cordinateCombination[j];
                points += 1;
                if (cordinateCombination[i].y - r.y == 1 || cordinateCombination[i].y - r.y == -1)
                {
                    points++;
                }
                if (cordinateCombination[i].x - r.x == 1 || cordinateCombination[i].x - r.x == -1)
                {
                    points++;
                }
                if (cordinateCombination[i].x - r.x == 1 && cordinateCombination[i].y - r.y == 1 ||
                    cordinateCombination[i].x - r.x == -1 && cordinateCombination[i].y - r.y == 1)
                {
                    points += 2;
                }
                if (cordinateCombination[i].x - r.x == 1 && cordinateCombination[i].y - r.y == -1 ||
                    cordinateCombination[i].x - r.x == -1 && cordinateCombination[i].y - r.y == -1)
                {
                    points += 2;
                }

            }*/
            points += blocks[(int)cordinateCombination[i].x][(int)cordinateCombination[i].y].Cost;

        }
        return points;
    }


    private void FindCombination(int i)
    {
        Debug.Log($"You find {(Fruts)i}");
    }

    private async Task Gravitation()
    {
        if (!StartSpawn)
        {
            await Task.Delay(10);
            return;
        }
        for (int x = 0; x < size.x; x++)
        {
            for (int y = blocks[x].Count - 1; y >= 0; y--)
            {

                if (!blocks[x][y].gameObject.activeSelf)
                {

                    blocks[x].Remove(blocks[x][y]);
                    UpdateLine(x, y);
                }
            }
        }


        await Task.Delay(100);
    }

    private void UpdateLine(int x, int beginY)
    {
        for (int y = beginY; y < blocks[x].Count; y++)
        {
            blocks[x][y].transform.position = transform.position +
                (Vector3)new Vector2(x * foot.x - (size.x - 1) * foot.x / 2, y * foot.y - (size.y - 1) * foot.y / 2);
        }
    }

    public void DeletePole()
    {
        if (VorkButton)
            DeletePoleAsync();
    }
    public async void DeletePoleAsync()
    {
        TaskVork = false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = blocks[x].Count - 1; y >= 0; y--)
            {
                {
                    StartCoroutine(blocks[x][y].Fruct.Reset());
                    blocks[x][y].gameObject.transform.position += Vector3.down * 1000;
                    blocks[x][y].gameObject.SetActive(false);
                    blocks[x].Remove(blocks[x][y]);
                }
            }
            await Task.Delay(100);
        }
        await Task.Delay(250);
        typeBlocks = new int[_valueOfFruckType];
        Create();
        await Task.Delay(1000);
        TaskUpdate();
        FindUpdate();
    }
}
public enum Fruts : int
{
    Арбуз = 0,
    Банан = 1,
    Виноград = 2,
    Слива = 3,
    Яблоко = 4,
    Зелёная = 5,
    Синяя = 6,
    Фиолетовая = 7,
    Сердце = 8,
    Леденец = 9,
    Бомба = 10
}

[Serializable]
public class SpriteImage
{
    public Sprite sprite;
    public Vector2 size;
}