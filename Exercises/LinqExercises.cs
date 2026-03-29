using LinqConsoleLab.EN.Data;

namespace LinqConsoleLab.EN.Exercises;

public sealed class LinqExercises
{
    
    public IEnumerable<string> Task01_StudentsFromWarsaw()
    {
        return UniversityData.Students
            .Where(s => s.City == "Warsaw")
            .Select(s => $"{s.IndexNumber} | {s.FirstName} {s.LastName} | {s.City}");
    }
    
    
    public IEnumerable<string> Task02_StudentEmailAddresses()
    {
        return UniversityData.Students
            .Select(s => s.Email);
    }

   
    public IEnumerable<string> Task03_StudentsSortedAlphabetically()
    {
        return UniversityData.Students
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Select(s => $"{s.IndexNumber} | {s.FirstName} {s.LastName}");
    }

    
    public IEnumerable<string> Task04_FirstAnalyticsCourse()
    {
        var course = UniversityData.Courses
            .FirstOrDefault(c => c.Category == "Analytics");

        if (course is null)
        {
            return ["No Analytics course found."];
        }

        return [$"{course.Title} | start: {course.StartDate:yyyy-MM-dd}"];
    }
    

    
    public IEnumerable<string> Task05_IsThereAnyInactiveEnrollment()
    {
        var hasInactive = UniversityData.Enrollments.Any(e => !e.IsActive);
        return [$"Inactive enrollment exists: {hasInactive}"];
    }

    
    public IEnumerable<string> Task06_DoAllLecturersHaveDepartment()
    {
        var allHaveDepartment = UniversityData.Lecturers
            .All(l => !string.IsNullOrWhiteSpace(l.Department));

        return [$"All lecturers have a department: {allHaveDepartment}"];
    }

    
    public IEnumerable<string> Task07_CountActiveEnrollments()
    {
        var activeCount = UniversityData.Enrollments.Count(e => e.IsActive);
        return [$"Active enrollments: {activeCount}"];
    }

    
    public IEnumerable<string> Task08_DistinctStudentCities()
    {
        return UniversityData.Students
            .Select(s => s.City)
            .Distinct()
            .OrderBy(city => city);
    }

    
    public IEnumerable<string> Task09_ThreeNewestEnrollments()
    {
        return UniversityData.Enrollments
            .OrderByDescending(e => e.EnrollmentDate)
            .Take(3)
            .Select(e => $"{e.EnrollmentDate:yyyy-MM-dd} | student: {e.StudentId} | course: {e.CourseId}");
    }

    
    public IEnumerable<string> Task10_SecondPageOfCourses()
    {
        const int pageSize = 2;

        return UniversityData.Courses
            .OrderBy(c => c.Title)
            .Skip(pageSize)
            .Take(pageSize)
            .Select(c => $"{c.Title} | {c.Category}");
    }

    
    public IEnumerable<string> Task11_JoinStudentsWithEnrollments()
    {
        return UniversityData.Students
            .Join(
                UniversityData.Enrollments,
                student => student.Id,
                enrollment => enrollment.StudentId,
                (student, enrollment) =>
                    $"{student.FirstName} {student.LastName} | enrolled: {enrollment.EnrollmentDate:yyyy-MM-dd}");
    }

    
    public IEnumerable<string> Task12_StudentCoursePairs()
    {
        return UniversityData.Enrollments
            .Join(
                UniversityData.Students,
                enrollment => enrollment.StudentId,
                student => student.Id,
                (enrollment, student) => new { enrollment, student })
            .Join(
                UniversityData.Courses,
                temp => temp.enrollment.CourseId,
                course => course.Id,
                (temp, course) => $"{temp.student.FirstName} {temp.student.LastName} | {course.Title}");
    }

    
    public IEnumerable<string> Task13_GroupEnrollmentsByCourse()
    {
        return UniversityData.Enrollments
            .Join(
                UniversityData.Courses,
                enrollment => enrollment.CourseId,
                course => course.Id,
                (enrollment, course) => course.Title)
            .GroupBy(title => title)
            .Select(group => $"{group.Key} | enrollments: {group.Count()}");
    }

    
    public IEnumerable<string> Task14_AverageGradePerCourse()
    {
        return UniversityData.Enrollments
            .Where(e => e.FinalGrade.HasValue)
            .Join(
                UniversityData.Courses,
                enrollment => enrollment.CourseId,
                course => course.Id,
                (enrollment, course) => new { course.Title, Grade = enrollment.FinalGrade!.Value })
            .GroupBy(x => x.Title)
            .Select(group => $"{group.Key} | average grade: {group.Average(x => x.Grade):0.00}");
    }

    
    public IEnumerable<string> Task15_LecturersAndCourseCounts()
    {
        return UniversityData.Lecturers
            .GroupJoin(
                UniversityData.Courses,
                lecturer => lecturer.Id,
                course => course.LecturerId,
                (lecturer, courses) =>
                    $"{lecturer.FirstName} {lecturer.LastName} | course count: {courses.Count()}");
    }

    
    public IEnumerable<string> Task16_HighestGradePerStudent()
    {
        return UniversityData.Students
            .Join(
                UniversityData.Enrollments.Where(e => e.FinalGrade.HasValue),
                student => student.Id,
                enrollment => enrollment.StudentId,
                (student, enrollment) => new
                {
                    student.FirstName,
                    student.LastName,
                    Grade = enrollment.FinalGrade!.Value
                })
            .GroupBy(x => new { x.FirstName, x.LastName })
            .Select(group => $"{group.Key.FirstName} {group.Key.LastName} | highest grade: {group.Max(x => x.Grade):0.0}");
    }

    
    public IEnumerable<string> Challenge01_StudentsWithMoreThanOneActiveCourse()
    {
        return UniversityData.Students
            .Join(
                UniversityData.Enrollments.Where(e => e.IsActive),
                student => student.Id,
                enrollment => enrollment.StudentId,
                (student, enrollment) => new { student.FirstName, student.LastName })
            .GroupBy(x => new { x.FirstName, x.LastName })
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key.FirstName} {group.Key.LastName} | active courses: {group.Count()}");
    }

    
    public IEnumerable<string> Challenge02_AprilCoursesWithoutFinalGrades()
    {
        return UniversityData.Courses
            .Where(c => c.StartDate.Month == 4 && c.StartDate.Year == 2026)
            .GroupJoin(
                UniversityData.Enrollments,
                course => course.Id,
                enrollment => enrollment.CourseId,
                (course, enrollments) => new
                {
                    course.Title,
                    HasAnyFinalGrade = enrollments.Any(e => e.FinalGrade.HasValue)
                })
            .Where(x => !x.HasAnyFinalGrade)
            .Select(x => x.Title);
    }

    
    public IEnumerable<string> Challenge03_LecturersAndAverageGradeAcrossTheirCourses()
    {
        return UniversityData.Lecturers
            .GroupJoin(
                UniversityData.Courses,
                lecturer => lecturer.Id,
                course => course.LecturerId,
                (lecturer, courses) => new { lecturer, courses })
            .Select(x => new
            {
                FullName = $"{x.lecturer.FirstName} {x.lecturer.LastName}",
                Grades = x.courses
                    .Join(
                        UniversityData.Enrollments.Where(e => e.FinalGrade.HasValue),
                        course => course.Id,
                        enrollment => enrollment.CourseId,
                        (course, enrollment) => enrollment.FinalGrade!.Value)
            })
            .Where(x => x.Grades.Any())
            .Select(x => $"{x.FullName} | average grade: {x.Grades.Average():0.00}");
    }

    
    public IEnumerable<string> Challenge04_CitiesAndActiveEnrollmentCounts()
    {
        return UniversityData.Students
            .Join(
                UniversityData.Enrollments.Where(e => e.IsActive),
                student => student.Id,
                enrollment => enrollment.StudentId,
                (student, enrollment) => student.City)
            .GroupBy(city => city)
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key)
            .Select(group => $"{group.Key} | active enrollments: {group.Count()}");
    }

    private static NotImplementedException NotImplemented(string methodName)
    {
        return new NotImplementedException(
            $"Complete method {methodName} in Exercises/LinqExercises.cs and run the command again.");
    }
}
