using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PageTest : PageBase
{

    private async void Start()
    {

        await StartScreen();
        await CloseScreen();

    }




}
