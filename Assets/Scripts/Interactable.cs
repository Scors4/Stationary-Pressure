using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public void OnInteraction(GameObject other);

    public void OnHover(GameObject other);
}
