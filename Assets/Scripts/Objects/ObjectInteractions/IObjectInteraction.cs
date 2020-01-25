using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectInteraction
{
    // Action triggered by the Human player's Action button
    void HumanAction(Human character);

    // Action triggered by the Ghost player's Action button
    void GhostAction(Ghost character);
}
