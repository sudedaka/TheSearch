using UnityEngine;

public class GameStartObjective : MonoBehaviour
{
    void Start()
    {
        var ui = FindObjectOfType<ObjectiveUI>();
        ui.SetObjective("Find your dad's letter.");
}
