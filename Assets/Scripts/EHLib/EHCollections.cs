using System;

public class EHPriorityQueue<T> where T : IComparable
{
    private const int INIT_SIZE = 8;
    private const int SIZE_INCREASE_MULTIPLE = 2;
    private T[] Arr;
    private int Size;
    private bool IsMinHeap;

    public EHPriorityQueue() : this(false)
    {
        
    }

    public EHPriorityQueue(bool IsMinHeap)
    {
        this.IsMinHeap = IsMinHeap;
        Arr = new T[INIT_SIZE];
    }

    public bool IsEmpty()
    {
        return Size <= 0;
    }

    public void Push(T ElementToAdd)
    {
        ++Size;
        if (Size >= Arr.Length)
        {
            T[] TempArr = new T[Arr.Length * SIZE_INCREASE_MULTIPLE];
            for (int i = 0; i < Arr.Length; ++i)
            {
                TempArr[i] = Arr[i];
            }
            Arr = TempArr;
        }

        Arr[Size] = ElementToAdd;
        if (IsMinHeap) Min_BubbleUp(Size);
        else Max_BubbleUp(Size);
    }

    public T Pop()
    {
        if (IsEmpty())
        {
            return default(T);
        }
        T Element = Arr[1];
        Arr[1] = Arr[Size];
        Arr[Size] = default(T);

        --Size;
        if (IsMinHeap) Min_Heapify(1);
        else Max_Heapify(1);
        return Element;
    }

    public T Peek()
    {
        if (IsEmpty())
        {
            return default(T);
        }
        else
        {
            return Arr[1];
        }
    }

    public void Clear()
    {
        for (int i = 0; i <= Size; ++i)
        {
            Arr[i] = default(T);
        }
        Size = 0;
    }

    protected void Max_Heapify(int ElementIndex)
    {
        int Left = ElementIndex * 2;
        int Right = ElementIndex * 2 + 1;
        int LargestIndex = ElementIndex;

        if (Left <= Size && Arr[Left].CompareTo(Arr[LargestIndex]) > 0)        {
            LargestIndex = Left;
        }
        if (Right <= Size && Arr[Right].CompareTo(Arr[LargestIndex]) > 0)
        {
            LargestIndex = Right;
        }
        
        if (LargestIndex != ElementIndex)
        {
            Swap(ElementIndex, LargestIndex);
            Max_Heapify(LargestIndex);
        }
    }

    protected void Min_Heapify(int ElementIndex)
    {
        int Left = ElementIndex * 2;
        int Right = ElementIndex * 2 + 1;
        int LargestIndex = ElementIndex;

        if (Left <= Size && Arr[Left].CompareTo(Arr[LargestIndex]) < 0)
        {
            LargestIndex = Left;
        }
        if (Right <= Size && Arr[Right].CompareTo(Arr[LargestIndex]) < 0)
        {
            LargestIndex = Right;
        }

        if (LargestIndex != ElementIndex)
        {
            Swap(ElementIndex, LargestIndex);
            Max_Heapify(LargestIndex);
        }
    }

    protected void Max_BubbleUp(int ElementIndex)
    {
        if (ElementIndex / 2 == 0)
        {
            return;
        }

        if (Arr[ElementIndex].CompareTo(Arr[ElementIndex / 2]) > 0)
        {
            Swap(ElementIndex, ElementIndex / 2);
            Max_BubbleUp(ElementIndex / 2);
        }
    }

    protected void Min_BubbleUp(int ElementIndex)
    {
        if (ElementIndex / 2 == 0)
        {
            return;
        }

        if (Arr[ElementIndex].CompareTo(Arr[ElementIndex / 2]) < 0)
        {
            Swap(ElementIndex, ElementIndex / 2);
            Max_BubbleUp(ElementIndex / 2);
        }
    }

    private void Swap(int Index1, int Index2)
    {
        T Placeholder = Arr[Index1];
        Arr[Index1] = Arr[Index2];
        Arr[Index2] = Placeholder;
    }
}