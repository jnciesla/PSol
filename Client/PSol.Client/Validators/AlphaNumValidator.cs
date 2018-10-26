using System.Text.RegularExpressions;
using GeonBit.UI.Entities.TextValidators;

namespace Validators
{
    /// <summary>
    /// Alphanumeric validator
    /// </summary>
    internal class AlphaNumValidator : ITextValidator
    {
        // the regex to use
        private readonly Regex _regex;

        // regex for slug with spaces
        private static readonly Regex _slugNoSpaces = new Regex(@"^[a-zA-Z0-9]+$");

        // regex for slug without spaces
        private static readonly Regex _slugWithSpaces = new Regex(@"^[a-zA-Z0-9\ ]+$");

        /// <summary>
        /// Create the slug validator.
        /// </summary>
        /// <param name="allowSpaces">If true, will allow spaces.</param>
        public AlphaNumValidator(bool allowSpaces = false)
        {
            _regex = allowSpaces ? _slugWithSpaces : _slugNoSpaces;
        }

        /// <summary>
        /// Return true if text input is slug.
        /// </summary>
        /// <param name="text">New text input value.</param>
        /// <param name="oldText">Previous text input value.</param>
        /// <returns>If TextInput value is legal.</returns>
        public override bool ValidateText(ref string text, string oldText)
        {
            return (text.Length == 0 || _regex.IsMatch(text));
        }
    }
}