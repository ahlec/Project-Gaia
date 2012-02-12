using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.GUI;

namespace Simulation.ResearchLabs
{
    public class ScreenItemResearchProgress : ScreenItemProgressBar
    {
        public ScreenItemResearchProgress(Game game, ApplicationSkin skin, int x, int y,
            int width, ResearchProgress researchProgress)
            : base(game, skin, x, y, width)
        {
            this.ResearchProgress = researchProgress;
        }
        private ResearchProgress researchProgress;
        private bool fading = false;
        private float previousFadingValue = .1f;
        public ResearchProgress ResearchProgress
        {
            get { return researchProgress; }
            set
            {
                researchProgress = value;
                if (researchProgress != null)
                {
                    CurrentValue = 0;
                    MaxValue = (int)researchProgress.Research.ResearchDuration.TotalMilliseconds;
                    fading = false;
                    previousFadingValue = .1f;
                    base.Opacity = 100;
                    researchProgress.ResearchCompleted += delegate(ResearchLabs.ResearchProgress progress) { fading = true; };
                }
                else
                    Tooltip = null;
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (ResearchProgress != null)
            {
                base.Draw(gameTime, spriteBatch);
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (fading)
            {
                base.Opacity -= previousFadingValue;
                previousFadingValue *= 2;
                if (base.Opacity <= 0)
                {
                    fading = false;
                    base.Opacity = 0;
                    if (FadingCompleted != null)
                        FadingCompleted.Invoke();
                }
            }
            if (ResearchProgress != null)
            {
                CurrentValue = (researchProgress.ElapsedTime < researchProgress.Research.ResearchDuration ?
                    (int)researchProgress.ElapsedTime.TotalMilliseconds : MaxValue);
                TimeSpan timeLeft = researchProgress.Research.ResearchDuration.Subtract(researchProgress.ElapsedTime);
                Tooltip = researchProgress.Research.Name + " (Time left: " + Math.Ceiling(timeLeft.TotalSeconds).ToString() +
                    " second" + (Math.Ceiling(timeLeft.TotalSeconds) != 1d ? "s" : "") + ")";
            }
            base.Update(gameTime);
        }
        public event FadingCompletedHandler FadingCompleted;
    }
    public delegate void FadingCompletedHandler();
}
