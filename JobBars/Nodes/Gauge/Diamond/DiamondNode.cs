using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using JobBars.Gauges.Types.Diamond;
using KamiToolKit.Timelines;
using System.Collections.Generic;

namespace JobBars.Nodes.Gauge.Diamond {
    public unsafe class DiamondNode : GaugeNode {
        public readonly List<DiamondTick> Ticks = [];

        public static readonly int MAX_ITEMS = 12;

        public DiamondNode() : base() {
            Size = new( 160, 46 );

            for( var idx = 0; idx < MAX_ITEMS; idx++ ) {
                var tick = new DiamondTick( this ) {
                    Position = new( 20 * idx, 0 )
                };
                Ticks.Add( tick );
            }

            Ticks.ForEach( x => x.AttachNode( this ) );
        }

        public void Sync() {
            foreach( var tick in Ticks ) {
                tick.DiamondContainer.Timeline?.PlayAnimation( JobBars.Configuration.GaugePulse ? 101 : 17 ); // Either play pulse or solid color
            }
        }

        public void SetMaxValue( int value ) {
            for( var idx = 0; idx < MAX_ITEMS; idx++ ) Ticks[idx].IsVisible = idx < value;
        }

        public void SetTextVisible( bool showText ) {
            SetSpacing( showText ? 5 : 0 );
            foreach( var tick in Ticks ) tick.Text.IsVisible = showText;
        }

        private void SetSpacing( int spacing ) {
            for( var idx = 0; idx < MAX_ITEMS; idx++ ) Ticks[idx].X = ( 20 + spacing ) * idx;
        }

        public void SetColor( int idx, ElementColor color ) => Ticks[idx].SetColor( color );

        public void SetValue( int idx, bool value ) => Ticks[idx].SetValue( value );

        public void SetText( int idx, string text ) {
            if( text == null ) return;
            Ticks[idx].Text.String = text;
            Ticks[idx].Text.IsVisible = true;
        }

        public void ShowText( int idx ) {
            Ticks[idx].Text.IsVisible = true;
        }

        public void HideText( int idx ) {
            Ticks[idx].Text.IsVisible = false;
        }

        // ====================

        public void Tick( IGaugeDiamondInterface tracker ) {
            SetVisible( !tracker.GetConfig().HideWhenInactive || tracker.GetActive() );
            SetScale( tracker.GetConfig().Scale );

            SetMaxValue( tracker.GetTotalMaxTicks() );
            SetTextVisible( tracker.GetDiamondTextVisible() );

            for( var i = 0; i < tracker.GetCurrentMaxTicks(); i++ ) {
                var trackerIndex = tracker.GetReverseFill() ? ( tracker.GetCurrentMaxTicks() - i - 1 ) : i;
                SetValue( i, tracker.GetTickValue( trackerIndex ) );
                SetColor( i, tracker.GetTickColor( trackerIndex ) );
                if( tracker.GetDiamondTextVisible()) {
                    SetText( i, tracker.GetDiamondText( trackerIndex ) );
                }
            }
        }

        public int GetHeight( IGaugeDiamondInterface tracker ) => ( int )( tracker.GetConfig().Scale * ( tracker.GetDiamondTextVisible() ? 40 : 32 ) );

        public int GetWidth( IGaugeDiamondInterface tracker ) => ( int )( tracker.GetConfig().Scale * ( 32 + 20 * ( tracker.GetCurrentMaxTicks() - 1 ) ) );
    }
}
