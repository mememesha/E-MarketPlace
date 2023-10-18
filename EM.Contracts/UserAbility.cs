namespace EM.Contracts
{
    [Flags]
    public enum UserAbility
    {
        None = 0,
        /// <summary> владелец организации. может всё. Но не является <see cref="Responsible"/></summary>
        Owner = 1 << 0,
        /// <summary> может изменять профиль компании, её участников, раздавать права, создавать склады </summary>
        Administrate = 1 << 1,
        /// <summary> может формировать предложения покупки/продажи </summary>
        Marketing = 1 << 2,
        /// <summary> может подписывать договоры </summary>
        SaleSigner = 1 << 3,
        /// <summary> публичный представитель компании </summary>
        Responsible = 1 << 4,
    }
}