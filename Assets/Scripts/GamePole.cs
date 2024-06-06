
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

    bool isMove = false;

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
    }

    public void Create()
    {

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
        item = spawner.Spawn(1, (Vector2)blocks[x][y].transform.position + Vector2.up * foot * y);
        item.transform.SetParent(transform);
        var fr = item.GetComponent<Fructs>();
        fr.Create(sprites[i], blocks[x][y].transform);
        blocks[x][y].Create((Fruts)i, fr);
        typeBlocks[i]++;

    }

    private async void TaskUpdate()
    {
        while (true)
        {
            await Gravitation();
            await Task.Delay(10);
        }
    }

    private void Update()
    {
        FindCombination();
    }

    private void FindCombination()
    {
        isMove = false;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (!isMove)
                    isMove = blocks[x][y].Fruct.isMoving;
            }
        }
        Debug.Log(isMove);
        for (int i = 0; i < typeBlocks.Length; i++)
        {
            if (isMove)
            {
                break;
            }
            if (typeBlocks[i] > 7)
            {
                Debug.Log($"You find {(Fruts)i}");
                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        if ((int)blocks[x][y].FrutBlock == i)
                        {
                            blocks[x][y].gameObject.SetActive(false);
                            Gravitation();
                        }
                    }
                }

            }

        }
    }

    private async Task Gravitation()
    {

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {

                if (!blocks[x][y].gameObject.activeSelf)
                {
                    typeBlocks[(int)blocks[x][y].FrutBlock]--;
                    Spawn(x, (int)size.y);
                    blocks[x].Remove(blocks[x][y]);
                    UpdateLine(x, y);
                }

            }
        }

        await Task.Delay(10);
    }

    private void UpdateLine(int x, int beginY)
    {
        for (int y = beginY; y < size.y; y++)
        {
            blocks[x][y].transform.position = transform.position + (Vector3)new Vector2(x * foot.x - (size.x - 1) * foot.x / 2, y * foot.y - (size.y - 1) * foot.y / 2);
        }
    }


}
public enum Fruts : int
{
    Арбуз = 0,
    Банан = 1,
    Виноград = 2
}
