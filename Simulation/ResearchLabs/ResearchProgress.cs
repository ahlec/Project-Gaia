using System;
using Microsoft.Xna.Framework;

namespace Simulation.ResearchLabs
{
    public class ResearchProgress
    {
        public ResearchProgress(Research research)
        {
            this.research = research;
            researching = true;
        }

        private Research research;
        private TimeSpan researchTime;
        private bool researching;
        public Research Research { get { return research; } }
        public TimeSpan ElapsedTime { get { return researchTime; } }
        public void Update(GameTime gameTime)
        {
            researchTime += gameTime.ElapsedGameTime;
            if (researching && researchTime >= research.ResearchDuration)
            {
                researching = false;
                if (ResearchCompleted != null)
                    ResearchCompleted.Invoke(this);
            }
        }
        public event ResearchProgressCompletedHandler ResearchCompleted;
    }
    public delegate void ResearchProgressCompletedHandler(ResearchProgress researchProgress);
    public delegate void ResearchCompletedHandler(Research research);
}
