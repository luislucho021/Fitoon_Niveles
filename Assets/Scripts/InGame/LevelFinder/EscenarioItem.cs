using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EscenarioItem", menuName = "ScriptableObjects/EscenarioItem", order = 3)]
public class EscenarioItem : ScriptableObject
{
    public Sprite imagenEscenario;
    public string nombreEscenario;
}
