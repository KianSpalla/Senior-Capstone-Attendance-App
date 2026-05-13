using QRCoder;

namespace Attendance.Api.Services;

public interface IEventQrCodeService
{
    string GetPayload(string eventCode);
    string GenerateSvg(string eventCode);
}

public class EventQrCodeService : IEventQrCodeService
{
    public string GetPayload(string eventCode)
    {
        return eventCode.Trim().ToUpperInvariant();
    }

    public string GenerateSvg(string eventCode)
    {
        var payload = GetPayload(eventCode);

        using var generator = new QRCodeGenerator();
        using var qrData = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new SvgQRCode(qrData);

        return qrCode.GetGraphic(8);
    }
}
