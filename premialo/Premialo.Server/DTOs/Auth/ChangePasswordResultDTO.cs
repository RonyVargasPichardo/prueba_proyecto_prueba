namespace Premialo.server.DTOs.Auth
{
    public class ChangePasswordResultDTO: OperationResult<object>
    {
        public ChangePasswordResultDTO(bool success, string? message) : base(success, message)
        {

        }
        private new readonly object Data;
    }
}
