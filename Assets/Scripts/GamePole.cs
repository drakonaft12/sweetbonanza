
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GamePole : MonoBehaviour
{
    [SerializeField] Vector2 size;
    [SerializeField] Vector2 foot = new Vector2(150, 150);
    [SerializeField] Spawner spawner;
    [SerializeField] Sprite[] sprites;
    private List<Block>[] blocks;

    bool isMove = true;
    bool TaskVork = true;

    private int[] typeBlocks;
    private void Awake()
    {
        typeBlocks = new int[3];
        blocks = new List<Block>[(int)size.x];
        for (int i = 0; i < (int)size.x; i++)
        {
            blocks[i] = new List<Block>();
        }
    }
    private void Start()
    {
        Create();
        TaskUpdate();
        FindUpdate();
    }

    private void OnDisable()
    {
        TaskVork = false;
    }

    public void Create()
    {
        TaskVork = true;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Spawn(x, y);
            }
        }
    }

    private void Spawn(int x, int y)
    {
        int i = Random.Range(0, 3);
        var item = spawner.Spawn(0, (Vector2)transform.position);
        item.transform.position += (Vector3)new Vector2(x * foot.x - (size.x - 1) * foot.x / 2, y * foot.y - (size.y - 1) * foot.y / 2);
        item.transform.SetParent(transform);

        blocks[x].Add(item.GetComponent<Block>());
        item = spawner.Spawn(1, (Vector2)blocks[x][y].transform.position + Vector2.up * foot * y + Vector2.up * foot * size.y);
        item.transform.SetParent(transform);
        var fr = item.GetComponent<Fructs>();
        fr.Create(sprites[i], blocks[x][y].transform);
        blocks[x][y].Create((Fruts)i, fr);
        typeBlocks[i]++;

    }

    private async void TaskUpdate()
    {
        while (TaskVork)
        {
            await Gravitation();

            await Task.Delay(10);
        }
    }

    private async void FindUpdate()
    {
        while (TaskVork)
        {
            await FindCombination();
        }
    }

    private void Update()
    {
        if (TaskVork)
        for (int x = 0; x < size.x; x++)
        {
            if (blocks[x].Count < size.y)
            {
                Spawn(x, (blocks[x].Count));
            }
        }
    }

    private async Task FindCombination()
    {
        for (int i = 0; i < typeBlocks.Length; i++)
        {
            await Task.Delay(100);
            if (isMove)
            {
                await Task.Delay(100);
                break;
            }
            if (typeBlocks[i] > 7)
            {

                FindCombination(i);
                List<Vector2> cordinateCombination = new List<Vector2>();

                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < blocks[x].Count; y++)
                    {
                        if ((int)blocks[x][y].FrutBlock == i)
                        {
                            cordinateCombination.Add(new Vector2(x, y));
                            StartCoroutine(blocks[x][y].Fruct.Destroy());
                        }
                    }
                }
                await Task.Delay(2000);
                Debug.Log(AddPoints(cordinateCombination));
                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < blocks[x].Count; y++)
                    {
                        if ((int)blocks[x][y].FrutBlock == i)
                        {
                            blocks[x][y].gameObject.SetActive(false);
                        }
                    }
                }
            }
            await Task.Delay(200);


        }

    }

    private int AddPoints(List<Vector2> cordinateCombination)
    {
        int points = 0;
        Vector2 r = new Vector2(-2, -2);
        for (int i = 0; i < cordinateCombination.Count; i++)
        {
            for (int j = 0; j < i; j++)
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

            }
        }
        return points;
    }


    private void FindCombination(int i)
    {
        Debug.Log($"You find {(Fruts)i}");
    }

    private async Task Gravitation()
    {
        isMove = true;
        await Task.Delay(10);
        isMove = false;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {

                if (!isMove)
                    isMove = blocks[x][y].Fruct.isMoving;
            }
        }
        for (int x = 0; x < size.x; x++)
        {
            for (int y = blocks[x].Count - 1; y >= 0; y--)
            {

                if (!blocks[x][y].gameObject.activeSelf)
                {
                    typeBlocks[(int)blocks[x][y].FrutBlock]--;
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
        }
        await Task.Delay(2000);
        typeBlocks = new int[3];
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
    Виноград = 2
}
