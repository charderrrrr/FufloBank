// Валидатор формата номера телефона
// PhoneRegex - скомпилированное регулярное выражение для проверки формата +7XXXXXXXXXX (ровно 10 цифр после +7)
// Validate - проверяет строку на соответствие формату, возвращает false при null или несовпадении

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
