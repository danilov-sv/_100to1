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
	public partial class AnswerBilletControl : UserControl
	{
		public static readonly DependencyProperty BilletTextProperty = DependencyProperty.Register(
			"BilletText", typeof(string), typeof(AnswerBilletControl),null);
		
		public string BilletText
		{
			get { return (string)GetValue(BilletTextProperty); }
    		set { SetValue(BilletTextProperty, value); }
		}
		
		public AnswerBilletControl()
		{
			// Required to initialize variables
			InitializeComponent();

		}
		
		public void FlipToBack()
		{
            FrontSide.Projection.SetValue(PlaneProjection.RotationXProperty, -90.0);
            BackSide.Projection.SetValue(PlaneProjection.RotationXProperty, 0.0);
        }

        public void FlipToFront(string Text, string Score)
		{
			txtText.Text = Text.ToUpper();
			txtScore.Text = Score;
			(Resources["FlipBackToFront"] as Storyboard).Begin();
		}
	}
}