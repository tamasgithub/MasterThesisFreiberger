using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Function
{
    DOUBLE,
    SQUARE,
    COUNT_LENGTH,
    NUMBER_TO_LETTER,
    LETTER_TO_NUMBER,
    ADD,
    SEPARATE_FIRST_LETTER

}

public static class FunctionDetails
{
    private static Dictionary<Function, Func<object[], object[]>> functionImpelementations = new Dictionary<Function, Func<object[], object[]>> {
        { Function.DOUBLE, inputs => new object[]{(int)inputs[0] * 2 } },
        { Function.SQUARE, inputs => new object[]{(int)inputs[0] * (int)inputs[0] } },
        { Function.COUNT_LENGTH, inputs => new object[]{((string)inputs[0]).Length } },
        { Function.NUMBER_TO_LETTER, inputs => new object[]{(char)(Mathf.Clamp((int)inputs[0], 1, 26) + 96) } },
        { Function.LETTER_TO_NUMBER, inputs => new object[]{ (int)((char)inputs[0]) - 96 } },
        { Function.ADD, inputs => new object[]{(int)inputs[0] + (int)inputs[1] } },
        // if the rest of the word is 1 letter, it is converted to a character type during creation of the output in FunctionMachine.cs
        { Function.SEPARATE_FIRST_LETTER, inputs => new object[]{((string)inputs[0])[0], ((string)inputs[0]).Substring(1) } },
    };

    private static Dictionary<Function, Type[]> functionInputTypes = new Dictionary<Function, Type[]> {
        { Function.DOUBLE, new Type[]{typeof(int) } },
        { Function.SQUARE, new Type[]{typeof(int) } },
        { Function.COUNT_LENGTH, new Type[]{typeof(string) } },
        { Function.NUMBER_TO_LETTER, new Type[]{typeof(int) } },
        { Function.LETTER_TO_NUMBER, new Type[]{typeof(char) } },
        { Function.ADD, new Type[]{typeof(int), typeof(int) } },
        { Function.SEPARATE_FIRST_LETTER, new Type[]{typeof(string) } },
    };

    private static Dictionary<Function, Type[]> functionOutputTypes = new Dictionary<Function, Type[]> {
        { Function.DOUBLE, new Type[]{typeof(int) } },
        { Function.SQUARE, new Type[]{typeof(int) } },
        { Function.COUNT_LENGTH, new Type[]{typeof(int) } },
        { Function.NUMBER_TO_LETTER, new Type[]{typeof(char) } },
        { Function.LETTER_TO_NUMBER, new Type[]{typeof(int) } },
        { Function.ADD, new Type[]{typeof(int) } },
        { Function.SEPARATE_FIRST_LETTER, new Type[]{typeof(char), typeof(string) } },
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