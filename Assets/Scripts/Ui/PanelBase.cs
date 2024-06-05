using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase<T> : MonoBehaviour
{
    protected T manager;
    public abstract void Init(T uIManager);

    public virtual void OnActivation()
    {
        gameObject.SetActive(true);

    }
    public virtual void OnDeactivation()
    {
        gameObject.SetActive(false);
    }
}
