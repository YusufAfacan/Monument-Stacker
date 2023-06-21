using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour
{
    public List<GameObject> bricksToBeActivated;
    public int nextBrickIndex;

    private void Awake()
    {
        nextBrickIndex = 0;
    }

    public void ActivateBlock()
    {
        bricksToBeActivated[nextBrickIndex].SetActive(true);
        nextBrickIndex++;
    }
}
