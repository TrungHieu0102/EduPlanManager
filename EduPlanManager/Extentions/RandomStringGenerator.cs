namespace EduPlanManager.Extentions
{
    public class RandomStringGenerator
    {
        public static string GenerateRandomString(int length = 10, char requiredChar = 'k')
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#_-";
            var random = new Random();

            char[] result = new char[length];

            result[random.Next(length)] = requiredChar;

            for (int i = 0; i < length; i++)
            {
                if (result[i] == '\0')
                {
                    result[i] = chars[random.Next(chars.Length)];
                }
            }

            return new string(result);
        }
    }
}
