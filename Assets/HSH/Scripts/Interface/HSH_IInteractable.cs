using UnityEngine;

public interface HSH_IInteractable<T> where T : class
{
    public void Interact(T someClass);
}
