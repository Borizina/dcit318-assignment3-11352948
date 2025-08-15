using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GradingSystemQ4
{
    // a) Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }

        public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }

    // b) & c) Custom exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // d) Processor
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using var reader = new StreamReader(inputFilePath);
            string? line;
            int lineNo = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 3)
                    throw new MissingFieldException($"Line {lineNo}: Missing fields (expected 3, got {parts.Length}).");

                var idStr = parts[0].Trim();
                var nameStr = parts[1].Trim();
                var scoreStr = parts[2].Trim();

                if (!int.TryParse(idStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                    throw new FormatException($"Line {lineNo}: Invalid ID format '{idStr}'.");

                if (!int.TryParse(scoreStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var score))
                    throw new InvalidScoreFormatException($"Line {lineNo}: Invalid score format '{scoreStr}'.");

                students.Add(new Student(id, nameStr, score));
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using var writer = new StreamWriter(outputFilePath, false);
            foreach (var s in students)
            {
                writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
            }
        }
    }

    // e) Main flow
    public static class Program
    {
        public static void Main()
        {
            // Paths can be adjusted as needed
            string inputPath = "grades_input.txt";
            string outputPath = "grades_report.txt";

            try
            {
                var processor = new StudentResultProcessor();
                var students = processor.ReadStudentsFromFile(inputPath);
                processor.WriteReportToFile(students, outputPath);
                Console.WriteLine($"Report written to: {outputPath}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Input file not found. Please ensure 'grades_input.txt' exists next to the executable.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}