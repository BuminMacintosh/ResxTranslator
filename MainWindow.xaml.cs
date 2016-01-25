using ResxTranslator.Commands;
using ResxTranslator.Dialog;
using ResxTranslator.Translate;
using System;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ResxTranslator
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
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

		//private int CurrentIndex
		//{
		//	get
		//	{
		//		var _currentIndex = System.Windows.Application.Current.Properties["CurrentIndex"] as int?;
		//		if (null == _currentIndex)
		//		{
		//			_currentIndex = -1;
		//			System.Windows.Application.Current.Properties["CurrentIndex"] = _currentIndex;
		//		}
		//		return _currentIndex.Value;
		//	}
		//	set
		//	{
		//		System.Windows.Application.Current.Properties["CurrentIndex"] = value;
		//	}
		//}

		private bool IsUpdated
		{
			get
			{
				var _isUpdated = System.Windows.Application.Current.Properties["IsUpdated"] as bool?;
				if (null == _isUpdated)
				{
					_isUpdated = false;
					System.Windows.Application.Current.Properties["IsUpdated"] = _isUpdated;
				}
				return _isUpdated.Value;
			}
			set
			{
				System.Windows.Application.Current.Properties["IsUpdated"] = value;
			}
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		private void TargetCommand_CanExecute(object x, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (0 != this.Model.ResxTranslatorTable.Count());
			e.Handled = true;
		}

		private void EnabledUI(bool isEnable)
		{
			// プログレスバー
			this.progressBar.IsIndeterminate = !isEnable;

			// UI
			this.menu.IsEnabled = isEnable;
			this.contentList.IsEnabled = isEnable;
			this.txtComment.IsEnabled = isEnable;
			this.txtSource.IsEnabled = isEnable;
			this.txtTarget.IsEnabled = isEnable;
			this.btnTranslate.IsEnabled = isEnable;
		}

		private void LoadResxFile(bool isSourceFile = true)
		{
			var dlg = new OpenFileDialog();
			dlg.FilterIndex = 1;
			dlg.Filter = "Resx ファイル(.Resx)|*.Resx";
			var result = dlg.ShowDialog();

			if (result != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}

			System.Diagnostics.Debug.WriteLine("Selected File Path : {0}", new object[] { dlg.FileName });

			// UI操作を停止
			this.EnabledUI(false);

			// ファイル読み込み開始
			this.ReadResxAsync(dlg.FileName, isSourceFile)
				.ContinueWith(_ =>
				{
					this.Dispatcher.BeginInvoke(new Action(() =>
					{
						// UI操作を開始
						this.EnabledUI(true);

						// コンテンツツリーを更新
						this.ReloadContentList();
					}));
				});
		}

		private Task ReadResxAsync(string filePath, bool isSourceFile)
		{
			return Task.Factory.StartNew(() =>
			{
				var rsxr = new ResXResourceReader(filePath);
				rsxr.UseResXDataNodes = true;
				var dict = rsxr.GetEnumerator();
				while (dict.MoveNext())
				{
					var node = (ResXDataNode)dict.Value;
					var row = this.Model.Get(node.Name);
					if (null == row && isSourceFile)
					{
						this.Model.Add(node);
					}
					else
					{
						this.Model.Update(row, node, isSourceFile);
					}
				}
				rsxr.Close();
			});
		}

		private Task SaveResxAsync(string filePath)
		{
			return Task.Factory.StartNew(() =>
			{
				var rsxw = new ResXResourceWriter(filePath);

				this.Model.AcceptChanges();

				foreach (DAC.ResxTranslatorDataSet.ResxTranslatorTableRow row in this.Model.ResxTranslatorTable)
				{
					rsxw.AddResource(DAC.ResxTranslatorDataSet.EncodeRow(row));
				}

				rsxw.Close();
			});
		}

		private void OpenSource_Click(object sender, ExecutedRoutedEventArgs e)
		{
			this.LoadResxFile();
		}

		private void OpenTarget_Click(object sender, ExecutedRoutedEventArgs e)
		{
			this.LoadResxFile(false);
		}

		private void SaveTarget_Click(object sender, ExecutedRoutedEventArgs e)
		{
			var dlg = new SaveFileDialog();
			dlg.FilterIndex = 1;
			dlg.Filter = "Resx ファイル(.Resx)|*.Resx";
			var result = dlg.ShowDialog();

			if (result != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}

			System.Diagnostics.Debug.WriteLine("Selected File Path : {0}", new object[] { dlg.FileName });

			// UI操作を停止
			this.EnabledUI(false);

			// 保存処理を開始
			this.SaveResxAsync(dlg.FileName)
				.ContinueWith(_ =>
				{
					this.Dispatcher.BeginInvoke(new Action(() =>
					{
						// UI操作を開始
						this.EnabledUI(true);

						// 更新されたフラグをリセット
						this.IsUpdated = false;
					}));
				});
		}

		private void CopyFromSource_Click(object sender, ExecutedRoutedEventArgs e)
		{
			// UI操作を停止
			this.EnabledUI(false);

			Task.Factory.StartNew(() =>
			{
				var rows = this.Model.ResxTranslatorTable.Select("TargetValue = ''");
				foreach (DAC.ResxTranslatorDataSet.ResxTranslatorTableRow row in rows)
				{
					row.TargetValue = row.SourceValue;
				}
			})
			.ContinueWith(_ =>
			{
				this.Dispatcher.BeginInvoke(new Action(() =>
				{
					// UI操作を開始
					this.EnabledUI(true);

					// コンテンツツリーを更新
					this.ReloadContentList();
				}));
			});
		}

		private void AddToComment_Click(object sender, ExecutedRoutedEventArgs e)
		{
			// UI操作を停止
			this.EnabledUI(false);

			Task.Factory.StartNew(() =>
			{
				var rows = this.Model.ResxTranslatorTable.Select();
				foreach (DAC.ResxTranslatorDataSet.ResxTranslatorTableRow row in rows)
				{
					var isEmpty = string.IsNullOrWhiteSpace(row.Comment);
					if (isEmpty || 0 > row.Comment.IndexOf(row.SourceValue, StringComparison.CurrentCultureIgnoreCase))
					{
						row.Comment = row.Comment
									+ (isEmpty ? "" : Environment.NewLine)
									+ row.SourceValue;
					}
				}
			})
			.ContinueWith(_ =>
			{
				this.Dispatcher.BeginInvoke(new Action(() =>
				{
					// UI操作を開始
					this.EnabledUI(true);

					// コンテンツツリーを更新
					this.ReloadContentList();
				}));
			});
		}

		private void OpenFindTextDialog(string title, FindTextDialog.FindValueEventHandler foundValueHandler)
		{
			foreach (Window w in App.Current.Windows)
			{
				if (this == w.Owner && w.Title.StartsWith(title))
				{
					w.Activate();
					return;
				}
			}

			var dlg = new FindTextDialog()
			{
				Owner = this,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
			};
			dlg.FindValueEvent += foundValueHandler;
			dlg.Title = string.Format("{0} {1}", title, dlg.Title);
			dlg.Show();
		}

		private void FindSourceValue_Click(object sender, ExecutedRoutedEventArgs e)
		{
			this.OpenFindTextDialog("元テキスト", this.FindSourceValueEvent);
		}

		private void FindTargetValue_Click(object sender, ExecutedRoutedEventArgs e)
		{
			this.OpenFindTextDialog("翻訳テキスト", this.FindTargetValueEvent);
		}

		private void DisplayedTextValue(FindValueEventArgs e, bool isSource = true)
		{
			// UI操作を停止
			this.EnabledUI(false);

			var rows = this.Model.Select((this.menuUnTranslatedOnly.IsChecked)
											? "TargetValue = '' OR SourceValue = TargetValue"
											: "");

			Task.Factory.StartNew(index =>
			{
				// 検索
				return DAC.ResxTranslatorDataSet.FindRow(
					rows,
					(int)index,
					e.SearchText,
					e.IsUpperVector,
					e.StrComp,
					isSource
				);

			}, this.contentList.SelectedIndex)
			.ContinueWith(_ =>
			{
				this.Dispatcher.BeginInvoke(new Action<System.Data.DataRow>(row =>
				{
					// UI操作を開始
					this.EnabledUI(true);

					// 結果
					if (null != row)
					{
						// 行操作
						this.contentList.SelectedItem = row;
						this.contentList.ScrollIntoView(row);
					}
					else
					{
						string msg = string.Format("{0} は見つかりませんでした。", e.SearchText);

						System.Windows.MessageBox.Show(
										this,
										msg,
										this.Title,
										MessageBoxButton.OK,
										MessageBoxImage.Information);
					}
				}),
				_.Result);
			});
		}

		private void FindSourceValueEvent(object sender, FindValueEventArgs e)
		{
			this.DisplayedTextValue(e);
		}

		private void FindTargetValueEvent(object sender, FindValueEventArgs e)
		{
			this.DisplayedTextValue(e, false);
		}

		private void ReloadContentList(bool resetSelectIndex = true)
		{
			var rows = this.Model.Select((this.menuUnTranslatedOnly.IsChecked)
											? "TargetValue = '' OR SourceValue = TargetValue"
											: "");

			if (resetSelectIndex)
			{
				// 未選択へ戻す
				this.contentList.SelectedIndex = -1;

				// 編集中の内容を空にする
				this.txtKey.Text = string.Empty;
				this.txtSource.Text = string.Empty;
				this.txtTarget.Text = string.Empty;
				this.txtComment.Text = string.Empty;
			}

			// コンテンツツリーを更新
			this.contentList.ItemsSource = rows;

			// ステータスバーを更新
			this.stbCounter.Text = string.Format("データ数：{0} 件", rows.Count());
		}

		private void UnTranslatedOnly_Click(object sender, RoutedEventArgs e)
		{
			this.ReloadContentList();
		}

		private void contentList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			// 変更前のデータを保存
			if (1 == e.RemovedItems.Count)
			{
				var row = e.RemovedItems[0] as DAC.ResxTranslatorDataSet.ResxTranslatorTableRow;

				//row.SourceValue = this.txtSource.Text;
				if (!string.IsNullOrEmpty(this.txtTarget.Text)
					&& row.TargetValue != this.txtTarget.Text)
				{
					row.TargetValue = this.txtTarget.Text;

					// 更新された場合、ユーザー名と日付をコメントに付与
					this.txtComment.Text += string.IsNullOrWhiteSpace(this.txtComment.Text)
											? string.Empty
											: Environment.NewLine;
					this.txtComment.Text += string.Format("({0:yyyy-MM-dd HH:mm:ss}) {1}", DateTime.Now, Environment.UserName);

					// 更新されたフラグをセット
					this.IsUpdated = true;
				}

				row.Comment = this.txtComment.Text;
			}

			// 選択行のデータで画面を更新
			if (1 == e.AddedItems.Count)
			{
				var row = e.AddedItems[0] as DAC.ResxTranslatorDataSet.ResxTranslatorTableRow;

				this.txtKey.Text = row.Name;
				this.txtSource.Text = row.SourceValue;
				this.txtTarget.Text = row.TargetValue;
				this.txtComment.Text = row.Comment;
			}

			this.Model.AcceptChanges();
			this.ReloadContentList(false);
		}

		private void Translate_Click(object sender, RoutedEventArgs e)
		{
			var sourceText = this.txtSource.Text;
			if (string.IsNullOrWhiteSpace(sourceText)) return;

			// UI操作を停止
			this.EnabledUI(false);

			// 翻訳処理を開始
			Task.Factory.StartNew(() =>
			{
				return Translator.Translate(sourceText);
			})
			.ContinueWith(_ =>
			{
				this.Dispatcher.BeginInvoke(new Action<string>(result =>
				{
					this.txtTarget.Text = result;

					// UI操作を開始
					this.EnabledUI(true);

					// コンテンツツリーを更新
					this.ReloadContentList(false);
				}),
				_.Result);
			});
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string msg = "データ更新されていますが、ファイルに保存されていません。"
						+ Environment.NewLine
						+ "本当に閉じてもよろしいですか？";

			if (this.IsUpdated
				&& MessageBoxResult.No
					== System.Windows.MessageBox.Show(
							this,
							msg,
							"終了確認",
							MessageBoxButton.YesNo,
							MessageBoxImage.Question))
			{
				e.Cancel = true;
			}
		}
	}
}
