using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteCollectible : BaseCollectible
{
    public override void ApplyCollectible()
    {
        base.ApplyCollectible();
        collectibles.cassettes++;
    }
}
