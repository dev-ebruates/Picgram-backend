namespace Application.Services;

public class GoogleAuthService
{
  public async Task<ClaimsPrincipal?> ValidateGoogleToken(string token)
  {
    // Google'ın public anahtarlarını al
    using var httpClient = new HttpClient();
    var googleKeysJson = await httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");
    var googleKeys = JsonConvert.DeserializeObject<GoogleKeys>(googleKeysJson);

    // Token handler
    var handler = new JwtSecurityTokenHandler();
    var validationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidIssuer = "https://accounts.google.com", // veya "https://accounts.google.com"
      ValidateAudience = true,
      ValidAudience = "1050319585875-1ujql7a4palt2csnbm2ohf1ufbcr1mio.apps.googleusercontent.com", // Google Console'daki Client ID'nizi yazın
      ValidateLifetime = true,
      RequireSignedTokens = true,
      IssuerSigningKeys = googleKeys.ToSecurityKeys() // Google'ın public key'leri burada kullanılıyor
    };

    try
    {
      var principal = handler.ValidateToken(token, validationParameters, out var securityToken);

      // Doğrulanan token'ın tipi JWT olmalı
      if (securityToken is JwtSecurityToken jwtToken &&
          jwtToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
      {
        return principal; // Başarılı doğrulama
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Token doğrulama hatası: {ex.Message}");
    }

    return null; // Başarısız doğrulama
  }
}

public class GoogleKeys
{
  public List<Key> Keys { get; set; }

  public IEnumerable<SecurityKey> ToSecurityKeys()
  {
    foreach (var key in Keys)
    {
      var rsa = RSA.Create();
      rsa.ImportParameters(new RSAParameters
      {
        Modulus = Base64UrlDecode(key.N),
        Exponent = Base64UrlDecode(key.E)
      });
      yield return new RsaSecurityKey(rsa);
    }
  }

  private static byte[] Base64UrlDecode(string input)
  {
    input = input.Replace('-', '+').Replace('_', '/');
    switch (input.Length % 4)
    {
      case 2: input += "=="; break;
      case 3: input += "="; break;
    }
    return Convert.FromBase64String(input);
  }
}

public class Key
{
  public string Kty { get; set; }
  public string Alg { get; set; }
  public string Use { get; set; }
  public string Kid { get; set; }
  public string N { get; set; }
  public string E { get; set; }
}