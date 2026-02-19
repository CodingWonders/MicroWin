using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.Helpers.PropertyCheckers
{
    public static class VersionComparer
    {
        public static readonly Version VERCONST_WIN10_22H2 = new(10, 0, 19045, 2130);
        public static readonly Version VERCONST_WIN11_21H2 = new(10, 0, 22000, 194);
        public static readonly Version VERCONST_WIN11_22H2 = new(10, 0, 22621, 382);
        public static readonly Version VERCONST_WIN11_23H2 = new(10, 0, 22631, 2428);
        public static readonly Version VERCONST_WIN11_24H2 = new(10, 0, 26100, 1742);
        public static readonly Version VERCONST_WIN11_25H2 = new(10, 0, 26200, 6584);
        public static readonly Version VERCONST_WIN11_26H1 = new(10, 0, 28000, 1575);

        public static bool IsNewerThanVersion(Version versionToCompare, Version minimumThreshold)
        {
            return versionToCompare >= minimumThreshold;
        }

        public static bool IsOlderThanVersion(Version versionToCompare, Version maximumThreshold)
        {
            return versionToCompare < maximumThreshold;
        }

        public static bool IsBetweenVersionRange(Version versionToCompare, Version lowerBound, Version upperBound)
        {
            return (versionToCompare >= lowerBound) && (versionToCompare < upperBound);
        }
    }
}
