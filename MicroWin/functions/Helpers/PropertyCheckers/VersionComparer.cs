using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.Helpers.PropertyCheckers
{
    public static class VersionComparer
    {
        /// <summary>
        /// Version constant for the GA release of Windows 10 22H2.
        /// </summary>
        public static readonly Version VERCONST_WIN10_22H2 = new(10, 0, 19045, 2130);
        /// <summary>
        /// Version constant for the GA release of Windows 11 21H2.
        /// </summary>
        public static readonly Version VERCONST_WIN11_21H2 = new(10, 0, 22000, 194);
        /// <summary>
        /// Version constant for the GA release of Windows 11 22H2.
        /// </summary>
        public static readonly Version VERCONST_WIN11_22H2 = new(10, 0, 22621, 382);
        /// <summary>
        /// Version constant for the GA release of Windows 11 23H2.
        /// </summary>
        public static readonly Version VERCONST_WIN11_23H2 = new(10, 0, 22631, 2428);
        /// <summary>
        /// Version constant for the GA release of Windows 11 24H2.
        /// </summary>
        public static readonly Version VERCONST_WIN11_24H2 = new(10, 0, 26100, 1742);
        /// <summary>
        /// Version constant for the GA release of Windows 11 25H2.
        /// </summary>
        public static readonly Version VERCONST_WIN11_25H2 = new(10, 0, 26200, 6584);
        /// <summary>
        /// Version constant for the GA release of Windows 11 26H1.
        /// </summary>
        public static readonly Version VERCONST_WIN11_26H1 = new(10, 0, 28000, 1575);

        /// <summary>
        /// Compares versions to determine if the source version is greater than or equal to the minimum threshold.
        /// </summary>
        /// <param name="versionToCompare">The reference version to compare</param>
        /// <param name="minimumThreshold">The lower threshold</param>
        /// <returns><see langword="true"/> if the version to compare is greater than or equal to the minimum threshold, <see langword="false"/> otherwise.</returns>
        public static bool IsNewerThanVersion(Version? versionToCompare, Version? minimumThreshold)
        {
            return versionToCompare >= minimumThreshold;
        }

        /// <summary>
        /// Compares versions to determine if the source version is lower than the maximum threshold.
        /// </summary>
        /// <param name="versionToCompare">The reference version to compare</param>
        /// <param name="maximumThreshold">The upper threshold</param>
        /// <returns><see langword="true"/> if the version to compare is lower than the maximum threshold, <see langword="false"/> otherwise.</returns>
        public static bool IsOlderThanVersion(Version? versionToCompare, Version? maximumThreshold)
        {
            return versionToCompare < maximumThreshold;
        }

        /// <summary>
        /// Compares versions to determine if the source version is between a specific version range.
        /// </summary>
        /// <param name="versionToCompare">The reference version to compare</param>
        /// <param name="lowerBound">The lower threshold</param>
        /// <param name="upperBound">The upper threshold</param>
        /// <returns><see langword="true"/> if the version to compare is between the specified range, <see langword="false"/> otherwise.</returns>
        public static bool IsBetweenVersionRange(Version? versionToCompare, Version? lowerBound, Version? upperBound)
        {
            return (versionToCompare >= lowerBound) && (versionToCompare < upperBound);
        }
    }
}
