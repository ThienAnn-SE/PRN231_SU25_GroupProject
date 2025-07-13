using AppCore.Dtos;

namespace WebApi.Extension
{
    public static class TestEvaluationExtensions
    {
        private enum MBTIDimension
        {
            EvsI, SvsN, TvsF, JvsP
        }

        private enum MBTITrait
        {
            E, I,
            S, N,
            T, F,
            J, P
        }

        private static MBTIDimension GetDimensionByIndex(int index)
        {
            // Assume 50 questions: evenly distributed (12 or 13 per dimension)
            return (index % 4) switch
            {
                0 => MBTIDimension.EvsI,
                1 => MBTIDimension.SvsN,
                2 => MBTIDimension.TvsF,
                3 => MBTIDimension.JvsP,
                _ => throw new InvalidOperationException()
            };
        }

        private static MBTITrait GetTraitFromAnswer(int questionIndex, int answerIndex)
        {
            var dimension = GetDimensionByIndex(questionIndex);

            return dimension switch
            {
                MBTIDimension.EvsI => answerIndex == 0 ? MBTITrait.E : MBTITrait.I,
                MBTIDimension.SvsN => answerIndex == 0 ? MBTITrait.S : MBTITrait.N,
                MBTIDimension.TvsF => answerIndex == 0 ? MBTITrait.T : MBTITrait.F,
                MBTIDimension.JvsP => answerIndex == 0 ? MBTITrait.J : MBTITrait.P,
                _ => throw new InvalidOperationException()
            };
        }

        public static string EvaluateMBTI(List<int> userAnswers)
        {
            if (userAnswers.Count != 50)
                throw new ArgumentException("Must provide exactly 50 answers.");

            var traitScores = new Dictionary<MBTITrait, int>
            {
                { MBTITrait.E, 0 }, { MBTITrait.I, 0 },
                { MBTITrait.S, 0 }, { MBTITrait.N, 0 },
                { MBTITrait.T, 0 }, { MBTITrait.F, 0 },
                { MBTITrait.J, 0 }, { MBTITrait.P, 0 }
            };

            for (int i = 0; i < userAnswers.Count; i++)
            {
                var trait = GetTraitFromAnswer(i, userAnswers[i]);
                traitScores[trait]++;
            }

            return string.Concat(
                traitScores[MBTITrait.E] >= traitScores[MBTITrait.I] ? "E" : "I",
                traitScores[MBTITrait.S] >= traitScores[MBTITrait.N] ? "S" : "N",
                traitScores[MBTITrait.T] >= traitScores[MBTITrait.F] ? "T" : "F",
                traitScores[MBTITrait.J] >= traitScores[MBTITrait.P] ? "J" : "P"
            );
        }
    }
}
