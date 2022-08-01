namespace Handy2DTools.SpriteAnimations.Handlers
{
    public interface ISpriteAnimationCycleEndDealer
    {
        void OnAnimationCycleEnd(SpriteAnimation animation, SpriteAnimationCycleType cycleType);
    }
}