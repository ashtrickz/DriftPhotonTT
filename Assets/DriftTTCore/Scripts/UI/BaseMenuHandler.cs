using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseMenuHandler : MonoBehaviour
{

    [SerializeField] protected Animator MenuAnimator;
    [SerializeField] protected MenuManager MenuManager;


    public virtual void ManageMenu(MenuManager menuManager)
    {
        MenuManager = menuManager;
    }

    public virtual void Enter()
    {
        EnableMenu();
        FadeIn();
    }

    public virtual void Exit()
    {
        FadeOut();
    }
    
    public virtual void FadeIn() => MenuAnimator.Play("FadeIn");
    public virtual void FadeOut() => MenuAnimator.Play("FadeOut");
    
    public virtual void EnableMenu() => gameObject.SetActive(true);
    public virtual void DisableMenu() => gameObject.SetActive(false);
}
