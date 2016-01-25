using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ResxTranslator.Dialog
{
	/// <summary>
	/// FindTextDialog.xaml の相互作用ロジック
	/// </summary>
	public partial class FindTextDialog : Window
	{
		private DAC.ResxTranslatorDataSet Model
		{
			get
			{
				var _model = System.Windows.Application.Current.Properties["DAC"] as DAC.ResxTranslatorDataSet;
				if (null == _model)
				{
					_model = new DAC.ResxTranslatorDataSet();
					System.Windows.Application.Current.Properties["DAC"] = _model;
				}
				return _model;
			}
			set
			{
				System.Windows.Application.Current.Properties["DAC"] = value;
			}
		}

		private int CurrentIndex
		{
			get
			{
				var _currentIndex = System.Windows.Application.Current.Properties["CurrentIndex"] as int?;
				if (null == _currentIndex)
				{
					_currentIndex = -1;
					System.Windows.Application.Current.Properties["CurrentIndex"] = _currentIndex;
				}
				return _currentIndex.Value;
			}
			set
			{
				System.Windows.Application.Current.Properties["CurrentIndex"] = value;
			}
		}

		/// <summary>
		/// 検索結果が見つかったイベントハンドラのデリゲート
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void FindValueEventHandler(object sender, FindValueEventArgs e);

		/// <summary>
		/// 検索結果が見つかったイベント
		/// </summary>
		public event FindValueEventHandler FindValueEvent;

		public FindTextDialog()
		{
			InitializeComponent();

			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, FindTargetValue_Click, FindCommand_CanExecute));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, Cancel_Click));
		}

		private void Cancel_Click(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}

		private void FindCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !string.IsNullOrWhiteSpace(this.txtFindValue.Text);
			e.Handled = true;
		}

		private void FindTargetValue_Click(object sender, ExecutedRoutedEventArgs e)
		{
			this.FindValueEvent(this, new FindValueEventArgs()
			{
				SearchText = this.txtFindValue.Text,
				IsUpperVector = this.rdoUp.IsChecked.Value,
				StrComp = (this.chkCaseSensitive.IsChecked.Value)
						? StringComparison.CurrentCulture
						: StringComparison.CurrentCultureIgnoreCase
			});
		}
	}

	public class FindValueEventArgs : EventArgs
	{
		public bool IsUpperVector { get; internal set; }
		public string SearchText { get; internal set; }
		public StringComparison StrComp { get; internal set; }
	}
}
