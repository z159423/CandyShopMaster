using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillerSet : MonoBehaviour
{
    public void deactiveOtherPiller(Piller piller)
    {
        foreach (var _piller in GetComponentsInChildren<Piller>())
        {
            if (_piller != piller)
            {
                _piller.enableActive = false;
                return;
            }
        }

    }
}
