namespace Services.Extensions.Exceptions;
public class ProducerAppConstants
{
    public const string GameDescriptionText = "Программа рандомно генерирует число, пользователь должен угадать это число.\n" +
                                              "При каждом вводе числа программа пишет больше или меньше отгадываемого.\n";
    public const string EnterNumber="Введите любое число в диапазоне от:";
    public const string ChooseAction = "Выберите действие:";
    public const string PressKey = "Нажмите любую клавишу";
    public const string GetMenuText = "1.Игра\n2.Настройки";
    public const string SettingDescriptionText = "Текущие Настройки:\n";
    public const string StartLimit = "Диапазон чисел от: ";
    public const string EndLimit = " до: ";
    public const string AttemptionsCount = "Количество попыток: ";
    public const string GetSettingsMenuText = "1.Редактировать диапазон чисел\n2.Редактировать количество попыток\n3.Главное меню";
    public const string UpdateStartLimit = "Укажите начало диапазона: ";
    public const string UpdateEndLimit = "Укажите конец диапазона: ";
    public const string UpdateLimitResult = "Значения диапазона чисел обновлены!";
    public const string UpdateAttemptionsCount = "Укажите количество попыток: ";
    public const string UpdateAttemptionsCountResult = "Количество попыток обновлено!";
    public const string ErrorLimitNumber = "Число не попадает в заданный диапазон";
}

