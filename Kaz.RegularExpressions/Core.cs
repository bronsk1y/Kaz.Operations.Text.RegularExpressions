namespace Kaz.Operations.Text.RegularExpressions
{
    #region Parametrs

    /// <summary>
    /// Specifies the Unicode character category.
    /// </summary>
    public enum UnicodeCategoryType
    {
        /// <summary>
        /// Any letter.
        /// </summary>
        Letter,
        /// <summary>
        /// An uppercase letter.
        /// </summary>
        UppercaseLetter,
        /// <summary>
        /// A lowercase letter.
        /// </summary>
        LowercaseLetter,
        /// <summary>
        /// Any numeric character.
        /// </summary>
        Number,
        /// <summary>
        /// Any punctuation character.
        /// </summary>
        Punctuation,
        /// <summary>
        /// Any separator character.
        /// </summary>
        Separator,
        /// <summary>
        /// Any symbol character.
        /// </summary>
        Symbol
    }

    /// <summary>
    /// Specifies the type of a regex group.
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// A capturing group that stores its match for later use.
        /// </summary>
        Capturing,
        /// <summary>
        /// A non-capturing group that groups without storing the match.
        /// </summary>
        NonCapturing,
        /// <summary>
        /// A named capturing group that stores its match under a given name.
        /// </summary>
        Named,
        /// <summary>
        /// A positive lookahead assertion that matches if followed by the group pattern.
        /// </summary>
        Lookahead,
        /// <summary>
        /// A negative lookahead assertion that matches if not followed by the group pattern.
        /// </summary>
        NegativeLookahead,
        /// <summary>
        /// A positive lookbehind assertion that matches if preceded by the group pattern.
        /// </summary>
        Lookbehind,
        /// <summary>
        /// A negative lookbehind assertion that matches if not preceded by the group pattern.
        /// </summary>
        NegativeLookbehind
    }

    #endregion
    internal static class RegexBuilderCore
    {
        internal static string GetUnicodeCategoryCode(UnicodeCategoryType category) => category switch
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
    }
}