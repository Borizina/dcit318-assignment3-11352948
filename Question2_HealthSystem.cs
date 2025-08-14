using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthSystemQ2
{
    // a) Generic repository
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => new List<T>(items);

        public T? GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var idx = items.FindIndex(x => predicate(x));
            if (idx >= 0)
            {
                items.RemoveAt(idx);
                return true;
            }
            return false;
        }
    }

    // b) Patient and c) Prescription
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => $"Patient {{ Id={Id}, Name={Name}, Age={Age}, Gender={Gender} }}";
    }

    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() => $"Prescription {{ Id={Id}, PatientId={PatientId}, Medication={MedicationName}, DateIssued={DateIssued:yyyy-MM-dd} }}";
    }

    // g) HealthSystemApp
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Smith", 28, "Female"));
            _patientRepo.Add(new Patient(2, "Bright Anku", 24, "Male"));
            _patientRepo.Add(new Patient(3, "Kwame Mensah", 35, "Male"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(102, 2, "Ibuprofen", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(103, 1, "Vitamin C", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Loratadine", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(105, 2, "Paracetamol", DateTime.Today));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var p in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(p.PatientId))
                    _prescriptionMap[p.PatientId] = new List<Prescription>();
                _prescriptionMap[p.PatientId].Add(p);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("== All Patients ==");
            foreach (var p in _patientRepo.GetAll())
                Console.WriteLine(p);
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Console.WriteLine($"\n== Prescriptions for PatientId {id} ==");
            var list = GetPrescriptionsByPatientId(id);
            if (list.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
                return;
            }
            foreach (var pr in list) Console.WriteLine(pr);
        }

        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            // Select one patient to display prescriptions
            app.PrintPrescriptionsForPatient(2);
        }
    }
}