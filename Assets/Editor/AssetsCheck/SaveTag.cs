using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public class SaveTag : Attribute
{
    public SaveTag(string Title)
    {
        this.title = Title;
        this.title = Title;
    }

    protected string title;
    public string Title
    {
        get
        {
            return this.title;
        }
    }
}
