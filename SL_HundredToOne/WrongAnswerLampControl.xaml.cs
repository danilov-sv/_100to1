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
	public partial class WrongAnswerLampControl : UserControl
	{
		private System.Windows.Media.Effects.DropShadowEffect dropShadowEffect;
		public WrongAnswerLampControl()
		{
			// Required to initialize variables
			InitializeComponent();
			dropShadowEffect = new System.Windows.Media.Effects.DropShadowEffect();
				dropShadowEffect.Opacity = 1;
				dropShadowEffect.ShadowDepth = 0;
				dropShadowEffect.BlurRadius = 51;
				dropShadowEffect.Color = Color.FromArgb(255, 251, 241, 97);
		}
		
		public void TurnOn()
		{
			LampRectangle.Stroke= new SolidColorBrush(Colors.White);
			LampRectangle.Fill = new SolidColorBrush(Color.FromArgb(255,255, 239, 135));
			LampRectangle.Effect = dropShadowEffect;
		}
		
		public void TurnOff()
		{
			LampRectangle.Stroke= new SolidColorBrush(Color.FromArgb(255,222, 180, 6));
			LampRectangle.Fill = new SolidColorBrush(Color.FromArgb(255,222, 180, 6));
			LampRectangle.Effect = null;			
		}
	}
}