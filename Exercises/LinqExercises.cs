using LinqConsoleLab.EN.Data;

namespace LinqConsoleLab.EN.Exercises;

public sealed class LinqExercises
{
    /// Task:
    /// Find all students who live in Warsaw.
    /// Return the index number, full name, and city.
    ///
    public IEnumerable<string> Task01_StudentsFromWarsaw()
    {
        return UniversityData.Students
            .Where(s => s.City == "Warsaw")
            .Select(s => $"{s.IndexNumber} | {s.FirstName} {s.LastName} | {s.City}");
    }
    
    /// Task:
    /// Build a list of all student email addresses.
    /// Use projection so that you do not return whole objects.
    ///
    public IEnumerable<string> Task02_StudentEmailAddresses()
    {
        return UniversityData.Students
            .Select(s => s.Email);
    }

    /// Task:
    /// Sort students alphabetically by last name and then by first name.
    /// Return the index number and full name.
    ///
    public IEnumerable<string> Task03_StudentsSortedAlphabetically()
    {
        return UniversityData.Students
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Select(s => $"{s.IndexNumber} | {s.FirstName} {s.LastName}");
    }

    /// Task:
    /// Find the first course from the Analytics category.
    /// If such a course does not exist, return a text message.
    ///
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
    

    /// Task:
    /// Check whether there is at least one inactive enrollment in the data set.
    /// Return one line with a True/False or Yes/No answer.
    ///
    public IEnumerable<string> Task05_IsThereAnyInactiveEnrollment()
    {
        var hasInactive = UniversityData.Enrollments.Any(e => !e.IsActive);
        return [$"Inactive enrollment exists: {hasInactive}"];
    }

    /// Task:
    /// Check whether every lecturer has a department assigned.
    /// Use a method that validates the condition for the whole collection.
    ///
    public IEnumerable<string> Task06_DoAllLecturersHaveDepartment()
    {
        var allHaveDepartment = UniversityData.Lecturers
            .All(l => !string.IsNullOrWhiteSpace(l.Department));

        return [$"All lecturers have a department: {allHaveDepartment}"];
    }

    /// Task:
    /// Count how many active enrollments exist in the system.
    ///
    public IEnumerable<string> Task07_CountActiveEnrollments()
    {
        var activeCount = UniversityData.Enrollments.Count(e => e.IsActive);
        return [$"Active enrollments: {activeCount}"];
    }

    /// Task:
    /// Return a sorted list of distinct student cities.
    ///
    public IEnumerable<string> Task08_DistinctStudentCities()
    {
        return UniversityData.Students
            .Select(s => s.City)
            .Distinct()
            .OrderBy(city => city);
    }

    /// Task:
    /// Return the three newest enrollments.
    /// Show the enrollment date, student identifier, and course identifier.
    ///
    public IEnumerable<string> Task09_ThreeNewestEnrollments()
    {
        return UniversityData.Enrollments
            .OrderByDescending(e => e.EnrollmentDate)
            .Take(3)
            .Select(e => $"{e.EnrollmentDate:yyyy-MM-dd} | student: {e.StudentId} | course: {e.CourseId}");
    }

    /// Task:
    /// Implement simple pagination for the course list.
    /// Assume a page size of 2 and return the second page of data.
    ///
    public IEnumerable<string> Task10_SecondPageOfCourses()
    {
        const int pageSize = 2;

        return UniversityData.Courses
            .OrderBy(c => c.Title)
            .Skip(pageSize)
            .Take(pageSize)
            .Select(c => $"{c.Title} | {c.Category}");
    }

    /// Task:
    /// Join students with enrollments by StudentId.
    /// Return the full student name and the enrollment date.
    ///
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

    /// Task:
    /// Prepare all student-course pairs based on enrollments.
    /// Use an approach that flattens the data into a single result sequence.
    ///
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

    /// Task:
    /// Group enrollments by course and return the course title together with the number of enrollments.
    ///
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

    /// Task:
    /// Calculate the average final grade for each course.
    /// Ignore records where the final grade is null.
    ///
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

    /// Task:
    /// For each lecturer, count how many courses are assigned to that lecturer.
    /// Return the full lecturer name and the course count.
    ///
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

    /// Task:
    /// For each student, find the highest final grade.
    /// Skip students who do not have any graded enrollment yet.
    ///
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

    /// Challenge:
    /// Find students who have more than one active enrollment.
    /// Return the full name and the number of active courses.
    ///
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

    /// Challenge:
    /// List the courses that start in April 2026 and do not have any final grades assigned yet.
    ///
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

    /// Challenge:
    /// Calculate the average final grade for every lecturer across all of their courses.
    /// Ignore missing grades but still keep the lecturers in mind as the reporting dimension.
    ///
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
    /// Challenge:
    /// Show student cities and the number of active enrollments created by students from each city.
    /// Sort the result by the active enrollment count in descending order.
    
    
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
