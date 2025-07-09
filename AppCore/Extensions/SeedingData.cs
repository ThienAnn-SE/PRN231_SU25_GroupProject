using AppCore.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppCore.Extensions
{
    public static class SeedingData
    {
        public static void SeedData(this MigrationBuilder migrationBuilder)
        {
            var MBTI_Id = Guid.NewGuid();
            var OCEAN_Id = Guid.NewGuid();
            var DISC_Id = Guid.NewGuid();
            migrationBuilder.InsertData(
            table: "PersonalityTypes",
            columns: new[] { nameof(PersonalityType.Id), nameof(PersonalityType.Name), nameof(PersonalityType.Description), nameof(PersonalityType.CreatorId), nameof(PersonalityType.EditorId), nameof(PersonalityType.CreatedAt), nameof(PersonalityType.UpdatedAt), nameof(PersonalityType.DeletedAt) },
            values: new object[,]
                {
                    {
                        MBTI_Id,
                        "MBTI",
                        "The Myers-Briggs Type Indicator (MBTI) is a personality classification system that identifies individuals based on four key psychological preferences: how they focus their energy (Extraversion or Introversion), how they perceive information (Sensing or Intuition), how they make decisions (Thinking or Feeling), and how they interact with the external world (Judging or Perceiving).",
                        null,
                        null,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        null
                    },
                    {
                        OCEAN_Id,
                        "OCEAN",
                        "The OCEAN model, also known as the Big Five personality traits, is a psychological framework that describes human personality through five broad dimensions: Openness, Conscientiousness, Extraversion, Agreeableness, and Neuroticism. Openness reflects a person's creativity and willingness to try new experiences; Conscientiousness indicates self-discipline and reliability; Extraversion measures sociability and energy levels; Agreeableness reflects kindness and cooperativeness; and Neuroticism assesses emotional stability and the tendency to experience negative emotions. This model is widely used in psychology to understand personality differences and predict behavior across various life domains.",
                        null,
                        null,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        null
                    },
                    {
                        DISC_Id,
                        "DISC",
                        "The DISC personality model is a behavioral assessment tool that categorizes individuals into four primary personality traits: Dominance, Influence, Steadiness, and Conscientiousness. People high in Dominance are assertive, results-oriented, and driven by challenges. Those high in Influence are outgoing, persuasive, and focused on social interactions. Individuals with high Steadiness are patient, dependable, and value stability and cooperation. High Conscientiousness reflects a preference for accuracy, structure, and quality work. The DISC model is commonly used in workplaces to improve communication, teamwork, and leadership by helping people understand and adapt to different behavioral styles.",
                        null,
                        null,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        null
                    }
                }
            );
            // Seed data for Personalities
            migrationBuilder.InsertData(
                table: "Personalities",
                columns: new[] { "Id", "Name", "Description", "PersonalityTypeId", "CreatorId", "EditorId", "CreatedAt", "UpdatedAt", "DeletedAt" },
                values: new object[,]
                {
                    // MBTI: 16 types
                    { Guid.NewGuid(), "ISTJ", "Responsible, serious, and detail-oriented. Values tradition and loyalty.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ISFJ", "Loyal, considerate, and committed to helping others.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "INFJ", "Idealistic, insightful, and driven by a sense of purpose.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "INTJ", "Strategic and logical thinkers with a focus on long-term goals.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ISTP", "Practical, analytical, and enjoys solving real-world problems.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ISFP", "Sensitive, artistic, and in tune with the present moment.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "INFP", "Idealistic and empathetic, values authenticity and personal growth.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "INTP", "Curious, analytical, and loves exploring abstract concepts.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ESTP", "Energetic, pragmatic, and action-oriented problem solvers.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ESFP", "Spontaneous, outgoing, and loves engaging with people.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ENFP", "Enthusiastic, imaginative, and driven by meaningful connections.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ENTP", "Inventive, curious, and thrives on debate and innovation.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ESTJ", "Organized, practical, and values efficiency and structure.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ESFJ", "Warm, reliable, and focused on community and harmony.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ENFJ", "Charismatic, empathetic, and thrives on helping others grow.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "ENTJ", "Bold, confident leaders who love setting and achieving goals.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // OCEAN: Big Five Traits
                    { Guid.NewGuid(), "Openness", "Creative, imaginative, and open to new experiences and ideas.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "Conscientiousness", "Disciplined, organized, and reliable in achieving goals.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "Extraversion", "Outgoing, energetic, and enjoys being around people.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "Agreeableness", "Compassionate, cooperative, and values getting along with others.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "Neuroticism", "Tends to experience emotional instability, anxiety, and mood swings.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // DISC: 4 Traits
                    { Guid.NewGuid(), "Dominance", "Assertive, results-driven, and enjoys challenges and control.", DISC_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "Influence", "Sociable, persuasive, and enjoys interacting with others.", DISC_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "Steadiness", "Calm, dependable, and values cooperation and stability.", DISC_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), "Conscientiousness", "Detail-oriented, accurate, and values quality and standards.", DISC_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null }
                }
            );
        }
    }
}
