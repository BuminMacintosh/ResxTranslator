using System.Windows.Input;

namespace ResxTranslator.Commands
{
	public static class ResxTranslaterCommands
	{
		public static readonly RoutedUICommand Open = new RoutedUICommand
		(
			"元となるResxを開く...(_O)", "Open", typeof(ResxTranslaterCommands),
			new InputGestureCollection() { new KeyGesture(Key.O, ModifierKeys.Control) }
		);

		public static readonly RoutedUICommand New = new RoutedUICommand
		(
			"翻訳したResxを開く...(_N)", "New", typeof(ResxTranslaterCommands),
			new InputGestureCollection() { new KeyGesture(Key.N, ModifierKeys.Control) }
		);

		public static readonly RoutedUICommand Save = new RoutedUICommand
		(
			"翻訳したResxを保存...(_S)", "Save", typeof(ResxTranslaterCommands),
			new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Control) }
		);

		public static readonly RoutedUICommand CopyFromSource = new RoutedUICommand
		(
			"未翻訳テキストを元テキストで更新する(_C)", "Copy", typeof(ResxTranslaterCommands),
			new InputGestureCollection() { new KeyGesture(Key.C, ModifierKeys.Control) }
		);

		public static readonly RoutedUICommand AddToComment = new RoutedUICommand
		(
			"コメントに元テキストを追加する(_A)", "AddToComment", typeof(ResxTranslaterCommands),
			new InputGestureCollection() { new KeyGesture(Key.A, ModifierKeys.Control) }
		);

		public static readonly RoutedUICommand FindSource = new RoutedUICommand
		(
			"元テキストを検索する...(_F)", "FindSource", typeof(ResxTranslaterCommands),
			new InputGestureCollection() { new KeyGesture(Key.F, ModifierKeys.Control) }
		);

		public static readonly RoutedUICommand FindTarget = new RoutedUICommand
		(
			"翻訳テキストを検索する...(_T)", "AddToComment", typeof(ResxTranslaterCommands),
			new InputGestureCollection() { new KeyGesture(Key.T, ModifierKeys.Control) }
		);

	}
}
