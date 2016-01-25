using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Resources;

namespace ResxTranslator.DAC
{
	partial class ResxTranslatorDataSet
	{
		public ResxTranslatorTableRow[] Select(string filter = "")
		{
			this.ResxTranslatorTable.CaseSensitive = true;
			return this.ResxTranslatorTable.Select(filter, "Name ASC") as ResxTranslatorTableRow[];
		}

		public void Add(ResXDataNode entry, bool isSource = true)
		{
			try
			{
				if (isSource)
				{
					// ソース
					this.ResxTranslatorTable.AddResxTranslatorTableRow(
						entry.Name,
						entry.GetValue((ITypeResolutionService)null).ToString(),
						string.Empty,
						entry.Comment
					);
				}
				else
				{
					// ターゲット
					this.ResxTranslatorTable.AddResxTranslatorTableRow(
						entry.Name,
						string.Empty,
						entry.GetValue((ITypeResolutionService)null).ToString(),
						entry.Comment
					);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public void Update(ResxTranslatorTableRow row, ResXDataNode entry, bool isSource = true)
		{
			try
			{
				if (null == row) return;

				if (isSource)
				{
					// ソース
					row.SourceValue = entry.GetValue((ITypeResolutionService)null).ToString();
				}
				else
				{
					// ターゲット
					row.TargetValue = entry.GetValue((ITypeResolutionService)null).ToString();
				}

				// コメント
				if (string.IsNullOrWhiteSpace(row.Comment))
				{
					row.Comment = entry.Comment;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public ResxTranslatorTableRow Get(object key)
		{
			var results = this.ResxTranslatorTable.Select(string.Format("Name = '{0}'", key));

			return ((null == results || 0 == results.Count()) ? null : results.First()) as ResxTranslatorTableRow;
		}

		public static System.Resources.ResXDataNode EncodeRow(ResxTranslatorTableRow row)
		{
			var node = new ResXDataNode(row.Name, row.TargetValue);
			node.Comment = row.Comment;
			return node;
		}

		public static System.Data.DataRow FindRow(
			System.Data.DataRow[] rows,
			int startindex,
			string searchText,
			bool isUpperVector,
			StringComparison strComp,
			bool isSource = true)
		{
			// 検索
			if (isUpperVector && 0 != startindex && startindex < rows.Count())
			{
				// 上に向かって検索
				for (int index = startindex - 1; index != 0; index--)
				{
					if (0 <= rows[index].GetTextValue(isSource).IndexOf(searchText, 0, strComp))
					{
						return rows[index];
					}
				}
			}
			else
			{
				// 下に向かって検索
				int initIndex = (0 > startindex) ? 0 : startindex + 1;
				for (int index = initIndex; index < rows.Count(); index++)
				{
					if (0 <= rows[index].GetTextValue(isSource).IndexOf(searchText, 0, strComp))
					{
						return rows[index];
					}
				}
			}

			return null;
		}
	}

	public static class ResxTranslatorTableRowExtension
	{
		public static string GetTextValue(this System.Data.DataRow row, bool isSource = true)
		{
			if (row is ResxTranslatorDataSet.ResxTranslatorTableRow)
			{
				return (isSource)
					? ((ResxTranslatorDataSet.ResxTranslatorTableRow)row).SourceValue
					: ((ResxTranslatorDataSet.ResxTranslatorTableRow)row).TargetValue;
			}

			return string.Empty;
		}
	}
}
