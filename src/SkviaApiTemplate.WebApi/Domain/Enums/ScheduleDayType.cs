namespace SkviaApiTemplate.WebApi.Domain.Enums;

public enum ScheduleDayType
{
    /// <summary>
    /// Día laborable regular / Turno normal de trabajo
    /// </summary>
    WorkDay = 1,

    /// <summary>
    /// Día de descanso programado
    /// </summary>
    DayOff = 2,

    /// <summary>
    /// Día para recuperar horas faltantes o feriados trabajados
    /// </summary>
    MakeUpDay = 3,

    /// <summary>
    /// Día no laborable por vacaciones del empleado
    /// </summary>
    Vacation = 4,

    /// <summary>
    /// Ausencia justificada por descanso médico o licencia
    /// </summary>
    MedicalLeave = 5
}