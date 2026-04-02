# Kaz.Operations.Text.RegularExpressions

## About

This library extends the Kaz.Operations.Text possibilities by adding a fluent API builder for Regular Expressions. This library targets **.NET 8** and **.NET Framework 4.7.2.**

## Features

- Adds a fluent API builder for regular expressions, regular expressions groups and charsets.

## How to use

```csharp
using System;
using Kaz.Operations.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        var builder = new RegexBuilder();

        builder
            .Start() // Start anchor
            .Group(() => new RegexCharset() // Charset
                .AddRange('a', 'z')
                .AddRange('A', 'Z')
                .AddRange('0', '9')
                .AddWord("._%+-")
            ).OneOrMore() // Quantifier

            .Literal("@")

            .Group(() => new RegexCharset() // Charset
                .AddRange('a', 'z')
                .AddRange('A', 'Z')
                .AddRange('0', '9')
                .AddWord(".-")
            ).OneOrMore() // Quantifier

            .Literal(".")

            .Group(() => new RegexCharset() // Charset
                .AddRange('a', 'z')
                .AddRange('A', 'Z')
            ).Repeat(2, 6) // Quantifier

            .End(); // End anchor

        var emailRegex = builder.Build(); // returns a new Regex instance

        string testEmail = "bronsk1y@gmail.com";
        bool isValid = emailRegex.IsMatch(testEmail); // true
    }
}
```

## Main Classes & Types

The main classes are:

- **`RegexBuilder`**
- `RegexGroup`
- `RegexCharset`

The main types are:

- **`GroupType`**
- `UnicodeCategoryType`

## Licence

Kaz.Operations.Text.RegularExpressions is released as an open source project under the [MIT Licence](https://licenses.nuget.org/MIT).

## Feedback & Contributing

You can report a bug or contribute at [the Github repository]().
