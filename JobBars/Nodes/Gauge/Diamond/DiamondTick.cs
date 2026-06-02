using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
using KamiToolKit.Timelines;
using System.Numerics;

namespace JobBars.Nodes.Gauge.Diamond {
    public unsafe class DiamondTick : SimpleOverlayNode {
        public readonly DiamondNode Parent;

        public readonly ImageNode Background;
        public readonly SimpleOverlayNode Container;
        public readonly SimpleOverlayNode DiamondContainer;
        public readonly ImageNode Diamond;
        public readonly TextNode Text;

        private bool PrevValue = false;

        public DiamondTick( DiamondNode parent ) {
            Size = new( 32, 32 );
            Parent = parent;

            Background = new SimpleImageNode() {
                Size = new( 32, 32 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 32, 32 ),
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackA.tex"
            };

            Container = new SimpleOverlayNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
            };

            DiamondContainer = new SimpleOverlayNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
            };

            Diamond = new SimpleImageNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
                TextureCoordinates = new( 32, 0 ),
                TextureSize = new( 32, 32 ),
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackA.tex"
            };

            Text = new TextNode() {
                Position = new( 0, 20 ),
                Size = new( 32, 32 ),
                FontSize = 14,
                LineSpacing = 14,
                TextColor = new( 1, 1, 1, 1 ),
                TextOutlineColor = new( 40f / 255f, 40f / 255f, 40f / 255f, 1 ),
                TextFlags = TextFlags.Glare,
            };
            Text.Node->AlignmentFontType = 4;

            Background.AttachNode( this );
            Container.AttachNode( this );

            DiamondContainer.AttachNode( Container );
            Text.AttachNode( Container );
            Diamond.AttachNode( DiamondContainer );

            Container.AddTimeline( new TimelineBuilder()
                .BeginFrameSet( 1, 10 ) // Pop in id = 1
                .AddLabel( 1, 1, AtkTimelineJumpBehavior.Start, 0 )
                .AddLabel( 10, 0, AtkTimelineJumpBehavior.PlayOnce, 0 )
                .EndFrameSet()
                .Build()
            );

            DiamondContainer.AddTimeline( new TimelineBuilder()
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

            Diamond.AddTimeline( new TimelineBuilder()
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
            Container.IsVisible = value;
            if( value && !PrevValue ) { // Now visible
                Container.Timeline?.PlayAnimation( 1 ); // Pop in
                Parent.Sync();
            }
            PrevValue = value;
        }

        public void SetColor( ElementColor color ) {
            Diamond.Timeline?.UpdateKeyFrame( 1, KeyFrameGroupType.Tint, addColor: color.AddColorKeyframe, multiplyColor: color.MultiplyColorKeyframe );
            Diamond.Timeline?.UpdateKeyFrame( 11, KeyFrameGroupType.Tint, addColor: color.AddColorKeyframe, multiplyColor: color.MultiplyColorKeyframe );
            Diamond.Timeline?.UpdateKeyFrame( 17, KeyFrameGroupType.Tint, addColor: color.AddColorKeyframe + new Vector3( 80f, 80f, 80f ), multiplyColor: color.MultiplyColorKeyframe );
            Diamond.Timeline?.UpdateKeyFrame( 46, KeyFrameGroupType.Tint, addColor: color.AddColorKeyframe, multiplyColor: color.MultiplyColorKeyframe );
        }
    }
}
