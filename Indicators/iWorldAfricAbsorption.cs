#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class iWorldAfricAbsorption : Indicator
	{
		double lastPrice;
		int lastBar = -1;
		double lastVolume = -1;
		
		private Series<double> buyVolume;
	    private Series<double> sellVolume;
	    private Series<double> priceImbalance;
	    private Series<double> absorptionIndicator;
	    private Series<double> kpiStrength;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "iWorldAfric Absorption";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				
				//AddPlot(Brushes.DarkCyan, "Price Imbalance");
				AddPlot(Brushes.Maroon, "Absorption");
				//AddPlot(Brushes.Orange, "KPI Strength");
				imbalanceThreshold = 1; // Default value for imbalance threshold
                absorptionMovementThreshold = 0.1; // Default value for absorption movement threshold
                absorptionThreshold = 1000; // Default value for absorption volume threshold
			}
			else if (State == State.Configure)
			{
				AddDataSeries(BarsPeriodType.Tick, 1);
			
				buyVolume = new Series<double>(this);
				sellVolume = new Series<double>(this);
				priceImbalance = new Series<double>(this);
				absorptionIndicator = new Series<double>(this);
				kpiStrength = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if(CurrentBars[0] < 3 || CurrentBars[1] < 3)
				return;
			
			if(BarsInProgress == 1)
			{
				if(CurrentBars[0] != lastBar)
				{
					buyVolume[0] = 0;
					sellVolume[0] = 0;
					lastVolume = 0;
				}
				
				buyVolume[0] += CalculateBuyVolume();
				sellVolume[0] += CalculateSellVolume();
				priceImbalance[0] = DetectImbalances(buyVolume[0], sellVolume[0]);
				absorptionIndicator[0] = DetectAbsorption(buyVolume[0], sellVolume[0]);
				kpiStrength[0] = CalculateKPIStrength(buyVolume[0], sellVolume[0], Closes[0][0] - Opens[0][0]);
				
				//Values[0][0] = priceImbalance[0];
				Values[0][0] = absorptionIndicator[0];
				//Values[1][0] = kpiStrength[0];
				
				lastBar = CurrentBars[0];
				lastPrice = Closes[1][0];
				lastVolume = Volumes[1][0];
			}
		}
		
		
		private double CalculateBuyVolume()
	    {
	        if(Closes[1][0] > lastPrice)
				return Volumes[1][0];
			
			return 0;
	    }

	    private double CalculateSellVolume()
	    {
	        if(Closes[1][0] < lastPrice)
				return Volumes[1][0];
			
			return 0;
	    }
		
		
	    private double DetectImbalances(double buyVol, double sellVol)
		{
			// Prevent division by zero
			if (sellVol == 0) 
				return 0;

			double imbalanceRatio = buyVol / sellVol;
			// Use the configurable imbalanceThreshold property
			return imbalanceRatio > imbalanceThreshold ? imbalanceRatio : 0;
		}
		
	    private double DetectAbsorption(double buyVol, double sellVol)
		{
			// Check if there's a significant volume with little price movement
			double totalVolume = buyVol + sellVol;
			double priceMovement = Math.Abs(Closes[0][0] - Opens[0][0]);
			
			// Use the configurable absorptionMovementThreshold property
			return (totalVolume > absorptionThreshold && priceMovement < absorptionMovementThreshold) ? totalVolume : 0;
		}
		
		
	    private double CalculateKPIStrength(double buyVol, double sellVol, double priceMovement)
	    {
	        // KPI considering the volume and the actual price movement
	        double volumeWeight = (buyVol + sellVol) / (Volumes[0][0] == 0 ? 1 : Volumes[0][0]);
	        return volumeWeight * priceMovement;
	    }
		
		double imbalanceThreshold = 0;
		/*
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Imbalance Threshold", GroupName = "Parameters", Order = 10)]
		public double imbalanceThreshold
		{ get; set; }
		*/
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Absorption Threshold", GroupName = "Parameters", Order = 20)]
		public double absorptionThreshold
		{ get; set; }
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private iWorldAfricAbsorption[] cacheiWorldAfricAbsorption;
		public iWorldAfricAbsorption iWorldAfricAbsorption(double absorptionThreshold)
		{
			return iWorldAfricAbsorption(Input, absorptionThreshold);
		}

		public iWorldAfricAbsorption iWorldAfricAbsorption(ISeries<double> input, double absorptionThreshold)
		{
			if (cacheiWorldAfricAbsorption != null)
				for (int idx = 0; idx < cacheiWorldAfricAbsorption.Length; idx++)
					if (cacheiWorldAfricAbsorption[idx] != null && cacheiWorldAfricAbsorption[idx].absorptionThreshold == absorptionThreshold && cacheiWorldAfricAbsorption[idx].EqualsInput(input))
						return cacheiWorldAfricAbsorption[idx];
			return CacheIndicator<iWorldAfricAbsorption>(new iWorldAfricAbsorption(){ absorptionThreshold = absorptionThreshold }, input, ref cacheiWorldAfricAbsorption);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.iWorldAfricAbsorption iWorldAfricAbsorption(double absorptionThreshold)
		{
			return indicator.iWorldAfricAbsorption(Input, absorptionThreshold);
		}

		public Indicators.iWorldAfricAbsorption iWorldAfricAbsorption(ISeries<double> input , double absorptionThreshold)
		{
			return indicator.iWorldAfricAbsorption(input, absorptionThreshold);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.iWorldAfricAbsorption iWorldAfricAbsorption(double absorptionThreshold)
		{
			return indicator.iWorldAfricAbsorption(Input, absorptionThreshold);
		}

		public Indicators.iWorldAfricAbsorption iWorldAfricAbsorption(ISeries<double> input , double absorptionThreshold)
		{
			return indicator.iWorldAfricAbsorption(input, absorptionThreshold);
		}
	}
}

#endregion
