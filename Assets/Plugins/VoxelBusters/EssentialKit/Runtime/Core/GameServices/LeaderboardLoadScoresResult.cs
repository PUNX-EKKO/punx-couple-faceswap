using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.EssentialKit
{
    /// <summary>
    /// This class contains the information retrieved when load scores operation is completed.
    /// </summary>
    public class LeaderboardLoadScoresResult
    {
        #region Properties

        /// <summary>
        /// An array of score values.
        /// </summary>
        public IScore[] Scores { get; private set; }

        #endregion

        #region Constructors

        public LeaderboardLoadScoresResult(IScore[] scores)
        {
            // Set properties
            Scores  = scores;
        }

        #endregion
    }
}