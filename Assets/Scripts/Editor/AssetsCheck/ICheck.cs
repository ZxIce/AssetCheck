using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public interface ICheck
{
    string SearchTag {get;　set;}
    bool CanFix {get;　set;}
    bool Check(string path);
    bool Fix(string path);
}
