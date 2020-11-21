using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITickableComponent
{
    void Tick(float DeltaTime);
}
