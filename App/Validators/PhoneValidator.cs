using System.Text.RegularExpressions;

namespace App.Validators
{
    public class PhoneValidator
    {
        private static readonly Regex PhoneRegex = new Regex(@"^\+7\d{10}$", RegexOptions.Compiled);

        public bool Validate(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
            return PhoneRegex.IsMatch(phone);
        }
    }
}