namespace ResearchWork.IO.Models
{
    /// <summary>
    ///     Хи квадрат для данных значений
    /// </summary>
    /// <param name="N">Концентрация</param>
    /// <param name="Tkin">Кинетическая температура</param>
    /// <param name="N0">Лучевая концентрация</param>
    /// <param name="Tcmb">Температура РИ</param>
    /// <param name="X2">Хи квадрат</param>
    public readonly record struct CalculateX2(
        decimal N,
        decimal Tkin,
        decimal N0,
        decimal Tcmb,
        double X2);
}