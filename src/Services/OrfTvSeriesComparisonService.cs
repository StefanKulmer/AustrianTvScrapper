using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesComparisonService
    {
        public IReadOnlyCollection<KeyValuePair<ComparisonResult, OrfTvSeries>> Compare(IReadOnlyCollection<OrfTvSeries> left, IReadOnlyCollection<OrfTvSeries> right)
        {
            var comparer = new OrfTvSeriesComparisonEqualityComparer();
            var firstNotSecond = left.Except(right, comparer);
            var secondNotFirst = right.Except(left, comparer);
            var both = left.Intersect(right, comparer);

            var final = new List<KeyValuePair<ComparisonResult, OrfTvSeries>>();
            final.AddRange(firstNotSecond.Select(x => new KeyValuePair<ComparisonResult, OrfTvSeries>(ComparisonResult.ExistsOnlyOnLeftSide, x)));
            final.AddRange(secondNotFirst.Select(x => new KeyValuePair<ComparisonResult, OrfTvSeries>(ComparisonResult.ExistsOnlyOnRightSide, x)));
            final.AddRange(both.Select(x => new KeyValuePair<ComparisonResult, OrfTvSeries>(ComparisonResult.ExistsOnBothSides, x)));

            final.Sort((a, b) => a.Value.Title.CompareTo(b.Value.Title));

            return final;
        }

        private class OrfTvSeriesComparisonEqualityComparer : IEqualityComparer<OrfTvSeries>
        {
            public bool Equals(OrfTvSeries x, OrfTvSeries y)
            {
                return string.Compare(x.Id, y.Id) == 0;
            }

            public int GetHashCode([DisallowNull] OrfTvSeries obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
