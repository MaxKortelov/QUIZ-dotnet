using System;

public class DateService
{
    public string DateDifferenceFormatted(DateTime? dateFrom, DateTime? dateTo)
    {
        if (dateFrom == null || dateTo == null)
        {
            return "0 min 0 sec";
        }

        var duration = dateTo.Value - dateFrom.Value;
        var totalSeconds = (int)duration.TotalSeconds;
        var minutes = totalSeconds / 60;
        var seconds = totalSeconds % 60;

        return $"{minutes} min {seconds} sec";
    }
}