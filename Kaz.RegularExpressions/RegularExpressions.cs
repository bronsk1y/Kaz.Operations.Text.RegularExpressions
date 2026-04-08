using System.Text.RegularExpressions;

namespace Kaz.Operations.Text.RegularExpressions
{
    /// <summary>
    /// Provides a fluent API builder for regular expressions.
    /// </summary>
    public class RegexBuilder
    {
        /// <summary>
        /// Gets the current regular expression pattern value.
        /// </summary>
        public string Value { get; internal set; } = "";

        private string LastElement { get; set; } = "";

        private bool CanApplyQuantifier { get; set; } = true;

        private RegexBuilder Append(string part, bool canQuantify)
        {
            Value += part;
            LastElement = part;
            CanApplyQuantifier = canQuantify;
            return this;
        }

        /// <summary>
        /// Anchors the match to the start of the string.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder Start()
        {
            if (string.IsNullOrEmpty(Value))
            {
                Value = "^";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Start anchor must be placed first.");
            }

            return this;
        }

        /// <summary>
        /// Anchors the match to the absolute start of the string.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder AbsoluteStart() => Append("\\A", false);

        /// <summary>
        /// Matches any single character except a newline.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder Any() => Append(".", true);

        /// <summary>
        /// Appends an escaped literal string to the pattern.
        /// </summary>
        /// <param name="text">The literal text to match.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder Literal(string text)
        {
            CanApplyQuantifier = true;
            if (text.Length > 1)
            {
                Value += $"(?:{Regex.Escape(text)})";
                LastElement = $"(?:{Regex.Escape(text)})";
            }
            else
            {
                Value += Regex.Escape(text);
                LastElement = Regex.Escape(text);
            }

            return this;
        }

        /// <summary>
        /// Matches any decimal digit.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder Digit() => Append("\\d", true);

        /// <summary>
        /// Matches any character that is not a decimal digit.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder NonDigit() => Append("\\D", true);

        /// <summary>
        /// Matches any word character.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder Word() => Append("\\w", true);

        /// <summary>
        /// Matches any non-word character.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder NonWord() => Append("\\W", true);

        /// <summary>
        /// Asserts a position at a word boundary.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder WordBoundary() => Append("\\b", false);

        /// <summary>
        /// Asserts a position that is not a word boundary.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder NonWordBoundary() => Append("\\B", false);

        /// <summary>
        /// Matches any whitespace character.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder Whitespace() => Append("\\s", true);

        /// <summary>
        /// Matches any non-whitespace character.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder NonWhitespace() => Append("\\S", true);

        /// <summary>
        /// Adds a backreference to a previously matched named group.
        /// </summary>
        /// <param name="name">The name of the capturing group to reference.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder NamedBacklink(string name) => Append($"\\k<{name}>", true);

        /// <summary>
        /// Adds a backreference to a previously matched numbered group.
        /// </summary>
        /// <param name="groupNumber">The one-based index of the capturing group to reference.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder UnnamedBacklink(int groupNumber) => Append($"\\{groupNumber}", true);

        /// <summary>
        /// Asserts a position at the end of the previous match.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder PreviousMatchEnd() => Append("\\G", true);

        /// <summary>
        /// Matches any character belonging to the specified Unicode category.
        /// </summary>
        /// <param name="category">The Unicode category to match.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="category"/> is not supported.</exception>
        public RegexBuilder UnicodeCategory(UnicodeCategoryType category) =>
            Append($"\\p{{{GetUnicodeCategoryCode(category)}}}", true);

        /// <summary>
        /// Matches any character not belonging to the specified Unicode category.
        /// </summary>
        /// <param name="category">The Unicode category to exclude.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="category"/> is not supported.</exception>
        public RegexBuilder NonUnicodeCategory(UnicodeCategoryType category) =>
            Append($"\\P{{{GetUnicodeCategoryCode(category)}}}", true);

        private static string GetUnicodeCategoryCode(UnicodeCategoryType category) => category switch
        {
            UnicodeCategoryType.Letter => "L",
            UnicodeCategoryType.UppercaseLetter => "Lu",
            UnicodeCategoryType.LowercaseLetter => "Ll",
            UnicodeCategoryType.Number => "N",
            UnicodeCategoryType.Punctuation => "P",
            UnicodeCategoryType.Separator => "Z",
            UnicodeCategoryType.Symbol => "S",
            _ => throw new ArgumentException("This type is not supported.")
        };

