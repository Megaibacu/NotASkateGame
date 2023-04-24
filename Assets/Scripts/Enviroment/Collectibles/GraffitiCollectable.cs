using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraffitiCollectable : BaseCollectible
{
    public override void ApplyCollectible()
    {
        base.ApplyCollectible();
        collectibles.graffitis++;
    }
}
