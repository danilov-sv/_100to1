using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SL_HundredToOne
{
	public partial class BlackboardAnswer : UserControl
	{
		public static readonly DependencyProperty BlackboardTextProperty = DependencyProperty.Register(
			"BlackboardText", typeof(string), typeof(BlackboardAnswer),null);
		public static readonly DependencyProperty BlackboardScoreProperty = DependencyProperty.Register(
			"BlackboardScore", typeof(string), typeof(BlackboardAnswer),null);
		
		public string BlackboardText
		{
			get { return (string)GetValue(BlackboardTextProperty); }
    		set { SetValue(BlackboardTextProperty, value); }
		}
		
		public string BlackboardScore
		{
			get { return (string)GetValue(BlackboardScoreProperty); }
    		set { SetValue(BlackboardScoreProperty, value); }
		}
		
		public BlackboardAnswer()
		{
			// Required to initialize variables
			InitializeComponent();
		}
	}
}