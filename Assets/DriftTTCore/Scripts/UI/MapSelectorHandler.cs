using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapSelectorHandler : BaseMenuHandler
{
    [Title("Map Selection Menu", TitleAlignment = TitleAlignments.Centered), 
     SerializeField] public Animator mapSelectorCanvasAnimator;
}
