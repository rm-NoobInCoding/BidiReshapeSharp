# BidiReshapeSharp 
![NuGet Version](https://img.shields.io/nuget/v/BidiReshapeSharp)
![NuGet Downloads](https://img.shields.io/nuget/dt/BidiReshapeSharp)

A library to Bi-Directional and Reshape Arabic or Persian texts

## Overview
BidiReshapeSharp is a .NET library for pre-processing Right-to-Left (RTL) scripts, such as Arabic and Persian, for display in rendering environments that do not natively implement the Unicode Bidirectional Algorithm (UBA).
There are good Python libraries that do this, but there was no library in C# to do the same. So I decided to do it myself.
The library resolves two primary issues that arise in basic LTR rendering pipelines:
1. Character Reordering: It handles the complex rearrangement of mixed RTL and LTR text segments (text, numbers, punctuation) to ensure correct visual flow.
2. Contextual Shaping: It replaces generic Unicode characters with their correct contextual glyph forms (initial, medial, final, or isolated) required for connecting scripts.

By using the shaping method, the input string is converted into a sequence of display characters that can be rendered correctly by any simple LTR text engine.

## Features
* Applies the necessary initial, medial, final, and isolated forms for all standard Arabic and Persian characters.
* Correctly substitutes common character pairs, such as Lām + Alif, with their typographic ligature forms (e.g., 'لا' becomes 'ﻻ').
* Implements full reordering logic to correctly position LTR elements (like numbers) within RTL sentences.
* Simple, static API for straightforward integration.

## Installation
The library is available as a NuGet package.

### NuGet Package Manager
```bash
Install-Package BidiReshapeSharp
```

### .NET CLI
```bash
dotnet add package BidiReshapeSharp
```

## Usage
The primary functionality is exposed through the static BidiShaper class and its Reshape method.

### Simple String Processing (Default Configuration)
The `ProcessString` method accepts a single string and returns the fully reshaped and reordered result using the default configuration settings.

```c#
using BidiReshapeSharp;
using System;

public class Program
{
    public static void Main()
    {
        // Example 1: Basic Arabic phrase requiring character connection and ligatures (e.g., in لأمر).
        string inputArabic = "الْكِتَابُ الْجَدِيدُ لأمر";
        string outputArabic = BidiReshape.ProcessString(inputArabic);

        Console.WriteLine($"Original (raw Unicode): {inputArabic}");
        Console.WriteLine($"Reshaped (display-ready): {outputArabic}");
        
        // The output string contains correctly connected glyphs and reordered segments.

        // Example 2: Mixing RTL text, LTR numbers, and punctuation.
        string inputMixed = "النص المختلط 123 مع ترقيم.";
        string outputMixed = BidiReshape.ProcessString(inputMixed);

        Console.WriteLine($"\nOriginal (mixed data): {inputMixed}");
        Console.WriteLine($"Reshaped (correctly ordered): {outputMixed}");
    }
}
```

### Advanced String Processing with Configuration
Use the overloaded `ProcessString` method along with a `ReshaperConfig` object to apply custom shaping rules.

```c#
using BidiReshapeSharp;
using System;
using BidiReshapeSharp.Reshaper;

public class Program
{
    public static void Main()
    {
        // Create a custom configuration object.
        var customConfig = new ReshaperConfig
        {
            DeleteHarakat = true,
            UseUnshapedInsteadOfIsolated = true
        };

        // We support persian texts with پچگژ too!
        string input = "نمونه متن پارسی";
        string output = BidiReshape.ProcessString(input, customConfig);

        Console.WriteLine($"Configured Output: {output}");
    }
}
```

## Based on 
Python Arabic Reshaper by mpcabd

BidiSharp by fsufyan
