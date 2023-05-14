using System.Text.RegularExpressions;

namespace CollectionKeepersAPIV1.Functions
{
    public class Functions
    {
        public static bool CheckIfValidEmail(string EmailToCheck)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(EmailToCheck, emailPattern);
        }

        public static bool IsListEmpty<T>(List<T> ListToTest)
        {
            return ListToTest.Count == 0;
        }
    }
}
