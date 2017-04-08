using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ReportDotNet.Playground.Template.PaymentOrderExample
{
	public class Data
	{
		public DateTime Date { get; set; }
		public decimal Sum { get; set; }

		public string LegalName { get; set; }
		public string Okpo { get; set; }
		public string DocumentNumber { get; set; }
		public string BossFio { get; set; }
		public string AccountantFio { get; set; }
		public string PostOfTheBoss { get; set; }
		public string CorrespondingAccountingRecord { get; set; }
		public string Contractor { set; get; }
		public string Cause { get; set; }
		public string Appendix { get; set; }

		public string DateStr => Date.ToShortDateString();

		public string Day => Date.Day.ToString().PadLeft(2, '0');

		public string Month => Date.ToString("MMMM", CultureInfo.InvariantCulture);

		public string Year => Date.Year.ToString();

		public string SumStr => Sum.ToString("F2");

		public string SumRubles => Math.Floor(Sum).ToString();

		public string SumCopecks => ((Sum - Math.Floor(Sum)) * 100).ToString().PadLeft(2, '0');

		public string SumInWords => "ясллю опнохяэч";

		public string LegalName33First => SplitIn2Lines(LegalName, 33)[0];

		public string LegalName33Second => SplitIn2Lines(LegalName, 33)[1];

		public string Contractor26First => SplitInManyLines(Contractor, 26, 35)[0];

		public string Contractor26Second => SplitInManyLines(Contractor, 26, 35)[1];

		public string Contractor26Third => SplitInManyLines(Contractor, 26, 35)[2];

		public string Contractor45First => SplitIn2Lines(Contractor, 45)[0];

		public string Contractor45Second => SplitIn2Lines(Contractor, 45)[1];

		public string Cause26First => SplitInManyLines(Cause, 26, 35)[0];

		public string Cause26Second => SplitInManyLines(Cause, 26, 35)[1];

		public string Cause26Third => SplitInManyLines(Cause, 26, 35)[2];

		public string Cause45First => SplitIn2Lines(Cause, 45)[0];

		public string Cause45Second => SplitIn2Lines(Cause, 45)[1];

		public string SumInWords36First => SplitIn2Lines(SumInWords, 36)[0];

		public string SumInWords36Second => SplitIn2Lines(SumInWords, 36)[1];

		private static string[] SplitInManyLines(string source, params int[] linesLength)
		{
			source = source ?? "";
			var first = linesLength.Aggregate(new[] { source }, (current, l) =>
																{
																	var r = current.LastOrDefault();
																	return current.Take(current.Length - 2).Concat(SplitIn2Lines(r, l)).ToArray();
																});
			var second = Enumerable.Repeat(0, linesLength.Length + 1);
			return EagerZip(first, second)
				.Select(x => x.Key)
				.ToArray();
			return new string[0];
		}

		private static string[] SplitIn2Lines(string source, int line1MaxLength)
		{
			source = source ?? "";
			var result = new string[2];
			var totalSumParts = source.Split(' ');
			result[0] = "";
			result[1] = "";

			foreach (var t in totalSumParts)
				if (result[0].Length + t.Length <= line1MaxLength)
					result[0] += t + ' ';
				else
					break;
			if (result[0].Length <= source.Length)
				result[1] = source.Substring(result[0].Length);

			for (var i = 0; i < result.Length; ++i)
				result[i] = result[i].Trim();
			return result;
		}

		private static IEnumerable<KeyValuePair<TFirst, TSecond>> EagerZip<TFirst, TSecond>(IEnumerable<TFirst> first,
																							IEnumerable<TSecond> second)
		{
			return EagerZip(first, second, (x, y) => new KeyValuePair<TFirst, TSecond>(x, y));
		}

		private static IEnumerable<TPair> EagerZip<TFirst, TSecond, TPair>(IEnumerable<TFirst> first,
																		   IEnumerable<TSecond> second,
																		   Func<TFirst, TSecond, TPair> createPair)
		{
			using (var secondEnumerator = second.GetEnumerator())
			{
				var secondExhaust = false;
				foreach (var firstItem in first)
					yield return createPair(firstItem,
											secondExhaust ||
											(secondExhaust = !secondEnumerator.MoveNext())
												? default(TSecond)
												: secondEnumerator.Current);
				while (secondEnumerator.MoveNext())
					yield return createPair(default(TFirst), secondEnumerator.Current);
			}
		}
	}
}