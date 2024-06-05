
using System;

public interface IDraggable 
{
    void DraggingStart();
    void DraggingEnd(Action endCallBack);
    bool CanDrag();
}
