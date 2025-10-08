namespace SchoolApp.Security;

public static class EncryptionUtil
{
    public static string Encrypt(string clearText)
    {
        var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(clearText);
        return encryptedPassword;
    }

    public static bool IsValidPassword(string plainText, string cipherText)
    {
        return BCrypt.Net.BCrypt.Verify(plainText, cipherText);
    }
}