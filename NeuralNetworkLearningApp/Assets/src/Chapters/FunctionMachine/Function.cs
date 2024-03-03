using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Function
{
    DOUBLE,
    SQUARE,
    COUNT_LENGTH,
    ADD,
    SEPARATE_FIRST_LETTER

}

public static class FunctionDetails
{
    private static Dictionary<Function, Func<object[], object[]>> functionImpelementations = new Dictionary<Function, Func<object[], object[]>> {
        { Function.DOUBLE, inputs => new object[]{(int)inputs[0] * 2 } },
        { Function.ADD, inputs => new object[]{(int)inputs[0] + (int)inputs[1] } },
    };

    private static Dictionary<Function, Type[]> functionInputTypes = new Dictionary<Function, Type[]> {
        { Function.DOUBLE, new Type[]{typeof(int) } },
        { Function.ADD, new Type[]{typeof(int), typeof(int) } }
    };

    private static Dictionary<Function, Type[]> functionOutputTypes = new Dictionary<Function, Type[]> {
        { Function.DOUBLE, new Type[]{typeof(int) } },
        { Function.ADD, new Type[]{typeof(int) } },
    };

    public static Func<object[], object[]> GetImplementation(Function function)
    {
        return functionImpelementations[function];
    }

    public static Type[] GetFunctionInputTypes(Function function)
    {
        return functionInputTypes[function];
    }

    public static Type[] GetFunctionOutputTypes(Function function)
    {
        return functionOutputTypes[function];
    }
}