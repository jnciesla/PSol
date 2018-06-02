using GeonBit.UI.Entities.TextValidators;

namespace Validators
{
    /// <summary>
    /// Alphanumeric validator
    /// </summary>
    internal class AlphaNumValidator : ITextValidator
    {
        // the regex to use
        System.Text.RegularExpressions.Regex _regex;

        // regex for slug with spaces
        static System.Text.RegularExpressions.Regex _slugNoSpaces = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9]+$");

        // regex for slug without spaces
        static System.Text.RegularExpressions.Regex _slugWithSpaces = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\ ]+$");

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
        public bool ValidateText(ref string text, string oldText)
        {
            return (text.Length == 0 || _regex.IsMatch(text));
        }
    }
}