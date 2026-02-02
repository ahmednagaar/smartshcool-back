using Nafes.API.Data;
using Nafes.API.Modules;
using Microsoft.EntityFrameworkCore;

namespace Nafes.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Admin
        var admin = await context.Admins.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Username == "admin");
        if (admin == null)
        {
            admin = new Admin
            {
                Username = "admin",
                Email = "admin@nafes.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12),
                Role = AdminRole.SuperAdmin,
                IsApproved = true,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            context.Admins.Add(admin);
        }
        else
        {
            // Reset existing admin
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12);
            admin.IsApproved = true;
            admin.IsLocked = false;
            admin.LockedUntil = null;
            admin.FailedLoginAttempts = 0;
            admin.IsDeleted = false; // Restore if deleted
            
            context.Admins.Update(admin);
        }
        
        await context.SaveChangesAsync();

        // Seed Students
        if (!context.Students.Any())
        {
            var students = new List<Student>
            {
                new Student { Name = "أحمد محمد", Age = 10, Grade = "الصف الرابع", CreatedDate = DateTime.UtcNow },
                new Student { Name = "فاطمة علي", Age = 11, Grade = "الصف الخامس", CreatedDate = DateTime.UtcNow },
                new Student { Name = "محمد حسن", Age = 12, Grade = "الصف السادس", CreatedDate = DateTime.UtcNow },
                new Student { Name = "سارة خالد", Age = 10, Grade = "الصف الرابع", CreatedDate = DateTime.UtcNow },
                new Student { Name = "عمر يوسف", Age = 11, Grade = "الصف الخامس", CreatedDate = DateTime.UtcNow }
            };
            context.Students.AddRange(students);
            await context.SaveChangesAsync();
        }

        // Seed Questions
        if (!context.Questions.Any())
        {

        // Seed Questions
        var questions = new List<Question>
        {
            // Grade 3 - Arabic
            new Question { Text = "ما جمع كلمة \"كتاب\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"كتاب\", \"كتب\", \"كاتب\"]", CorrectAnswer = "كتب", Grade = GradeLevel.Grade3, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما عكس كلمة \"كبير\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"طويل\", \"صغير\", \"سريع\"]", CorrectAnswer = "صغير", Grade = GradeLevel.Grade3, Subject = SubjectType.Arabic, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "أيهما اسم؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"يكتب\", \"مدرسة\", \"يذهب\"]", CorrectAnswer = "مدرسة", Grade = GradeLevel.Grade3, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 3 - Science
            new Question { Text = "ما الكوكب الذي نعيش عليه؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"القمر\", \"الأرض\", \"الشمس\"]", CorrectAnswer = "الأرض", Grade = GradeLevel.Grade3, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "أي من الآتي حيوان؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"شجرة\", \"حجر\", \"قطة\"]", CorrectAnswer = "قطة", Grade = GradeLevel.Grade3, Subject = SubjectType.Science, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما لون الشمس؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"أزرق\", \"أصفر\", \"أخضر\"]", CorrectAnswer = "أصفر", Grade = GradeLevel.Grade3, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 3 - Math
            new Question { Text = "5 + 3 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"6\", \"7\", \"8\"]", CorrectAnswer = "8", Grade = GradeLevel.Grade3, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "10 − 4 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"5\", \"6\", \"7\"]", CorrectAnswer = "6", Grade = GradeLevel.Grade3, Subject = SubjectType.Math, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "أيهما أكبر؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"6\", \"8\", \"9\"]", CorrectAnswer = "9", Grade = GradeLevel.Grade3, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 4 - Arabic
            new Question { Text = "ما جمع كلمة \"ولد\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"ولود\", \"أولاد\", \"ولدين\"]", CorrectAnswer = "أولاد", Grade = GradeLevel.Grade4, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما نوع كلمة \"يلعب\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"اسم\", \"فعل\", \"حرف\"]", CorrectAnswer = "فعل", Grade = GradeLevel.Grade4, Subject = SubjectType.Arabic, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما مرادف كلمة \"سعيد\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"حزين\", \"فرحان\", \"غاضب\"]", CorrectAnswer = "فرحان", Grade = GradeLevel.Grade4, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 4 - Science
            new Question { Text = "ما الحالة السائلة للماء؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"بخار\", \"ثلج\", \"ماء\"]", CorrectAnswer = "ماء", Grade = GradeLevel.Grade4, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "أي عضو نستخدمه للتنفس؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"القلب\", \"الرئة\", \"المعدة\"]", CorrectAnswer = "الرئة", Grade = GradeLevel.Grade4, Subject = SubjectType.Science, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما مصدر الضوء الطبيعي؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"المصباح\", \"الشمس\", \"القمر\"]", CorrectAnswer = "الشمس", Grade = GradeLevel.Grade4, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 4 - Math
            new Question { Text = "6 × 2 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"8\", \"10\", \"12\"]", CorrectAnswer = "12", Grade = GradeLevel.Grade4, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "15 ÷ 3 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"4\", \"5\", \"6\"]", CorrectAnswer = "5", Grade = GradeLevel.Grade4, Subject = SubjectType.Math, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "20 + 15 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"30\", \"35\", \"40\"]", CorrectAnswer = "35", Grade = GradeLevel.Grade4, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 5 - Arabic
            new Question { Text = "ما مفرد كلمة \"أقلام\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"قلم\", \"قلام\", \"قلمون\"]", CorrectAnswer = "قلم", Grade = GradeLevel.Grade5, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما ضد كلمة \"نشاط\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"تعب\", \"كسل\", \"سرعة\"]", CorrectAnswer = "كسل", Grade = GradeLevel.Grade5, Subject = SubjectType.Arabic, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "الجملة \"الطالب مجتهد\" هي؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"فعلية\", \"اسمية\", \"استفهامية\"]", CorrectAnswer = "اسمية", Grade = GradeLevel.Grade5, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 5 - Science
            new Question { Text = "ما الغاز اللازم للتنفس؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"النيتروجين\", \"الأكسجين\", \"الهيدروجين\"]", CorrectAnswer = "الأكسجين", Grade = GradeLevel.Grade5, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما الكوكب الأحمر؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"الزهرة\", \"المريخ\", \"عطارد\"]", CorrectAnswer = "المريخ", Grade = GradeLevel.Grade5, Subject = SubjectType.Science, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "أي من الآتي نبات؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"قط\", \"حجر\", \"شجرة\"]", CorrectAnswer = "شجرة", Grade = GradeLevel.Grade5, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 5 - Math
            new Question { Text = "9 × 4 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"32\", \"36\", \"40\"]", CorrectAnswer = "36", Grade = GradeLevel.Grade5, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "50 − 18 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"30\", \"32\", \"34\"]", CorrectAnswer = "32", Grade = GradeLevel.Grade5, Subject = SubjectType.Math, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "نصف العدد 20 هو؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"5\", \"10\", \"15\"]", CorrectAnswer = "10", Grade = GradeLevel.Grade5, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 6 - Arabic
            new Question { Text = "ما نوع كلمة \"الصدق\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Hard, Options = "[\"اسم\", \"فعل\", \"مصدر\"]", CorrectAnswer = "مصدر", Grade = GradeLevel.Grade6, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما جمع كلمة \"مدينة\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"مدائن\", \"مدن\", \"مدينة\"]", CorrectAnswer = "مدن", Grade = GradeLevel.Grade6, Subject = SubjectType.Arabic, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "مرادف كلمة \"شجاع\"؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"خائف\", \"جريء\", \"ضعيف\"]", CorrectAnswer = "جريء", Grade = GradeLevel.Grade6, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 6 - Science
            new Question { Text = "ما العضو المسؤول عن ضخ الدم؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"الرئة\", \"القلب\", \"المخ\"]", CorrectAnswer = "القلب", Grade = GradeLevel.Grade6, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما الوحدة الأساسية لقياس الطول؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"الكيلو\", \"المتر\", \"الجرام\"]", CorrectAnswer = "المتر", Grade = GradeLevel.Grade6, Subject = SubjectType.Science, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ما الكوكب الأكبر في المجموعة الشمسية؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"الأرض\", \"زحل\", \"المشتري\"]", CorrectAnswer = "المشتري", Grade = GradeLevel.Grade6, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 6 - Math
            new Question { Text = "12 × 5 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"50\", \"60\", \"70\"]", CorrectAnswer = "60", Grade = GradeLevel.Grade6, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "100 ÷ 4 = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"20\", \"25\", \"30\"]", CorrectAnswer = "25", Grade = GradeLevel.Grade6, Subject = SubjectType.Math, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "3² = ؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Hard, Options = "[\"6\", \"9\", \"12\"]", CorrectAnswer = "9", Grade = GradeLevel.Grade6, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow }
        };
        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();
        }

        // Seed Games
        if (!context.Games.Any())
        {
            var mathGame = new Game
            {
                Title = "اختبار الرياضيات - المستوى الأول",
                Description = "اختبار أساسي في الرياضيات للصف الرابع والخامس",
                TimeLimit = 15,
                PassingScore = 60,
                CreatedDate = DateTime.UtcNow
            };
            context.Games.Add(mathGame);

            var scienceGame = new Game
            {
                Title = "اختبار العلوم - الفضاء",
                Description = "اختبار عن المجموعة الشمسية والكواكب",
                TimeLimit = 10,
                PassingScore = 70,
                CreatedDate = DateTime.UtcNow
            };
            context.Games.Add(scienceGame);

            var mixedGame = new Game
            {
                Title = "اختبار شامل - نافس",
                Description = "اختبار شامل يغطي الرياضيات والعلوم واللغة العربية",
                TimeLimit = 20,
                PassingScore = 65,
                CreatedDate = DateTime.UtcNow
            };
            context.Games.Add(mixedGame);

            await context.SaveChangesAsync();
        }



        // Seed Achievements
        var achievements = new List<Achievement>
        {
            new Achievement { Title = "بداية الرحلة", Description = "أكمل أول اختبار لك بنجاح", Icon = "🚀", Points = 10, CriteriaType = "TestCount", CriteriaValue = 1, CreatedDate = DateTime.UtcNow },
            new Achievement { Title = "علامة كاملة", Description = "احصل على 100% في أي اختبار", Icon = "⭐", Points = 20, CriteriaType = "Score", CriteriaValue = 100, CreatedDate = DateTime.UtcNow },
            new Achievement { Title = "عبقري الرياضيات", Description = "أكمل 3 اختبارات رياضيات", Icon = "📐", Points = 30, CriteriaType = "SubjectCount", CriteriaValue = 3, CriteriaSubject = "الرياضيات", CreatedDate = DateTime.UtcNow },
            new Achievement { Title = "المثابر", Description = "أكمل 5 اختبارات", Icon = "🔥", Points = 25, CriteriaType = "TestCount", CriteriaValue = 5, CreatedDate = DateTime.UtcNow },
            new Achievement { Title = "السرعة الفائقة", Description = "حل اختبار في أقل من دقيقة", Icon = "⚡", Points = 15, CriteriaType = "Time", CriteriaValue = 60, CreatedDate = DateTime.UtcNow },
            
            // Wheel Game Achievements
            new Achievement { Title = "دوار المعرفة", Description = "أكمل 5 جولات في عجلة الأسئلة", Icon = "🎡", Points = 20, CriteriaType = "WheelGames", CriteriaValue = 5, CreatedDate = DateTime.UtcNow },
            new Achievement { Title = "المحترف", Description = "احصل على 50 نقطة في جولة واحدة", Icon = "🎓", Points = 30, CriteriaType = "WheelScore", CriteriaValue = 50, CreatedDate = DateTime.UtcNow },
            new Achievement { Title = "البرق", Description = "أجب على 20 سؤال في جولة واحدة", Icon = "⚡", Points = 25, CriteriaType = "WheelQuestions", CriteriaValue = 20, CreatedDate = DateTime.UtcNow },
            new Achievement { Title = "الدقة المتناهية", Description = "أجب 10 أسئلة صحيحة متتالية", Icon = "🎯", Points = 40, CriteriaType = "WheelAccuracy", CriteriaValue = 10, CreatedDate = DateTime.UtcNow }
        };

        foreach (var achievement in achievements)
        {
            if (!context.Achievements.Any(a => a.Title == achievement.Title))
            {
                context.Achievements.Add(achievement);
            }
        }
        await context.SaveChangesAsync();
        if (!await context.SystemSettings.AnyAsync())
        {
            context.SystemSettings.AddRange(
                new SystemSetting { Key = "MaintenanceMode", Value = "false", Description = "تفعيل وضع الصيانة", Group = "General", Type = "boolean" },
                new SystemSetting { Key = "AllowRegistration", Value = "true", Description = "السماح بتسجيل الطلاب", Group = "General", Type = "boolean" },
                new SystemSetting { Key = "DefaultGrade", Value = "الصف الرابع", Description = "الصف الافتراضي للطلاب الجدد", Group = "Student", Type = "string" }
            );
            await context.SaveChangesAsync();
        }
        
        // Seed Wheel Spin Segments
        if (!context.WheelSpinSegments.Any())
        {
            var segments = new List<WheelSpinSegment>
            {
                new WheelSpinSegment { DisplayText = "10 نقاط", SegmentValue = 10, SegmentType = SegmentType.Points, ColorCode = "#FFC107", Probability = 0.3m },
                new WheelSpinSegment { DisplayText = "20 نقطة", SegmentValue = 20, SegmentType = SegmentType.Points, ColorCode = "#4CAF50", Probability = 0.25m },
                new WheelSpinSegment { DisplayText = "50 نقطة", SegmentValue = 50, SegmentType = SegmentType.Points, ColorCode = "#2196F3", Probability = 0.15m },
                new WheelSpinSegment { DisplayText = "100 نقطة", SegmentValue = 100, SegmentType = SegmentType.Points, ColorCode = "#9C27B0", Probability = 0.05m },
                new WheelSpinSegment { DisplayText = "مكافأة", SegmentValue = 20, SegmentType = SegmentType.Bonus, ColorCode = "#00BCD4", Probability = 0.1m },
                new WheelSpinSegment { DisplayText = "نقاط مضاعفة", SegmentType = SegmentType.DoublePoints, ColorCode = "#FF5722", Probability = 0.05m },
                new WheelSpinSegment { DisplayText = "خسارة الدور", SegmentType = SegmentType.LoseTurn, ColorCode = "#F44336", Probability = 0.1m }
            };
            context.WheelSpinSegments.AddRange(segments);
            await context.SaveChangesAsync();
        }

        // DISABLED: Force re-seed was deleting ALL user data on every restart!
        // Use /api/WheelGame/seed endpoint for manual seeding instead.
        /*
        // Seed Wheel Questions (Force re-seed to ensure TestType is set correctly)
        // First delete dependent records (WheelQuestionAttempts, WheelGameSessions)
        var existingAttempts = context.WheelQuestionAttempts.ToList();
        if (existingAttempts.Any())
        {
            context.WheelQuestionAttempts.RemoveRange(existingAttempts);
            await context.SaveChangesAsync();
        }
        
        var existingSessions = context.WheelGameSessions.ToList();
        if (existingSessions.Any())
        {
            context.WheelGameSessions.RemoveRange(existingSessions);
            await context.SaveChangesAsync();
        }
        
        // Now delete wheel questions
        var existingQuestions = context.WheelQuestions.ToList();
        if (existingQuestions.Any())
        {
            context.WheelQuestions.RemoveRange(existingQuestions);
            await context.SaveChangesAsync();
        }
        
        // Re-seed with correct TestType values
        {
            var questions = new List<WheelQuestion>
            {
                // Grade 4 - Arabic
                new WheelQuestion { QuestionText = "ما عاصمة السعودية؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "الرياض", WrongAnswers = "[\"جدة\", \"مكة\", \"الدمام\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Arabic, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy, CategoryTag = "جغرافيا", Explanation = "الرياض هي العاصمة." },
                new WheelQuestion { QuestionText = "ضد كلمة شجاع؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "جبان", WrongAnswers = "[\"قوي\", \"سريع\", \"ذكي\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Arabic, TestType = TestType.Central, DifficultyLevel = DifficultyLevel.Easy },
                
                // Grade 4 - Science
                new WheelQuestion { QuestionText = "حيوان يسمى سفينة الصحراء؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "الجمل", WrongAnswers = "[\"الحصان\", \"الفيل\", \"الأسد\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Science, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy },
                
                // Grade 4 - Math
                new WheelQuestion { QuestionText = "5 * 5 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "25", WrongAnswers = "[\"20\", \"30\", \"10\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Math, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy },
                new WheelQuestion { QuestionText = "20 / 4 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "5", WrongAnswers = "[\"4\", \"6\", \"8\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Math, TestType = TestType.Central, DifficultyLevel = DifficultyLevel.Easy },

                // Grade 5 - Arabic
                new WheelQuestion { QuestionText = "الفاعل يكون دائماً؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "مرفوعاً", WrongAnswers = "[\"منصوباً\", \"مجروراً\", \"ساكناً\"]", PointsValue = 15, GradeId = GradeLevel.Grade5, SubjectId = SubjectType.Arabic, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Medium },

                // Grade 5 - Math
                new WheelQuestion { QuestionText = "محيط مربع ضلعه 5سم؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "20", WrongAnswers = "[\"25\", \"15\", \"10\"]", PointsValue = 15, GradeId = GradeLevel.Grade5, SubjectId = SubjectType.Math, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Medium },
                new WheelQuestion { QuestionText = "100 - 45 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "55", WrongAnswers = "[\"45\", \"65\", \"50\"]", PointsValue = 10, GradeId = GradeLevel.Grade5, SubjectId = SubjectType.Math, TestType = TestType.Central, DifficultyLevel = DifficultyLevel.Easy },

                // Grade 5 - Science
                new WheelQuestion { QuestionText = "عدد كواكب المجموعة الشمسية؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "8", WrongAnswers = "[\"7\", \"9\", \"10\"]", PointsValue = 15, GradeId = GradeLevel.Grade5, SubjectId = SubjectType.Science, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Medium },

                // Grade 6 - Arabic
                new WheelQuestion { QuestionText = "إعراب المبتدأ؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "مرفوع", WrongAnswers = "[\"منصوب\", \"مجرور\"]", PointsValue = 15, GradeId = GradeLevel.Grade6, SubjectId = SubjectType.Arabic, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Medium },

                // Grade 6 - Math
                new WheelQuestion { QuestionText = "3 أس 2 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "9", WrongAnswers = "[\"6\", \"3\", \"12\"]", PointsValue = 20, GradeId = GradeLevel.Grade6, SubjectId = SubjectType.Math, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Hard },
                new WheelQuestion { QuestionText = "جذر 16 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "4", WrongAnswers = "[\"2\", \"8\", \"16\"]", PointsValue = 15, GradeId = GradeLevel.Grade6, SubjectId = SubjectType.Math, TestType = TestType.Central, DifficultyLevel = DifficultyLevel.Medium },

                // Grade 6 - Science
                new WheelQuestion { QuestionText = "الغاز الذي نتنفسه؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "الأكسجين", WrongAnswers = "[\"الهيدروجين\", \"النيتروجين\"]", PointsValue = 15, GradeId = GradeLevel.Grade6, SubjectId = SubjectType.Science, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy },
                
                // Grade 3 - Math
                new WheelQuestion { QuestionText = "5 + 3 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "8", WrongAnswers = "[\"7\", \"9\", \"6\"]", PointsValue = 10, GradeId = GradeLevel.Grade3, SubjectId = SubjectType.Math, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy },
                new WheelQuestion { QuestionText = "10 - 4 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "6", WrongAnswers = "[\"5\", \"7\", \"4\"]", PointsValue = 10, GradeId = GradeLevel.Grade3, SubjectId = SubjectType.Math, TestType = TestType.Central, DifficultyLevel = DifficultyLevel.Easy },
                
                // Grade 3 - Arabic
                new WheelQuestion { QuestionText = "جمع كلمة كتاب؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "كتب", WrongAnswers = "[\"كاتب\", \"كتيب\"]", PointsValue = 10, GradeId = GradeLevel.Grade3, SubjectId = SubjectType.Arabic, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy },
                
                // Grade 3 - Science
                new WheelQuestion { QuestionText = "ما لون الشمس؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "أصفر", WrongAnswers = "[\"أحمر\", \"أخضر\"]", PointsValue = 10, GradeId = GradeLevel.Grade3, SubjectId = SubjectType.Science, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy }
            };
            context.WheelQuestions.AddRange(questions);
            await context.SaveChangesAsync();
        }
        */ // END OF DISABLED WHEEL QUESTION SEEDER
        
        // Seed Drag & Drop Games
        if (!context.DragDropQuestions.Any())
        {
            var dragDropQuestions = new List<DragDropQuestion>
            {
                // Grade 3 - Science - Classification
                new DragDropQuestion 
                { 
                    Grade = GradeLevel.Grade3,
                    Subject = SubjectType.Science,
                    GameTitle = "تصنيف الكائنات الحية",
                    Instructions = "اسحب الكائنات الحية إلى المجموعة الصحيحة: نباتات أو حيوانات.",
                    NumberOfZones = 2,
                    UITheme = "nature",
                    TimeLimit = 60,
                    PointsPerCorrectItem = 10,
                    ShowImmediateFeedback = true,
                    DisplayOrder = 1,
                    IsActive = true,
                    CreatedBy = 1,
                    Zones = new List<DragDropZone>
                    {
                        new DragDropZone { Label = "نباتات", ColorCode = "#4CAF50", ZoneOrder = 1, IconUrl = "assets/icons/plant.png" },
                        new DragDropZone { Label = "حيوانات", ColorCode = "#FF9800", ZoneOrder = 2, IconUrl = "assets/icons/animal.png" }
                    },
                    Items = new List<DragDropItem>
                    {
                        new DragDropItem { Text = "شجرة التفاح", CorrectZoneId = 0, ItemOrder = 1 }, // ZoneId will be fixed by EF navigation logic or we need careful seeding
                        new DragDropItem { Text = "أسد", CorrectZoneId = 0, ItemOrder = 2 },
                        new DragDropItem { Text = "وردة", CorrectZoneId = 0, ItemOrder = 3 },
                        new DragDropItem { Text = "قطة", CorrectZoneId = 0, ItemOrder = 4 }
                    }
                },
                
                // Grade 4 - Arabic - Parts of Speech
                new DragDropQuestion 
                { 
                    Grade = GradeLevel.Grade4,
                    Subject = SubjectType.Arabic,
                    GameTitle = "أقسام الكلام",
                    Instructions = "صنف الكلمات التالية إلى: اسم، فعل، أو حرف.",
                    NumberOfZones = 3,
                    UITheme = "modern",
                    TimeLimit = 90,
                    PointsPerCorrectItem = 15,
                    ShowImmediateFeedback = true,
                    DisplayOrder = 1,
                    IsActive = true,
                    CreatedBy = 1,
                    Zones = new List<DragDropZone>
                    {
                        new DragDropZone { Label = "اسم", ColorCode = "#2196F3", ZoneOrder = 1 },
                        new DragDropZone { Label = "فعل", ColorCode = "#F44336", ZoneOrder = 2 },
                        new DragDropZone { Label = "حرف", ColorCode = "#9C27B0", ZoneOrder = 3 }
                    },
                    Items = new List<DragDropItem>
                    {
                        new DragDropItem { Text = "محمد", CorrectZoneId = 0, ItemOrder = 1 },
                        new DragDropItem { Text = "يكتب", CorrectZoneId = 0, ItemOrder = 2 },
                        new DragDropItem { Text = "في", CorrectZoneId = 0, ItemOrder = 3 },
                        new DragDropItem { Text = "مدرسة", CorrectZoneId = 0, ItemOrder = 4 },
                        new DragDropItem { Text = "ذهب", CorrectZoneId = 0, ItemOrder = 5 }
                    }
                }
            };

            // Fix relationships manually since EF Core seeding with navigation properties works recursively but needs care for FKs
            // Actually, adding graphs works if we rely on navigation.
            // But Item.CorrectZoneId requires the Zone.Id which is generated.
            // EF Core Fix-up usually handles this if we use navigation properties.
            // Let's modify the object initialization to use navigation `CorrectZone` instead of ID.
            
            foreach (var q in dragDropQuestions)
            {
               // Manually link items to zones by index/logic since we can't guess ID
               // Item 0 -> Zone 0, Item 1 -> Zone 1, etc.
               
               if (q.GameTitle == "تصنيف الكائنات الحية")
               {
                   // Plants: 0 and 2
                   q.Items[0].CorrectZone = q.Zones[0];
                   q.Items[2].CorrectZone = q.Zones[0];
                   
                   // Animals: 1 and 3
                   q.Items[1].CorrectZone = q.Zones[1];
                   q.Items[3].CorrectZone = q.Zones[1];
               }
               else if (q.GameTitle == "أقسام الكلام")
               {
                   // Noun (Zone 0): 0, 3
                   q.Items[0].CorrectZone = q.Zones[0];
                   q.Items[3].CorrectZone = q.Zones[0];
                   
                   // Verb (Zone 1): 1, 4
                   q.Items[1].CorrectZone = q.Zones[1];
                   q.Items[4].CorrectZone = q.Zones[1];
                   
                   // Particle (Zone 2): 2
                   q.Items[2].CorrectZone = q.Zones[2];
               }
               
               context.DragDropQuestions.Add(q);
            }
            
            await context.SaveChangesAsync();
        }
    }
}
