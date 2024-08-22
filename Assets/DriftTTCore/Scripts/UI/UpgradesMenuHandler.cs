using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UpgradesMenuHandler : BaseMenuHandler
{
    [Title("Vehicle Upgrade Menu", TitleAlignment = TitleAlignments.Centered), 
     SerializeField] private Animator upgradesCanvasAnimator;
    [SerializeField] private TMP_Text upgradesCurrency;
}
