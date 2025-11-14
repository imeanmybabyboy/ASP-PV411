namespace ASP_PV411.Data.Entities
{
    public class UserRole
    {
        public string Id { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CanCreate { get; set; }
        public int CanRead { get; set; }
        public int CanUpdate { get; set; }
        public int CanDelete { get; set; }
    }
}

/*Задача роботи з користувачами: реєстрація, автентивікація, авторизація
автентифікація: перевірка особи і видача токена (посвідчення)
авторизація: перевірка токена та прийняття рішення про допуск чи відмову у допуску

[UserRoles]            [Users] 
 Id                     Id
 Description  int       Name       кумулятивне поле для персональних даних
 CanCreate    int       Email      ... для обов'язкових даних
 CanRead      int       Birthdate  ... для опціональних даних
 CanUpdate    int       Login (Unique)
 CanDelete    int       Salt
                        Dk
                        RegisterAt
                        DeleteAt
                        RoleId

[Tokens]
 Id
 UserId
 IssuedAt
 ExpiredAt
 */