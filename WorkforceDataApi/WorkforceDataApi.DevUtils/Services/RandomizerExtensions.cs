using Bogus;
using TeacherIdentity.AuthServer.Models;

namespace WorkforceDataApi.DevUtils.Services;

public static class RandomizerExtensions
{
    public static string LocalAuthorityNumber(this Randomizer randomizer)
    {
        return randomizer.Number(0, 999).ToString("000");
    }

    public static string EstablishmentNumber(this Randomizer randomizer)
    {
        return randomizer.Number(0, 9999).ToString("0000");
    }

    public static FullOrPartTimeIndicatorType FullOrPartTimeIndicator(this Randomizer randomizer)
    {
        // Distribution we'll use is:
        // 90% FullTimeEmployment
        // 8% PartTimeRegular
        // 2% IrregularPartTime
        var weight = randomizer.Number(1, 100);
        FullOrPartTimeIndicatorType indicator;
        if (weight < 3)
        {
            indicator = FullOrPartTimeIndicatorType.IrregularPartTime;
        }
        else if (weight < 9)
        {
            indicator = FullOrPartTimeIndicatorType.PartTimeRegular;
        }
        else
        {
            indicator = FullOrPartTimeIndicatorType.FullTimeEmployment;
        }

        return indicator;
    }

    public static string TeachingStatus(this Randomizer randomizer)
    {
        // Distribution we'll use is:
        // 99% A - Active
        // 1%  E - Re-employed        
        var weight = randomizer.Number(1, 100);
        if (weight < 2)
        {
            return "E";
        }

        return "A";
    }
}
