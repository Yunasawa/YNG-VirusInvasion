using UnityEngine;

[System.Serializable]
public class Group<T1, T2>
{
    public T1 E1;
    public T2 E2;

    public Group() : this(default, default) { }
    public Group(T1 e1, T2 e2) { E1 = e1; E2 = e2; }
}

[System.Serializable]
public class Group<T1, T2, T3>
{
    public T1 E1;
    public T2 E2;
    public T3 E3;

    public Group() : this(default, default, default) { }
    public Group(T1 e1, T2 e2, T3 e3) { E1 = e1; E2 = e2; E3 = e3; }
}

[System.Serializable]
public class Group<T1, T2, T3, T4>
{
    public T1 E1;
    public T2 E2;
    public T3 E3;
    public T4 E4;

    public Group() : this(default, default, default, default) { }
    public Group(T1 e1, T2 e2, T3 e3, T4 e4) { E1 = e1; E2 = e2; E3 = e3; E4 = e4; }
}

[System.Serializable]
public class Group<T1, T2, T3, T4, T5>
{
    public T1 E1;
    public T2 E2;
    public T3 E3;
    public T4 E4;
    public T5 E5;

    public Group() : this(default, default, default, default, default) { }
    public Group(T1 e1, T2 e2, T3 e3, T4 e4, T5 e5) { E1 = e1; E2 = e2; E3 = e3; E4 = e4; E5 = e5; }
}
