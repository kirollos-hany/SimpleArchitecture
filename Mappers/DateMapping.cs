using Mapster;

namespace SimpleArchitecture.Mappers;

public class DateMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DateTime, DateOnly>().MapWith((src) => DateOnly.FromDateTime(src));
        config.NewConfig<DateOnly, DateTime>().MapWith((src) => new DateTime(src.Year, src.Month, src.Day, 12, 0, 0));
        config.NewConfig<DateTime?, DateOnly?>().MapWith((src) => src == null ? null : DateOnly.FromDateTime(src.Value));
        config.NewConfig<DateOnly?, DateTime?>().MapWith((src) => src == null ? null : new DateTime(src.Value.Year, src.Value.Month, src.Value.Day, 12, 0, 0));
        config.NewConfig<DateTime?, DateOnly>().MapWith((src) => src == null ? default : DateOnly.FromDateTime(src.Value));
        config.NewConfig<DateOnly?, DateTime>().MapWith((src) => src == null ? default : new DateTime(src.Value.Year, src.Value.Month, src.Value.Day, 12, 0, 0));
        
        config.NewConfig<DateTime, TimeOnly>().MapWith((src) => TimeOnly.FromDateTime(src));
        config.NewConfig<DateTime?, TimeOnly?>().MapWith((src) => src == null ? null : TimeOnly.FromDateTime(src.Value));
        config.NewConfig<DateTime?, TimeOnly>().MapWith((src) => src == null ? default : TimeOnly.FromDateTime(src.Value));
    }
}