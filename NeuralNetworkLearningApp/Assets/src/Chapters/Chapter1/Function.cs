using System;
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
    SEPARATE_FIRST_LETTER,
    BAD_ANIMAL_CLASSIFIER,
    BETTER_ANIMAL_CLASSIFIER
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
        // "categorizes" by looking at the first letter, won't work for other input words!
        { Function.BAD_ANIMAL_CLASSIFIER, inputs => BadAnimalClassifier(inputs) },
        { Function.BETTER_ANIMAL_CLASSIFIER, inputs => BetterAnimalClassifier(inputs) },
    };

    private static Dictionary<Function, Type[]> functionInputTypes = new Dictionary<Function, Type[]> {
        { Function.DOUBLE, new Type[]{typeof(int) } },
        { Function.SQUARE, new Type[]{typeof(int) } },
        { Function.COUNT_LENGTH, new Type[]{typeof(string) } },
        { Function.NUMBER_TO_LETTER, new Type[]{typeof(int) } },
        { Function.LETTER_TO_NUMBER, new Type[]{typeof(char) } },
        { Function.ADD, new Type[]{typeof(int), typeof(int) } },
        { Function.SEPARATE_FIRST_LETTER, new Type[]{typeof(string) } },
        { Function.BAD_ANIMAL_CLASSIFIER, new Type[]{typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) } },
        { Function.BETTER_ANIMAL_CLASSIFIER, new Type[]{typeof(int), typeof(int), typeof(int), typeof(int) } },
    };

    private static Dictionary<Function, Type[]> functionOutputTypes = new Dictionary<Function, Type[]> {
        { Function.DOUBLE, new Type[]{typeof(int) } },
        { Function.SQUARE, new Type[]{typeof(int) } },
        { Function.COUNT_LENGTH, new Type[]{typeof(int) } },
        { Function.NUMBER_TO_LETTER, new Type[]{typeof(char) } },
        { Function.LETTER_TO_NUMBER, new Type[]{typeof(int) } },
        { Function.ADD, new Type[]{typeof(int) } },
        { Function.SEPARATE_FIRST_LETTER, new Type[]{typeof(char), typeof(string) } },
        { Function.BAD_ANIMAL_CLASSIFIER, new Type[]{typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) } },
        { Function.BETTER_ANIMAL_CLASSIFIER, new Type[]{typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) } },
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

    private static object[] BadAnimalClassifier(object[] inputs)
    {
        Dictionary<string, object[]> classes = new Dictionary<string, object[]>()
        {
            {"reptile", new object[] { 18, 5, 16, 20, 9, 12, 5, 0, 0 } },
            {"mammal", new object[] { 13, 1, 13, 13, 1, 12, 0, 0, 0 } },
            {"bird", new object[] { 2, 9, 18, 4, 0, 0, 0, 0, 0 } },
            {"amphibian", new object[] { 1, 13, 16, 8, 9, 2, 9, 1, 14 } },
            {"fish", new object[] { 6, 9, 19, 8, 0, 0, 0, 0, 0 } },
        };
        Dictionary<string, string> mapping= new Dictionary<string, string>()
        {
            {"boa", "reptile" },
            {"dog", "mammal" },
            {"eagle", "bird" },
            {"frog", "amphibian" },
            {"paddlefish", "fish" },
        };

        string animalName = "";
        foreach (object input in inputs)
        {
            if ((int)input == 0)
            {
                break;
            }
            animalName += ((char)((int)input + 96)).ToString();
        }
        string mappedClass;
        object[] encodedClass;
        if (mapping.TryGetValue(animalName, out mappedClass)) {
            classes.TryGetValue(mappedClass, out encodedClass);
            return encodedClass;
        } else {
            throw new Exception("Unknown input");
        }
    }

    //inputs are bits encoding the features fur, legs, scales feathers
    // outputs are bits that one-hot encode the classes mammal, reptile, bird, amphibian and fish
    private static object[] BetterAnimalClassifier(object[] inputs)
    {
        bool[] features = new bool[inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            features[i] = (int)inputs[i] > 0;
        }

        int mappedClass;
        if (features[0])
        {
            mappedClass = 0;
        }
        else if (features[1])
        {
            if (features[2])
            {
                mappedClass = 1;
            } else if (features[3])
            {
                mappedClass = 2;
            } else
            {
                mappedClass = 3;
            }
        } else
        {
            mappedClass = 4;
        }
        object[] encodedClass = new object[] { 0, 0, 0, 0, 0 };
        encodedClass[mappedClass] = 1;
        return encodedClass;
    }
}