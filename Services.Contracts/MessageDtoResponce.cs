namespace Services.Contracts;
public class MessageDtoResponce:MessageDto
{
    public MessageDtoResponce(int number, int start, int end, int attemptionNumber, 
                              int attemptionCount, string content, bool isSuccess, Guid id) 
                             : base(number, start, end, attemptionNumber, attemptionCount,content, isSuccess, id)
    {

    }
}