        /// <summary>
        /// Makes the preceding element optional.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexBuilder Optional()
        {
            if (CanApplyQuantifier)
            {
                Value += "?";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Requires the preceding element to appear one or more times.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexBuilder OneOrMore()
        {
            if (CanApplyQuantifier)
            {
                Value += "+";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Allows the preceding element to appear zero or more times.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexBuilder ZeroOrMore()
        {
            if (CanApplyQuantifier)
            {
                Value += "*";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Requires the preceding element to appear exactly the specified number of times.
        /// </summary>
        /// <param name="value">The exact number of repetitions.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexBuilder Repeat(int value)
        {
            if (CanApplyQuantifier)
            {
                Value += $"{{{value}}}";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Requires the preceding element to appear between <paramref name="min"/> and <paramref name="max"/> times.
        /// </summary>
        /// <param name="min">The minimum number of repetitions.</param>
        /// <param name="max">The maximum number of repetitions.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexBuilder Repeat(int min, int max)
        {
            if (CanApplyQuantifier)
            {
                Value += $"{{{min},{max}}}";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Makes the preceding quantifier lazy, that makes it match less characters. 
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been made lazy or previous element is not a quantifier.</exception>
        public RegexBuilder Lazy()
        {
            if (LastElement.Equals("?") || LastElement.Equals("+") || LastElement.Equals("*"))
                Append("?", false);
            else throw new InvalidOperationException("Previous quantifier is already lazy or previous element is not a quantifier.");

            return this;
        }

        /// <summary>
        /// Anchors the match to the end of the string.
        /// </summary>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder End()
        {
            Value += "$";
            return this;
        }

        /// <summary>
        /// Appends a regex group built by the provided <see cref="RegexGroup"/> factory.
        /// </summary>
        /// <param name="factory">A factory function that returns a configured <see cref="RegexGroup"/>.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder Group(Func<RegexGroup> factory) => Append(factory().Build(), true);

        /// <summary>
        /// Appends a character class group built by the provided <see cref="RegexCharset"/> factory.
        /// </summary>
        /// <param name="factory">A factory function that returns a configured <see cref="RegexCharset"/>.</param>
        /// <returns>The current <see cref="RegexBuilder"/> instance.</returns>
        public RegexBuilder Charset(Func<RegexCharset> factory) => Append(factory().Build(), true);

        /// <summary>
        /// Compiles the current pattern into a <see cref="Regex"/> instance.
        /// </summary>
        /// <returns>A compiled <see cref="Regex"/> for the current pattern.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the pattern is empty.</exception>
        public Regex Build()
        {
            if (Value.Length > 0)
                return new Regex(Value);
            else throw new InvalidOperationException("Builder pattern value cannot be empty.");
        }

        /// <summary>
        /// Compiles the current pattern into a <see cref="Regex"/> instance using provided options.
        /// </summary>
        /// <param name="options">A <see cref="RegexOptions"></see> to apply to the compiled regular expression.</param>
        /// <returns>A compiled <see cref="Regex"/> for the current pattern with the applied options.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the pattern is empty.</exception>
        public Regex Build(RegexOptions options)
        {
            if (Value.Length > 0)
                return new Regex(Value, options);
            else throw new InvalidOperationException("Builder pattern value cannot be empty.");
        }

        /// <summary>
        /// Compiles the current pattern into a <see cref="Regex"/> instance using provided options and a timeout interval.
        /// </summary>
        /// <param name="options">A <see cref="RegexOptions"></see> to apply to the compiled regular expression.</param>
        /// <param name="matchTimeout">A timeout interval that defines the maximum duration for a match to be found.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when the pattern is empty.</exception>
        public Regex Build(RegexOptions options, TimeSpan matchTimeout)
        {
            if (Value.Length > 0)
                return new Regex(Value, options, matchTimeout);
            else throw new InvalidOperationException("Builder pattern value cannot be empty.");
        }
    }

    /// <summary>
    /// Represents a single group within a regex pattern.
    /// </summary>
    public class RegexGroup
    {
        /// <summary>
        /// Gets the current pattern value of the group.
        /// </summary>
        public string Value { get; internal set; } = "";

        private string LastElement { get; set; } = "";
        private bool CanApplyQuantifier { get; set; } = true;

        /// <summary>
        /// Gets or sets the type of the group.
        /// </summary>
        public GroupType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the group, used when <see cref="Type"/> is <see cref="GroupType.Named"/>.
        /// </summary>
        public string Name { get; set; } = "";

        private RegexGroup Append(string part, bool canQuantify)
        {
            Value += part;
            LastElement = part;
            CanApplyQuantifier = canQuantify;
            return this;
        }

        /// <summary>
        /// Anchors the match to the absolute start of the string.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup AbsoluteStart() => Append("\\A", false);

        /// <summary>
        /// Matches any single character except a newline.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup Any() => Append(".", true);

        /// <summary>
        /// Appends an escaped literal string to the group pattern.
        /// </summary>
        /// <param name="text">The literal text to match.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup Literal(string text)
        {
            CanApplyQuantifier = true;
            if (text.Length > 1)
            {
                Value += $"(?:{Regex.Escape(text)})";
                LastElement = $"(?:{Regex.Escape(text)})";
            }
            else
            {
                Value += Regex.Escape(text);
                LastElement = Regex.Escape(text);
            }

            return this;
        }

        /// <summary>
        /// Matches any decimal digit.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup Digit() => Append("\\d", true);

        /// <summary>
        /// Matches any character that is not a decimal digit.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup NonDigit() => Append("\\D", true);

        /// <summary>
        /// Matches any word character.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup Word() => Append("\\w", true);

        /// <summary>
        /// Matches any non-word character.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup NonWord() => Append("\\W", true);

        /// <summary>
        /// Asserts a position at a word boundary.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup WordBoundary() => Append("\\b", false);

        /// <summary>
        /// Asserts a position that is not a word boundary.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup NonWordBoundary() => Append("\\B", false);

        /// <summary>
        /// Matches any whitespace character.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup Whitespace() => Append("\\s", true);

        /// <summary>
        /// Matches any non-whitespace character.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup NonWhitespace() => Append("\\S", true);

        /// <summary>
        /// Adds a backreference to a previously matched named group.
        /// </summary>
        /// <param name="name">The name of the capturing group to reference.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup NamedBacklink(string name) => Append($"\\k<{name}>", true);

        /// <summary>
        /// Adds a backreference to a previously matched numbered group.
        /// </summary>
        /// <param name="groupNumber">The one-based index of the capturing group to reference.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup UnnamedBacklink(int groupNumber) => Append($"\\{groupNumber}", true);

        /// <summary>
        /// Asserts a position at the end of the previous match.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup PreviousMatchEnd() => Append("\\G", true);

        /// <summary>
        /// Matches any character belonging to the specified Unicode category.
        /// </summary>
        /// <param name="category">The Unicode category to match.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="category"/> is not supported.</exception>
        public RegexGroup UnicodeCategory(UnicodeCategoryType category) =>
            Append($"\\p{{{GetUnicodeCategoryCode(category)}}}", true);

        /// <summary>
        /// Matches any character not belonging to the specified Unicode category.
        /// </summary>
        /// <param name="category">The Unicode category to exclude.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="category"/> is not supported.</exception>
        public RegexGroup NonUnicodeCategory(UnicodeCategoryType category) =>
            Append($"\\P{{{GetUnicodeCategoryCode(category)}}}", true);

        private static string GetUnicodeCategoryCode(UnicodeCategoryType category) => category switch
        {
            UnicodeCategoryType.Letter => "L",
            UnicodeCategoryType.UppercaseLetter => "Lu",
            UnicodeCategoryType.LowercaseLetter => "Ll",
            UnicodeCategoryType.Number => "N",
            UnicodeCategoryType.Punctuation => "P",
            UnicodeCategoryType.Separator => "Z",
            UnicodeCategoryType.Symbol => "S",
            _ => throw new ArgumentException("This type is not supported.")
        };

        /// <summary>
        /// Makes the preceding element optional.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexGroup Optional()
        {
            if (CanApplyQuantifier)
            {
                Value += "?";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Requires the preceding element to appear one or more times.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexGroup OneOrMore()
        {
            if (CanApplyQuantifier)
            {
                Value += "+";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Allows the preceding element to appear zero or more times.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexGroup ZeroOrMore()
        {
            if (CanApplyQuantifier)
            {
                Value += "*";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Requires the preceding element to appear exactly the specified number of times.
        /// </summary>
        /// <param name="value">The exact number of repetitions.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexGroup Repeat(int value)
        {
            if (CanApplyQuantifier)
            {
                Value += $"{{{value}}}";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Requires the preceding element to appear between <paramref name="min"/> and <paramref name="max"/> times.
        /// </summary>
        /// <param name="min">The minimum number of repetitions.</param>
        /// <param name="max">The maximum number of repetitions.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been applied to the preceding element.</exception>
        public RegexGroup Repeat(int min, int max)
        {
            if (CanApplyQuantifier)
            {
                Value += $"{{{min},{max}}}";
                CanApplyQuantifier = false;
            }
            else
            {
                throw new InvalidOperationException("Quantifier has already been applied to the previous element.");
            }
            return this;
        }

        /// <summary>
        /// Makes the preceding quantifier lazy, that makes it match less characters. 
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a quantifier has already been made lazy or previous element is not a quantifier.</exception>
        public RegexGroup Lazy()
        {
            if (LastElement.Equals("?") || LastElement.Equals("+") || LastElement.Equals("*"))
                Append("?", false);
            else throw new InvalidOperationException("Previous quantifier is already lazy or previous element is not a quantifier.");

            return this;
        }

        /// <summary>
        /// Appends an alternation operator to allow matching either the preceding or the following pattern.
        /// </summary>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the group is empty or already ends with an alternation operator.</exception>
        public RegexGroup Or()
        {
            if (string.IsNullOrEmpty(Value))
                throw new InvalidOperationException("Alternation cannot be at the start of a group.");

            if (Value.EndsWith("|"))
                throw new InvalidOperationException("Alternation symbol is already present.");

            Value += "|";
            CanApplyQuantifier = false;
            return this;
        }

        /// <summary>
        /// Appends a character class built by the provided <see cref="RegexCharset"/> factory.
        /// </summary>
        /// <param name="factory">A factory function that returns a configured <see cref="RegexCharset"/>.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup Charset(Func<RegexCharset> factory) => Append(factory().Build(), true);

        /// <summary>
        /// Appends a regex group built by the provided <see cref="RegexGroup"/> factory.
        /// </summary>
        /// <param name="factory">A factory function that returns a configured <see cref="RegexCharset"/>.</param>
        /// <returns>The current <see cref="RegexGroup"/> instance.</returns>
        public RegexGroup Group(Func<RegexGroup> factory) => Append(factory().Build(), true);

        /// <summary>
        /// Builds the group pattern string wrapped in the group syntax defined by its type.
        /// </summary>
        /// <returns>The complete group pattern string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the group pattern is empty or <see cref="Name"/> is empty for a named group.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="Type"/> is not supported.</exception>
        public string Build()
        {
            if (Value.Length > 0)
            {
                switch (Type)
                {
                    case GroupType.Capturing:
                        return $"({Value})";
                    case GroupType.NonCapturing:
                        return $"(?:{Value})";
                    case GroupType.Named:
                        if (!string.IsNullOrEmpty(Name))
                            return $"(?<{Name}>{Value})";
                        else throw new InvalidOperationException("Name value cannot be empty if the group type is \"Named\"");
                    case GroupType.Lookahead:
                        return $"(?={Value})";
                    case GroupType.NegativeLookahead:
                        return $"(?!{Value})";
                    case GroupType.Lookbehind:
                        return $"(?<={Value})";
                    case GroupType.NegativeLookbehind:
                        return $"(?<!{Value})";
                    default:
                        throw new ArgumentException("This type is not supported or dosen't exist.");
                }
            }
            else
            {
                throw new InvalidOperationException("Group pattern value cannot be empty.");
            }
        }
    }

    /// <summary>
    /// Represents a character class within a regex pattern.
    /// </summary>
    public class RegexCharset
    {
        /// <summary>
        /// Gets the current character class pattern value.
        /// </summary>
        public string Value { get; internal set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether the character class is negated.
        /// </summary>
        public bool Negate { get; set; } = false;

        /// <summary>
        /// Adds a character range to the character class.
        /// </summary>
        /// <param name="start">The first character in the range.</param>
        /// <param name="end">The last character in the range.</param>
        /// <returns>The current <see cref="RegexCharset"/> instance.</returns>
        public RegexCharset AddRange(char start, char end)
        {
            Value += $"{start}-{end}";
            return this;
        }

        /// <summary>
        /// Adds a single character to the character class.
        /// </summary>
        /// <param name="c">The character to add.</param>
        /// <returns>The current <see cref="RegexCharset"/> instance.</returns>
        public RegexCharset AddChar(char c)
        {
            Value += c;
            return this;
        }

        /// <summary>
        /// Adds each character in the provided string individually to the character class.
        /// </summary>
        /// <param name="chars">A string whose characters are each added to the class.</param>
        /// <returns>The current <see cref="RegexCharset"/> instance.</returns>
        public RegexCharset AddWord(string chars)
        {
            foreach (var c in chars)
                AddChar(c);

            return this;
        }

        /// <summary>
        /// Builds the character class pattern string.
        /// </summary>
        /// <returns>The complete character class pattern string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the character class pattern is empty.</exception>
        public string Build()
        {
            if (string.IsNullOrEmpty(Value))
                throw new InvalidOperationException("Charset pattern value cannot be empty.");

            return Negate ? $"[^{Value}]" : $"[{Value}]";
        }
    }
}