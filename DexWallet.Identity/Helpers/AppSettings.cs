namespace DexWallet.Common.Attributes;

public class AppSettings
{
    public string SigningKey { get; set; } = null!;

    // refresh token time to live (in days), inactive tokens are
    // automatically deleted from the database after this time
    public int RefreshTokenTtl { get; set; }
}