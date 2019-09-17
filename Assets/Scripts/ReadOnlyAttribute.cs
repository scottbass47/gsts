using System;
using UnityEngine;

[AttributeUsage (AttributeTargets.Field,Inherited = true)]
public class ReadOnlyAttribute : PropertyAttribute {}