using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeapItem<T> : System.IComparable<T>
{
    int HeapIndex { get; set; }
}
