using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Services.Abstractions;
using Services.Contracts;
using Services.Extensions;
using Services.Extensions.Exceptions;
using Services.Implementations;
namespace ProducerApp;

public sealed class StartApp : BackgroundService
{
    private readonly IMassTransitHelper _helper;
    private readonly IConsoleAction _actionPrint;
    private readonly IProducerSettingService _settingService;
    private readonly RabbitConfig _rabbitConfig;

    public StartApp(IMassTransitHelper helper, IConsoleAction actionPrint, IProducerSettingService settingService, IOptions<RabbitConfig> rabbitConfig)
    {
        _rabbitConfig = rabbitConfig?.Value ?? throw new ArgumentNullException(nameof(rabbitConfig));
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _actionPrint = actionPrint ?? throw new ArgumentNullException(nameof(actionPrint));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(actionPrint));
    }
    /// <summary>
    /// запуск всех процессов
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var task1 = _helper.ReceiveMessageAsync(_rabbitConfig);
        var task2 = GeneralProcess();
        await Task.WhenAll(task1, task2);
    }

    #region  Private methods
    /// <summary>
    /// запуск главного меню
    /// </summary>
    /// <returns></returns>
    private async Task GeneralProcess()
    {
        _actionPrint.PrintMessage(ProducerAppConstants.GetMenuText);
        while (true)
        {
            _actionPrint.PrintMessage(ProducerAppConstants.ChooseAction);
            await GetChooseByUserAsync();
        }
    }
    /// <summary>
    /// обработка выбора действия пользователем
    /// </summary>
    /// <returns></returns>
    private async Task GetChooseByUserAsync()
    {
        try
        {
            await MenuHelperAsync(GetByteNumber());
        }
        catch (Exception ex)
        {
            if (ex is ConsoleInputException consoleEx)
                _actionPrint.PrintMessage(consoleEx.Message);
        }
    }
    private async Task MenuHelperAsync(byte number)
    {
        switch (number)
        {
            case 1:
                await StartGameAsync();
                break;
            case 2:
                OpenSettings();
                break;
            default:
                throw new ConsoleInputException(ExceptionConstants.ConsoleInputExceptionMessage);
        }
    }
    /// <summary>
    /// игра
    /// </summary>
    /// <returns></returns>
    private async Task StartGameAsync()
    {
        bool flag = true;
        int counter = 0;
        SettingDto currentSettings = _settingService.GetSettings();
        _actionPrint.PrintMessage(ProducerAppConstants.GameDescriptionText);
        while (flag)
        {
            try
            {
                int attemptionCount = currentSettings.AttemptsCount - counter;
                _actionPrint.PrintMessage(string.Concat(ProducerAppConstants.AttemptionsCount, attemptionCount));
                _actionPrint.PrintMessage(string.Concat(ProducerAppConstants.EnterNumber, currentSettings.Start, ProducerAppConstants.EndLimit, currentSettings.End));
                await SendNumberAsync(currentSettings, counter);
                counter++;
                flag = StartGameHelper(counter,currentSettings);
            }
            catch (Exception ex)
            {
                if (ex is ConsoleInputException consoleEx)
                    _actionPrint.PrintMessage(consoleEx.Message);
            }
        }
        _actionPrint.PrintMessage(ProducerAppConstants.GetMenuText);
    }
    /// <summary>
    /// если количество попыток закончилось, либо число отгадано, то выходим из цикла
    /// </summary>
    /// <param name="counter"></param>
    /// <param name="currentSettings"></param>
    /// <returns></returns>
    private bool StartGameHelper(int counter, SettingDto currentSettings)
    {
        bool flag=true;
        if (counter == currentSettings.AttemptsCount)
            flag = false;
        if (ConsumerService.IsSuccess)
            flag = false;

        return flag;
    }
    /// <summary>
    /// отправка числа на сервер
    /// </summary>
    /// <param name="currentSettings"></param>
    /// <param name="attemptionNumber"></param>
    /// <returns></returns>
    private async Task SendNumberAsync(SettingDto currentSettings, int attemptionNumber)
    {
        int number = GetIntNumber();
        if ((number < currentSettings.Start) || (number > currentSettings.End))
            _actionPrint.PrintMessage(ProducerAppConstants.ErrorLimitNumber);
        else
        {
            #region without Saga
            await _helper.SendMessageAsync(new MessageDto(number, currentSettings.Start,
                                                          currentSettings.End, attemptionNumber,
                                                          currentSettings.AttemptsCount, String.Empty, false, Guid.NewGuid()), _rabbitConfig?.ProducerQueueName, _rabbitConfig);
            _actionPrint.PressKey();
            #endregion
            #region with Saga By Masstransit
            //string message = await _helper.SendMessageWithSagaAsync(new MessageDto(number, currentSettings.Start,
            //                                             currentSettings.End, attemptionNumber,
            //                                             currentSettings.AttemptsCount, String.Empty, false, Guid.NewGuid()));
            //_actionPrint.PrintMessage(message);
            #endregion

        }
    }
    /// <summary>
    /// получаем число (для отправки на сервер) с консоли
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ConsoleInputException"></exception>
    private int GetIntNumber()
    {
        bool flag = Int32.TryParse(_actionPrint.GetConsoleInput(), out var number);
        if (!flag)
            throw new ConsoleInputException(ExceptionConstants.ConsoleInputExceptionMessage);

        return number;
    }
    /// <summary>
    /// получаем действие пользователя с консоли
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ConsoleInputException"></exception>
    private byte GetByteNumber()
    {
        bool flag = byte.TryParse(_actionPrint.GetConsoleInput(), out var number);
        if (!flag)
            throw new ConsoleInputException(ExceptionConstants.ConsoleInputExceptionMessage);

        return number;
    }
    /// <summary>
    /// окно настроек
    /// </summary>
    private void OpenSettings()
    {
        bool flag = true;
        while (flag)
        {
            try
            {
                SettingDto currentSetting = _settingService.GetSettings();
                PrintSettingsDetails(currentSetting);
                flag = GetFlagBySettings(GetByteNumber(), currentSetting);
            }
            catch (Exception ex)
            {
                if (ex is ConsoleInputException consoleEx)
                    _actionPrint.PrintMessage(consoleEx.Message);
            }
        }
        _actionPrint.PrintMessage(ProducerAppConstants.GetMenuText);
    }
    /// <summary>
    /// меню настроек
    /// </summary>
    /// <param name="number"></param>
    /// <param name="currentSetting"></param>
    /// <returns></returns>
    /// <exception cref="ConsoleInputException"></exception>
    private bool GetFlagBySettings(byte number, SettingDto currentSetting)
    {
        bool flag = number switch
        {
            1 => EditLimitNumbersBySettings(currentSetting),
            2 => EditCountAttemptionsBySettings(currentSetting),
            3 => false,
            _ => throw new ConsoleInputException(ExceptionConstants.ConsoleInputExceptionMessage)
        };
        return flag;
    }
    /// <summary>
    /// описание настроект (вывод текста)
    /// </summary>
    /// <param name="currentSetting"></param>
    private void PrintSettingsDetails(SettingDto currentSetting)
    {
        _actionPrint.PrintMessage(ProducerAppConstants.SettingDescriptionText);
        string limitNumbers = string.Concat(ProducerAppConstants.StartLimit, currentSetting.Start,
            ProducerAppConstants.EndLimit, currentSetting.End);
        _actionPrint.PrintMessage(limitNumbers);
        string limitAttemptions = string.Concat(ProducerAppConstants.AttemptionsCount, currentSetting.AttemptsCount);
        _actionPrint.PrintMessage(limitAttemptions);
        _actionPrint.PrintMessage(ProducerAppConstants.GetSettingsMenuText);
        _actionPrint.PrintMessage(ProducerAppConstants.ChooseAction);
    }
    /// <summary>
    /// установка диапазона отгадываемых значений
    /// </summary>
    /// <param name="currentSetting"></param>
    /// <returns></returns>
    private bool EditLimitNumbersBySettings(SettingDto currentSetting)
    {
        _actionPrint.PrintMessage(ProducerAppConstants.UpdateStartLimit);
        currentSetting.Start = GetIntNumber();
        _actionPrint.PrintMessage(ProducerAppConstants.UpdateEndLimit);
        currentSetting.End = GetIntNumber();
        _settingService.UpdateSettings(currentSetting);
        _actionPrint.PrintMessage(ProducerAppConstants.UpdateLimitResult);
        return true;
    }
    /// <summary>
    /// установка количества попыток отгадывания числа
    /// </summary>
    /// <param name="currentSetting"></param>
    /// <returns></returns>
    private bool EditCountAttemptionsBySettings(SettingDto currentSetting)
    {
        _actionPrint.PrintMessage(ProducerAppConstants.UpdateAttemptionsCount);
        currentSetting.AttemptsCount = GetIntNumber();
        _settingService.UpdateSettings(currentSetting);
        _actionPrint.PrintMessage(ProducerAppConstants.UpdateAttemptionsCountResult);
        return true;
    }
    #endregion
}
