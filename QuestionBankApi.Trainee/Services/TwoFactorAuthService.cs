using OtpNet;
using QRCoder;


namespace QuestionBankApi.Trainee.Services
{
    public class TwoFactorAuthService
    {
        public string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateQrCodeUri(string email, string secretKey, string issuer)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secretKey));
            return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}?secret={secretKey}&issuer={Uri.EscapeDataString(issuer)}";
        }

        public byte[] GenerateQrCode(string qrCodeUri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        public bool ValidateTwoFactorCode(string secretKey, string code)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secretKey));
            return totp.VerifyTotp(code, out long timeStepMatched, new VerificationWindow(2, 2));
        }

    }
}
