using Services.Contracts;
namespace Services.Abstractions;
public interface IProducerSettingService
{
    SettingDto GetSettings();
    void UpdateSettings(SettingDto setting);
}

