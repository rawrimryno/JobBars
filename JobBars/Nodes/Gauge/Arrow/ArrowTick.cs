using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
using KamiToolKit.Timelines;
using System.Numerics;

namespace JobBars.Nodes.Gauge.Arrow {
    public class ArrowTick : SimpleOverlayNode {
        public readonly ArrowNode Parent;

        public readonly ImageNode Background;
        public readonly SimpleOverlayNode ArrowContainer;
        public readonly ImageNode Arrow;

        private bool PrevValue = false;

        public ArrowTick( ArrowNode parent ) {
            Parent = parent;
            Size = new( 32, 32 );

            Background = new SimpleImageNode() {
                Size = new( 32, 32 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 32, 32 ),
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackB.tex"
            };

            ArrowContainer = new SimpleOverlayNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
            };

            Arrow = new SimpleImageNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
                TextureCoordinates = new( 32, 0 ),
                TextureSize = new( 32, 32 ),
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackB.tex"
            };

            Background.AttachNode( this );
            ArrowContainer.AttachNode( this );

            Arrow.AttachNode( ArrowContainer );

            AddTimeline( new TimelineBuilder()
                .BeginFrameSet( 1, 10 ) // Pop in id = 1
                .AddLabel( 1, 1, AtkTimelineJumpBehavior.Start, 0 )
                .AddLabel( 10, 0, AtkTimelineJumpBehavior.PlayOnce, 0 )
                .EndFrameSet()
                .Build()
            );

            ArrowContainer.AddTimeline( new TimelineBuilder()
                .BeginFrameSet( 1, 10 )
                .AddFrame( 1, scale: new Vector2( 2.5f, 2.5f ), alpha: 0, addColor: new Vector3( 80f, 80f, 80f ) )
                .AddFrame( 5, scale: new Vector2( 1f, 1f ), alpha: 255, addColor: new Vector3( 0f, 0f, 0f ) )
                .EndFrameSet()

                .BeginFrameSet( 1, 46 )
                .AddLabel( 1, 17, AtkTimelineJumpBehavior.Start, 0 ) // Solid color
                .AddLabel( 10, 0, AtkTimelineJumpBehavior.PlayOnce, 0 )
                .AddLabel( 11, 101, AtkTimelineJumpBehavior.Start, 0 ) // Loop
                .AddLabel( 46, 0, AtkTimelineJumpBehavior.LoopForever, 101 )
                .EndFrameSet()

                .Build()
            );

            Arrow.AddTimeline( new TimelineBuilder()
                .BeginFrameSet( 1, 46 )
                .AddFrame( 1, addColor: new Vector3( 120f, -50f, -50f ) )
                .AddFrame( 11, addColor: new Vector3( 120f, -50f, -50f ) )
                .AddFrame( 17, addColor: new Vector3( 200f, 30f, 30f ) )
                .AddFrame( 46, addColor: new Vector3( 120f, -50f, -50f ) )
                .EndFrameSet()
                .Build()
            );
        }

        public void SetValue( bool value ) {
            Arrow.IsVisible = value;
            if( value && !PrevValue ) { // Now visible
                Timeline?.PlayAnimation( 1 ); // Pop in
                Parent.Sync();
            }
            PrevValue = value;
        }

        public void SetColor( ElementColor color ) {
            Arrow.Timeline?.UpdateKeyFrame( 1, KeyFrameGroupType.Tint, addColor: color.AddColorKeyframe, multiplyColor: color.MultiplyColorKeyframe );
            Arrow.Timeline?.UpdateKeyFrame( 11, KeyFrameGroupType.Tint, addColor: color.AddColorKeyframe, multiplyColor: color.MultiplyColorKeyframe );
            Arrow.Timeline?.UpdateKeyFrame( 17, KeyFrameGroupType.Tint, addColor: color.AddColorKeyframe + new Vector3( 80f, 80f, 80f ), multiplyColor: color.MultiplyColorKeyframe );
            Arrow.Timeline?.UpdateKeyFrame( 46, KeyFrameGroupType.Tint, addColor: color.AddColorKeyframe, multiplyColor: color.MultiplyColorKeyframe );
        }
    }
}
