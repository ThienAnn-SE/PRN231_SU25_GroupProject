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
            var ISTJ_Id = Guid.NewGuid();
            var ISFJ_Id = Guid.NewGuid();
            var INFJ_Id = Guid.NewGuid();
            var INTJ_Id = Guid.NewGuid();
            var ISTP_Id = Guid.NewGuid();
            var ISFP_Id = Guid.NewGuid();
            var INFP_Id = Guid.NewGuid();
            var INTP_Id = Guid.NewGuid();
            var ESTP_Id = Guid.NewGuid();
            var ESFP_Id = Guid.NewGuid();
            var ENFP_Id = Guid.NewGuid();
            var ENTP_Id = Guid.NewGuid();
            var ESTJ_Id = Guid.NewGuid();
            var ESFJ_Id = Guid.NewGuid();
            var ENFJ_Id = Guid.NewGuid();
            var ENTJ_Id = Guid.NewGuid();
            // OCEAN: Big Five Traits
            var Openness_Id = Guid.NewGuid();
            var Conscientiousness_Ocean_Id = Guid.NewGuid(); // Avoid name clash with DISC
            var Extraversion_Id = Guid.NewGuid();
            var Agreeableness_Id = Guid.NewGuid();
            var Neuroticism_Id = Guid.NewGuid();

            // DISC: 4 Traits
            var Dominance_Id = Guid.NewGuid();
            var Influence_Id = Guid.NewGuid();
            var Steadiness_Id = Guid.NewGuid();
            var Conscientiousness_Disc_Id = Guid.NewGuid(); // Distinct from OCEAN

            // Seed data for Personalities
            migrationBuilder.InsertData(
                table: "Personalities",
                columns: new[] { "Id", "Name", "Description", "PersonalityTypeId", "CreatorId", "EditorId", "CreatedAt", "UpdatedAt", "DeletedAt" },
                values: new object[,]
                {
                    // MBTI: 16 types
                    { ISTJ_Id, "ISTJ", "Responsible, serious, and detail-oriented. Values tradition and loyalty.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ISFJ_Id, "ISFJ", "Loyal, considerate, and committed to helping others.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { INFJ_Id, "INFJ", "Idealistic, insightful, and driven by a sense of purpose.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { INTJ_Id, "INTJ", "Strategic and logical thinkers with a focus on long-term goals.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ISTP_Id, "ISTP", "Practical, analytical, and enjoys solving real-world problems.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ISFP_Id, "ISFP", "Sensitive, artistic, and in tune with the present moment.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { INFP_Id, "INFP", "Idealistic and empathetic, values authenticity and personal growth.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { INTP_Id, "INTP", "Curious, analytical, and loves exploring abstract concepts.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ESTP_Id, "ESTP", "Energetic, pragmatic, and action-oriented problem solvers.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ESFP_Id, "ESFP", "Spontaneous, outgoing, and loves engaging with people.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ENFP_Id, "ENFP", "Enthusiastic, imaginative, and driven by meaningful connections.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ENTP_Id, "ENTP", "Inventive, curious, and thrives on debate and innovation.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ESTJ_Id, "ESTJ", "Organized, practical, and values efficiency and structure.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ESFJ_Id, "ESFJ", "Warm, reliable, and focused on community and harmony.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ENFJ_Id, "ENFJ", "Charismatic, empathetic, and thrives on helping others grow.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { ENTJ_Id, "ENTJ", "Bold, confident leaders who love setting and achieving goals.", MBTI_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // OCEAN: Big Five Traits
                    { Openness_Id, "Openness", "Creative, imaginative, and open to new experiences and ideas.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Conscientiousness_Ocean_Id, "Conscientiousness", "Disciplined, organized, and reliable in achieving goals.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Extraversion_Id, "Extraversion", "Outgoing, energetic, and enjoys being around people.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Agreeableness_Id, "Agreeableness", "Compassionate, cooperative, and values getting along with others.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Neuroticism_Id, "Neuroticism", "Tends to experience emotional instability, anxiety, and mood swings.", OCEAN_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // DISC: 4 Traits
                    { Dominance_Id, "Dominance", "Assertive, results-driven, and enjoys challenges and control.", DISC_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Influence_Id, "Influence", "Sociable, persuasive, and enjoys interacting with others.", DISC_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Steadiness_Id, "Steadiness", "Calm, dependable, and values cooperation and stability.", DISC_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Conscientiousness_Disc_Id, "Conscientiousness", "Detail-oriented, accurate, and values quality and standards.", DISC_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null }
                }
            );
            var now = DateTime.UtcNow;
            var universityIdHust = Guid.NewGuid();
            var universityIdBk = Guid.NewGuid();
            var universityIdvnuhcm = Guid.NewGuid();
            var universityIdneu = Guid.NewGuid();
            var universityIdtdt = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "Universities",
                columns: new[]
                {
                    nameof(University.Id),
                    nameof(University.Name),
                    nameof(University.Location),
                    nameof(University.PhoneNumber),
                    nameof(University.Email),
                    nameof(University.Website),
                    nameof(University.Description),
                    nameof(University.CreatorId),
                    nameof(University.EditorId),
                    nameof(University.CreatedAt),
                    nameof(University.UpdatedAt),
                    nameof(University.DeletedAt)
                },
                values: new object[,]
                {
                    {
                        universityIdHust,
                        "Đại học Quốc gia Hà Nội",
                        "Hà Nội",
                        "02437547617",
                        "contact@vnu.edu.vn",
                        "https://www.vnu.edu.vn",
                        "Trường đại học đa ngành hàng đầu tại Việt Nam.",
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        universityIdBk,
                        "Đại học Bách Khoa Hà Nội",
                        "Hà Nội",
                        "02438692267",
                        "contact@hust.edu.vn",
                        "https://www.hust.edu.vn",
                        "Trường đại học kỹ thuật hàng đầu Việt Nam.",
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        universityIdvnuhcm,
                        "Đại học Quốc gia TP. Hồ Chí Minh",
                        "TP. Hồ Chí Minh",
                        "02837242160",
                        "contact@vnuhcm.edu.vn",
                        "https://www.vnuhcm.edu.vn",
                        "Hệ thống đại học trọng điểm quốc gia phía Nam.",
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        universityIdneu,
                        "Đại học Kinh tế Quốc dân",
                        "Hà Nội",
                        "02436280280",
                        "contact@neu.edu.vn",
                        "https://www.neu.edu.vn",
                        "Trường đại học chuyên về kinh tế, quản trị và tài chính.",
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        universityIdtdt,
                        "Đại học Tôn Đức Thắng",
                        "TP. Hồ Chí Minh",
                        "02837755035",
                        "contact@tdtu.edu.vn",
                        "https://www.tdtu.edu.vn",
                        "Trường đại học công lập thuộc Tổng Liên đoàn Lao động Việt Nam.",
                        null,
                        null,
                        now,
                        now,
                        null
                    }
                }
            );
            var HUST_SE_ID = Guid.NewGuid();
            var VNU_SE_ID = Guid.NewGuid();
            var AI_ID = Guid.NewGuid();
            var EE_ID = Guid.NewGuid();
            var IB_ID = Guid.NewGuid();
            var BM_ID = Guid.NewGuid();
            var LO_ID = Guid.NewGuid();
            var BE_ID = Guid.NewGuid();
            var DC_ID = Guid.NewGuid();
            var LA_ID = Guid.NewGuid();
            var FB_ID = Guid.NewGuid();
            var ME_ID = Guid.NewGuid();
            var AE_ID = Guid.NewGuid();
            var MA_ID = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "Majors",
                columns: new[]
                {
                    nameof(Major.Id),
                    nameof(Major.Name),
                    nameof(Major.Description),
                    nameof(Major.RequiredSkills),
                    nameof(Major.UniversityId),
                    nameof(Major.CreatorId),
                    nameof(Major.EditorId),
                    nameof(Major.CreatedAt),
                    nameof(Major.UpdatedAt),
                    nameof(Major.DeletedAt)
                },
                values: new object[,]
                {
                    {
                        HUST_SE_ID,
                        "Công nghệ thông tin",
                        "Chuyên ngành về phát triển phần mềm, mạng, bảo mật, trí tuệ nhân tạo.",
                        "Lập trình C#, Giải thuật, CSDL, Toán logic",
                        universityIdHust, // VNU Hanoi
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        AI_ID,
                        "Trí tuệ nhân tạo",
                        "Ngành học tập trung vào phát triển hệ thống thông minh, machine learning và deep learning.",
                        "Python, Machine Learning, Toán rời rạc, Giải tích",
                        universityIdHust,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        EE_ID,
                        "Kỹ thuật điện",
                        "Nghiên cứu và ứng dụng điện năng, điện tử và hệ thống điều khiển.",
                        "Vật lý điện, Toán kỹ thuật, AutoCAD",
                       universityIdHust, // HUST
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                       IB_ID,
                        "Kinh tế quốc tế",
                        "Phân tích thị trường, thương mại toàn cầu và chính sách kinh tế.",
                        "Kinh tế vi mô, Vĩ mô, Tiếng Anh, Phân tích số liệu",
                        universityIdneu, // NEU
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        BM_ID,
                        "Quản trị kinh doanh",
                        "Học cách quản lý tài nguyên, con người, tài chính trong doanh nghiệp.",
                        "Lãnh đạo, Giao tiếp, Excel, Phân tích dữ liệu",
                        universityIdtdt, // TDTU
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        VNU_SE_ID,
                        "Kỹ thuật phần mềm",
                        "Phát triển hệ thống phần mềm lớn, đảm bảo chất lượng, hiệu năng và bảo trì dễ dàng.",
                        "C#, UML, Cơ sở dữ liệu, Kiểm thử phần mềm",
                        universityIdvnuhcm,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        ME_ID,
                        "Kỹ thuật cơ khí",
                        "Thiết kế, chế tạo và bảo trì máy móc, thiết bị cơ khí.",
                        "AutoCAD, SolidWorks, Toán kỹ thuật",
                        universityIdHust, // HUST
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                     {
                        FB_ID,
                        "Tài chính ngân hàng",
                        "Quản lý tài chính, đầu tư, ngân hàng và thị trường chứng khoán.",
                        "Phân tích tài chính, Excel, Kế toán, Rủi ro tài chính",
                        universityIdneu,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        MA_ID,
                        "Marketing",
                        "Nghiên cứu thị trường, truyền thông thương hiệu, hành vi khách hàng.",
                        "Giao tiếp, Phân tích dữ liệu, Thiết kế nội dung",
                        universityIdtdt,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        AE_ID,
                        "Kỹ thuật xây dựng",
                        "Thiết kế, thi công và giám sát các công trình xây dựng dân dụng và công nghiệp.",
                        "AutoCAD, SAP2000, Vật liệu xây dựng, Kết cấu bê tông",
                        universityIdBk,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        LA_ID,
                        "Luật",
                        "Đào tạo kiến thức về pháp luật dân sự, hình sự, kinh tế và hành chính.",
                        "Lý luận pháp luật, Phân tích văn bản, Kỹ năng tranh tụng",
                        universityIdtdt,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        Guid.NewGuid(),
                        "Quản trị du lịch và lữ hành",
                        "Tổ chức và quản lý tour du lịch, khách sạn, và dịch vụ du lịch.",
                        "Giao tiếp, Quản trị, Tiếng Anh, Kỹ năng tổ chức sự kiện",
                        universityIdneu,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        BE_ID,
                        "Công nghệ sinh học",
                        "Nghiên cứu và ứng dụng các kỹ thuật sinh học trong y tế, nông nghiệp, và môi trường.",
                        "Sinh học, Hóa học, Lab thực nghiệm, Tin sinh học",
                        universityIdvnuhcm,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        DC_ID,
                        "Khoa học dữ liệu",
                        "Phân tích và xử lý dữ liệu lớn để hỗ trợ quyết định và dự đoán xu hướng.",
                        "Python, SQL, Machine Learning, Kỹ năng trực quan hóa",
                        universityIdHust,
                        null,
                        null,
                        now,
                        now,
                        null
                    },
                    {
                        LO_ID,
                        "Logistics và quản lý chuỗi cung ứng",
                        "Quản lý vận chuyển, kho bãi và tối ưu chuỗi cung ứng.",
                        "Quản trị vận hành, Excel nâng cao, Đàm phán, ERP",
                        universityIdBk,
                        null,
                        null,
                        now,
                        now,
                        null
                    }
                }
            );

            migrationBuilder.InsertData(
                table: "MajorPersonalities",
                columns: new[] { nameof(MajorPersonality.Id), nameof(MajorPersonality.MajorId), nameof(MajorPersonality.PersonalityId), nameof(MajorPersonality.CreatorId), nameof(MajorPersonality.EditorId), nameof(MajorPersonality.CreatedAt), nameof(MajorPersonality.UpdatedAt), nameof(MajorPersonality.DeletedAt) },
                values: new object[,]
                {
                    // HUST Software Engineering (assumed to favor logical, analytical, and detail-oriented types)
                    { Guid.NewGuid(), HUST_SE_ID, INTJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), HUST_SE_ID, ISTJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), HUST_SE_ID, INTP_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // VNU Software Engineering
                    { Guid.NewGuid(), VNU_SE_ID, INTP_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), VNU_SE_ID, ENTP_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // Artificial Intelligence
                    { Guid.NewGuid(), AI_ID, INTP_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), AI_ID, INTJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), AI_ID, Openness_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // Electrical Engineering
                    { Guid.NewGuid(), EE_ID, ISTJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), EE_ID, ESTJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // International Business
                    { Guid.NewGuid(), IB_ID, ENFP_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), IB_ID, Extraversion_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), IB_ID, Influence_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // Business Management
                    { Guid.NewGuid(), BM_ID, ESFJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), BM_ID, ENFJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // Logistics
                    { Guid.NewGuid(), LO_ID, ESTJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), LO_ID, Conscientiousness_Ocean_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },

                    // Business Economics
                    { Guid.NewGuid(), BE_ID, INTJ_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null },
                    { Guid.NewGuid(), BE_ID, ENTP_Id, null, null, DateTime.UtcNow, DateTime.UtcNow, null }
                }
            );

            //sample data for OCEAN tests
            var OCEAN_Test_Id = Guid.NewGuid();
            var OceanQuestionIds = Enumerable.Range(0, 10)
                .Select(i => Guid.NewGuid()).ToList();
            string[] questionTexts =
            {
                "I see myself as someone who has an active imagination.",         
                "I see myself as someone who has few artistic interests.",        
                "I see myself as someone who does a thorough job.",               
                "I see myself as someone who tends to be lazy.",                 
                "I see myself as someone who is outgoing, sociable.",             
                "I see myself as someone who is reserved.",                       
                "I see myself as someone who is generally trusting.",            
                "I see myself as someone who tends to find fault with others.",   
                "I see myself as someone who gets nervous easily.",               
                "I see myself as someone who is relaxed, handles stress well."    
            };
            string[] answerTexts = new[]
{
                "Strongly Disagree",
                "Disagree",
                "Neutral",
                "Agree",
                "Strongly Agree"
            };

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "Id", "Title", "Description", "PersonalityTypeId", "CreatorId", "EditorId", "CreatedAt", "UpdatedAt", "DeletedAt" },
                values: new object[,]
                {
                    {
                        OCEAN_Test_Id, 
                        "OCEAN Personality Test (10 Questions)",
                        "A personality test based on the Big Five personality traits (OCEAN): Openness, Conscientiousness, Extraversion, Agreeableness, and Neuroticism. This version contains 50 questions that help assess the five core dimensions of personality.",
                        OCEAN_Id, 
                        null,
                        null,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        null
                    }
                }
            );

            for (int i = 0; i < 10; i++)
            {
                migrationBuilder.InsertData(
                    table: "Questions",
                    columns: new[]
                    {
                        "Id", "Text", "TestId", "CreatorId", "EditorId", "CreatedAt", "UpdatedAt", "DeletedAt"
                    },
                    values: new object[]
                    {
                    OceanQuestionIds[i],
                    questionTexts[i],
                    OCEAN_Test_Id,
                    null,
                    null,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    null
                    }
                );
            }

            for (int q = 0; q < 10; q++)
            {
                for (int i = 0; i < 5; i++)
                {
                    migrationBuilder.InsertData(
                        table: "Answers",
                        columns: new[]
                        {
                            "Id", "Text", "QuestionId", "CreatorId", "EditorId", "CreatedAt", "UpdatedAt", "DeletedAt"
                        },
                        values: new object[]
                        {
                            Guid.NewGuid(),
                            answerTexts[i],
                            OceanQuestionIds[q],
                            null,
                            null,
                            DateTime.UtcNow,
                            DateTime.UtcNow,
                            null
                        }
                    );
                }
            }

            var MBTI_Test_Id = Guid.NewGuid();
            var MBTI_QuestionIds = Enumerable.Range(0, 50)
                 .Select(i => Guid.NewGuid()).ToList();
            string[] mbtiQuestions = new[]
            {
                // E vs I
                "You enjoy being at the center of attention.",
                "You prefer to spend time alone rather than in a crowd.",
                "You find it easy to introduce yourself to other people.",
                "You feel drained after socializing, even if you enjoyed yourself.",
                "You are more outgoing than reserved.",
                "You often start conversations.",
                "You find it difficult to approach others.",
                "You enjoy group activities more than solitary ones.",
                "You feel energized after being with people.",
                "You prefer quiet time to recharge.",

                // S vs N
                "You focus more on details than the big picture.",
                "You trust facts more than theories.",
                "You are practical and grounded.",
                "You rely more on your experience than your imagination.",
                "You value realism over creativity.",
                "You are drawn to abstract ideas and possibilities.",
                "You often notice patterns that others miss.",
                "You enjoy solving theoretical problems.",
                "You are imaginative and open to new ideas.",
                "You prefer facts over abstract concepts.",

                // T vs F
                "You make decisions based on logic, not emotions.",
                "You find it difficult to empathize with others.",
                "You value justice over mercy.",
                "You prefer objective criticism over compliments.",
                "You prioritize fairness over feelings.",
                "You care deeply about others’ feelings.",
                "You avoid hurting people’s feelings at all costs.",
                "You are more compassionate than analytical.",
                "You believe kindness is more important than truth.",
                "You often let your heart rule your head.",

                // J vs P
                "You prefer a clear plan over spontaneity.",
                "You like to have things settled and decided.",
                "You are uncomfortable with last-minute changes.",
                "You make to-do lists and stick to them.",
                "You feel anxious when things are unorganized.",
                "You enjoy being flexible with your schedule.",
                "You act on impulse rather than planning ahead.",
                "You often leave your options open.",
                "You dislike routine and prefer variety.",
                "You adapt easily to unexpected changes.",

                // Mixed final 10 for balance
                "You feel comfortable making quick decisions.", // J
                "You often reflect on past conversations.", // I
                "You look for deeper meaning in everything.", // N
                "You enjoy debating ideas even if controversial.", // T
                "You tend to follow your emotions.", // F
                "You would rather follow your heart than your head.", // F
                "You often multitask to keep things interesting.", // P
                "You find comfort in structured environments.", // J
                "You avoid being in the spotlight.", // I
                "You trust your instincts more than facts." // N
            };

            string[] answerOptions = { "Agree", "Disagree" };

            migrationBuilder.InsertData(
            table: "Tests",
            columns: new[] { "Id", "Title", "Description", "PersonalityTypeId", "CreatorId", "EditorId", "CreatedAt", "UpdatedAt", "DeletedAt" },
            values: new object[,]
                {
                    {
                        MBTI_Test_Id,
                        "MBTI Personality Test (50 Questions)",
                        "A personality test based on the Myers-Briggs Type Indicator, consisting of 50 questions covering the 4 dichotomies: E/I, S/N, T/F, J/P.",
                        MBTI_Id,
                        null,
                        null,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        null
                    }
                }
            );
            for (int i = 0; i < mbtiQuestions.Length; i++)
            {
                migrationBuilder.InsertData(
                    table: "Questions",
                    columns: new[] { "Id", "Text", "TestId", "CreatorId", "EditorId", "CreatedAt", "UpdatedAt", "DeletedAt" },
                    values: new object[]
                    {
                        MBTI_QuestionIds[i],
                        mbtiQuestions[i],
                        MBTI_Test_Id,
                        null,
                        null,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        null
                    }
                );
            }

            for (int i = 0; i < mbtiQuestions.Length; i++)
            {
                foreach (var answer in answerOptions)
                {
                    migrationBuilder.InsertData(
                        table: "Answers",
                        columns: new[] { "Id", "Text", "QuestionId", "CreatorId", "EditorId", "CreatedAt", "UpdatedAt", "DeletedAt" },
                        values: new object[]
                        {
                            Guid.NewGuid(),
                            answer,
                            MBTI_QuestionIds[i],
                            null,
                            null,
                            DateTime.UtcNow,
                            DateTime.UtcNow,
                            null
                        }
                    );
                }
            }

        }
    }
}
