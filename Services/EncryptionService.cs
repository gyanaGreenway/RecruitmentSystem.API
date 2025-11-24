using Microsoft.AspNetCore.DataProtection;

namespace RecruitmentSystem.API.Services;

public interface IEncryptionService
{
    string Protect(string plaintext);
    string Unprotect(string protectedText);
}

public class DataProtectionEncryptionService : IEncryptionService
{
    private readonly IDataProtector _protector;

    public DataProtectionEncryptionService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("RecruitmentSystem.Onboarding.Documents.v1");
    }

    public string Protect(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext)) return plaintext;
        return _protector.Protect(plaintext);
    }

    public string Unprotect(string protectedText)
    {
        if (string.IsNullOrEmpty(protectedText)) return protectedText;
        try
        {
            return _protector.Unprotect(protectedText);
        }
        catch
        {
            // If unprotect fails, return raw value to avoid breaking consumers
            return protectedText;
        }
    }
}
