using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IDataSource : IDisposable {


    TileBuffer ReadForward();
    TileBuffer ReadBackward();
    void Reset();

}
