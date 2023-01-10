using UniRx;
using System;

namespace Tetris_UI
{
    public class ScoreModel
    {
        private static ReactiveProperty<int> score = new(0);
        private static int point = 100;

        public static void AddScore()
        {
            score.Value += point;
        }

        public int GetScorePoint()
        {
            return score.Value;
        }

        public IObservable<int> GetScore()
        {
            return score;
        }
    }
}