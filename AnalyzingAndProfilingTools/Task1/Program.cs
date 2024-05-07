using System.Security.Cryptography;

GeneratePasswordHashUsingSalt("veryveryvery1ongp@ssw0rdw!thCust0mSymb0!s@ndNum8ers@andR@andomStr!ingksdihfiuehfgnxlv jko9syh8fuygbdjhgbvo;siu9diywhgeyutfvg d,jvso");

static string GeneratePasswordHashUsingSalt(string passwordText)
{
    var saltLength = 16;
    var hashOutputLength = 20;
    var iterate = 10000;
    using (var pbkdf2 = new Rfc2898DeriveBytes(passwordText, saltLength, iterate, HashAlgorithmName.SHA1))
    {
        byte[] hashBytes = new byte[saltLength + hashOutputLength];

        Array.Copy(pbkdf2.Salt, 0, hashBytes, 0, 16);
        Array.Copy(pbkdf2.GetBytes(hashOutputLength), 0, hashBytes, saltLength, hashOutputLength);

        return Convert.ToBase64String(hashBytes);
    }
}

static string GeneratePasswordHashUsingSaltOld(string passwordText, byte[] salt)
{

    var iterate = 10000;
    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
    byte[] hash = pbkdf2.GetBytes(20);

    byte[] hashBytes = new byte[36];
    Array.Copy(salt, 0, hashBytes, 0, 16);
    Array.Copy(hash, 0, hashBytes, 16, 20);

    var passwordHash = Convert.ToBase64String(hashBytes);

    return passwordHash;

}
