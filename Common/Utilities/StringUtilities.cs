using System.Security.Cryptography;
using System.Text;

namespace SimpleArchitecture.Common.Utilities;

public static class StringUtilities
{
    public static string GenerateRandomStringBase64(uint byteCount)
    {
        var randomNumber = new byte[byteCount];
        
        using var rng = RandomNumberGenerator.Create();
        
        rng.GetBytes(randomNumber);
        
        var randomString =  Convert.ToBase64String(randomNumber);

        return randomString;
    }

    public static string BuildMemoryCacheKey(string key, string context) => $"{context}{Constants.MemoryCacheIdentifiers.Separator}{key}";

    public static string BuildDictionaryString(this IReadOnlyDictionary<string, string> map)
    {
        if (map.Count == 0) 
            return string.Empty;
        
        var sb = new StringBuilder();

        foreach (var pair in map)
        {
            var msg = $"{pair.Key}: {pair.Value}{Environment.NewLine}";

            sb.Append(msg);
        }

        return sb.ToString();
    }
    
    public static string GenerateRandomPassword()
    {
        var random = new Random();
        
        const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string symbols = "@#$-_";
        const string allChars = lowerCaseChars + upperCaseChars + digits + symbols;

        const int length = 8;
        
        // StringBuilder for constructing the final string
        var result = new StringBuilder(length);

        // Guarantee at least one uppercase letter and one digit
        result.Append(upperCaseChars[random.Next(upperCaseChars.Length)]);
        result.Append(symbols[random.Next(symbols.Length)]);
        result.Append(digits[random.Next(digits.Length)]);

        // Fill the remaining characters
        for (var i = result.Length; i < length; i++)
        {
            result.Append(allChars[random.Next(allChars.Length)]);
        }

        // Shuffle the result to mix the guaranteed characters with others
        return ShuffleString(result.ToString());
    }
    
    private static string ShuffleString(string input)
    {
        var random = new Random();
        
        var array = input.ToCharArray();
        
        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            // Swap array[i] with array[j]
            (array[i], array[j]) = (array[j], array[i]);
        }
        
        return new string(array);
    }
    
    public static string GenerateRandomString(int length)
    {
        var random = new Random();
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var result = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }
}