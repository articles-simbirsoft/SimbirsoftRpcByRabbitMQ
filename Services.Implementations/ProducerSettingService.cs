using Services.Abstractions;
using Services.Contracts;
namespace Services.Implementations;

public class ProducerSettingService : IProducerSettingService
{
    private int _start = 0;
    private int _end = 100;
    private int _attemptionCount = 3;

    public SettingDto GetSettings()
    {
        return new SettingDto(_start, _end, _attemptionCount);
    }

    public void UpdateSettings(SettingDto dto)
    {
        _start = dto.Start;
        _end = dto.End;
        _attemptionCount = dto.AttemptsCount;
    }
}
