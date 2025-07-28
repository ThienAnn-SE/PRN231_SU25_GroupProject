using AppCore.Dtos;

namespace WebApi.Extension
{
    public class TestResource
    {
        public const int MBTIQuestionCount = 50;
        public const int DISCQuestionCount = 28;
        public const int OCEANQuestionCount = 10;
    }

    public static class TestEvaluationExtensions
    {
        private enum MBTIDimension
        {
            EvsI, SvsN, TvsF, JvsP
        }

        private enum DISCTrait
        {
            D, I, S, C
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

        private static DISCTrait GetDISCTraitByIndex(int index)
        {
            // Assume equal distribution: 7 questions per trait (7 * 4 = 28)
            return (index % 4) switch
            {
                0 => DISCTrait.D,
                1 => DISCTrait.I,
                2 => DISCTrait.S,
                3 => DISCTrait.C,
                _ => throw new InvalidOperationException()
            };
        }

        public static string EvaluateDiscTest(List<int> userAnswers)
        {
            if (userAnswers.Count != 28)
                throw new ArgumentException("Must provide exactly 28 answers.");

            var scores = new Dictionary<DISCTrait, int>
    {
        { DISCTrait.D, 0 },
        { DISCTrait.I, 0 },
        { DISCTrait.S, 0 },
        { DISCTrait.C, 0 }
    };

            for (int i = 0; i < userAnswers.Count; i++)
            {
                var trait = GetDISCTraitByIndex(i);
                scores[trait] += userAnswers[i]; // assumes 0 or 1 input
            }

            var dominant = scores.OrderByDescending(kvp => kvp.Value).First();
            return dominant.Key.ToString(); // returns "D", "I", "S", or "C"
        }

        public static string EvaluateOceanTest(List<int> userAnswers)
        {
            var scores = new Dictionary<string, int> { { "O", 0 }, { "C", 0 }, { "E", 0 }, { "A", 0 }, { "N", 0 } };
            var reverseScoredIndexes = new HashSet<int> { 1, 3, 5, 7, 9 };

            for (int i = 0; i < userAnswers.Count; i++)
            {
                string trait = GetTraitByQuestionIndex(i);
                int score = reverseScoredIndexes.Contains(i) ? 6 - userAnswers[i] : userAnswers[i];
                scores[trait] += score;
            }
            var result = GetDominantTrait(scores);
            if (result == "?")
            {
                throw new InvalidOperationException("Could not determine dominant trait.");
            }
            return result;
        }


        private static string GetDominantTrait(Dictionary<string, int> scores)
        {
            var dominant = scores.OrderByDescending(kvp => kvp.Value).First();
            return dominant.Key switch
            {
                "O" => "Openness",
                "C" => "Conscientiousness",
                "E" => "Extraversion",
                "A" => "Agreeableness",
                "N" => "Neuroticism",
                _ => "Unknown"
            };
        }


        private static string GetTraitByQuestionIndex(int index)
        {
            return index switch
            {
                0 or 1 => "O", // Openness
                2 or 3 => "C", // Conscientiousness
                4 or 5 => "E", // Extraversion
                6 or 7 => "A", // Agreeableness
                8 or 9 => "N", // Neuroticism
                _ => "?"
            };
        }
    }
}
