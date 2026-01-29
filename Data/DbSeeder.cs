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
            // === MATH QUESTIONS ===
            
            // Grade 3 - Math - Nafes
            new Question { Text = "ما هو ناتج 5 + 3؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"6\", \"7\", \"8\", \"9\"]", CorrectAnswer = "8", Grade = GradeLevel.Grade3, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "أكمل النمط: 2, 4, 6, ...", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"7\", \"8\", \"9\", \"10\"]", CorrectAnswer = "8", Grade = GradeLevel.Grade3, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            
            // Grade 3 - Math - Central
            new Question { Text = "ما هو ناتج 10 - 4؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"5\", \"6\", \"7\", \"8\"]", CorrectAnswer = "6", Grade = GradeLevel.Grade3, Subject = SubjectType.Math, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            
            // Grade 4 - Math - Central
            new Question { Text = "ما هو ناتج 12 × 4؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"44\", \"46\", \"48\", \"50\"]", CorrectAnswer = "48", Grade = GradeLevel.Grade4, Subject = SubjectType.Math, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "قيمة الرقم 5 في العدد 543 هي:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"5\", \"50\", \"500\", \"5000\"]", CorrectAnswer = "500", Grade = GradeLevel.Grade4, Subject = SubjectType.Math, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },

            // Grade 5 - Math - Central
            new Question { Text = "ما هو ناتج 144 ÷ 12؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"10\", \"11\", \"12\", \"13\"]", CorrectAnswer = "12", Grade = GradeLevel.Grade5, Subject = SubjectType.Math, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            
            // Grade 6 - Math - Nafes
            new Question { Text = "إذا كان س = 5، فما قيمة 2س + 3؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Hard, Options = "[\"10\", \"11\", \"12\", \"13\"]", CorrectAnswer = "13", Grade = GradeLevel.Grade6, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "مساحة مربع طول ضلعه 4 سم تساوي:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"12 سم²\", \"16 سم²\", \"20 سم²\", \"8 سم²\"]", CorrectAnswer = "16 سم²", Grade = GradeLevel.Grade6, Subject = SubjectType.Math, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // === SCIENCE QUESTIONS ===

            // Grade 3 - Science - Nafes (Usually Grade 3 has basic science)
            new Question { Text = "الحيوان الذي يغطي جسمه الريش هو:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"السمكة\", \"العصفور\", \"القطة\", \"الثعبان\"]", CorrectAnswer = "العصفور", Grade = GradeLevel.Grade3, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // Grade 4 - Science - Central
            new Question { Text = "كم عدد كواكب المجموعة الشمسية؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"7\", \"8\", \"9\", \"10\"]", CorrectAnswer = "8", Grade = GradeLevel.Grade4, Subject = SubjectType.Science, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            new Question { Text = "الجزء المسؤول عن صنع الغذاء في النبات هو:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"الجذر\", \"الساق\", \"الورقة\", \"الزهرة\"]", CorrectAnswer = "الورقة", Grade = GradeLevel.Grade4, Subject = SubjectType.Science, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },

            // Grade 5 - Science - Central
            new Question { Text = "حالة المادة التي لها شكل ثابت وحجم ثابت هي:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"الصلبة\", \"السائلة\", \"الغازية\", \"البلازما\"]", CorrectAnswer = "الصلبة", Grade = GradeLevel.Grade5, Subject = SubjectType.Science, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },

            // Grade 6 - Science - Nafes
            new Question { Text = "وحدة قياس القوة هي:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"المتر\", \"الكيلوجرام\", \"النيوتن\", \"الثانية\"]", CorrectAnswer = "النيوتن", Grade = GradeLevel.Grade6, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "أي مما يلي يعتبر تغيراً كيميائياً؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Hard, Options = "[\"انصهار الثلج\", \"هطول المطر\", \"صدا الحديد\", \"تمزق الورق\"]", CorrectAnswer = "صدا الحديد", Grade = GradeLevel.Grade6, Subject = SubjectType.Science, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },

            // === ARABIC QUESTIONS ===

            // Grade 3 - Arabic - Nafes
            new Question { Text = "كلمة (المدرسة) بدأت بلام:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"شمسية\", \"قمرية\", \"أصلية\", \"زائدة\"]", CorrectAnswer = "قمرية", Grade = GradeLevel.Grade3, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            
            // Grade 4 - Arabic - Central
            new Question { Text = "ما هو جمع كلمة 'كتاب'؟", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Easy, Options = "[\"كتب\", \"كتابات\", \"كتبة\", \"كاتب\"]", CorrectAnswer = "كتب", Grade = GradeLevel.Grade4, Subject = SubjectType.Arabic, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },
            
            // Grade 5 - Arabic - Central
            new Question { Text = "الفاعل في جملة (قرأ الطالب الدرس) هو:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"قرأ\", \"الدرس\", \"الطالب\", \"ضمير مستتر\"]", CorrectAnswer = "الطالب", Grade = GradeLevel.Grade5, Subject = SubjectType.Arabic, TestType = TestType.Central, CreatedDate = DateTime.UtcNow },

            // Grade 6 - Arabic - Nafes
            new Question { Text = "علامة رفع المثنى هي:", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Hard, Options = "[\"الضمة\", \"الألف\", \"الواو\", \"الفتحة\"]", CorrectAnswer = "الألف", Grade = GradeLevel.Grade6, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow },
            new Question { Text = "ضد كلمة (الأمانة):", Type = QuestionType.MultipleChoice, Difficulty = DifficultyLevel.Medium, Options = "[\"الصدق\", \"الخيانة\", \"الوفاء\", \"الإخلاص\"]", CorrectAnswer = "الخيانة", Grade = GradeLevel.Grade6, Subject = SubjectType.Arabic, TestType = TestType.Nafes, CreatedDate = DateTime.UtcNow }
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
    }
}
