using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Patterns.Observer;
using NMB_HLabSys.Patterns.Strategy;

namespace NMB_HLabSys.Patterns.Factory
{
    public enum Urgency { Routine, Urgent, Stat }

    // ----------------------------------------------------------
    // Specialised TestRequest subclasses (one per urgency level)
    // ----------------------------------------------------------
    public class RoutineTestRequest : TestRequestsModel
    {
        public int TurnaroundHours { get; } = 48;
        public RoutineTestRequest() => Console.WriteLine("[Factory] Created ROUTINE request.");
    }

    public class UrgentTestRequest : TestRequestsModel
    {
        public int TurnaroundHours { get; } = 4;
        public UrgentTestRequest() => Console.WriteLine("[Factory] Created URGENT request.");
    }

    public class StatTestRequest : TestRequestsModel
    {
        public int TurnaroundHours { get; } = 1;
        public bool AlertLabManager { get; } = true;  // STAT always alerts manager
        public StatTestRequest() => Console.WriteLine("[Factory] Created STAT request.");
    }

    // ----------------------------------------------------------
    // The Factory — also wires up the Observer chain so every
    // new request already has its notification observers attached
    // ----------------------------------------------------------
    public static class TestRequestFactory
    {
        // The notification strategy is injected once here;
        // all observers share it (Strategy pattern in use)
        private static INotificationStrategy _notificationStrategy
            = new EmailNotificationStrategy("smtp.nmbhdl.co.za", 587, "noreply@nmbhdl.co.za");

        // Call this in tests / dev to swap to console output
        public static void UseStrategy(INotificationStrategy strategy)
            => _notificationStrategy = strategy;

        // ------ THE FACTORY METHOD ------
        public static TestRequestsModel Create(Urgency urgency,
                                         string patientEmail, string patientName,
                                         string doctorEmail, string doctorName)
        {
            // 1. Create the right subtype
            TestRequestsModel request = urgency switch
            {
                Urgency.Routine => new RoutineTestRequest(),
                Urgency.Urgent => new UrgentTestRequest(),
                Urgency.Stat => new StatTestRequest(),
                _ => throw new ArgumentOutOfRangeException(nameof(urgency))
            };

            // 2. Populate common fields
            request.Patient.Email = patientEmail;
            request.Patient.Name = patientName;
           

            // 3. Wire up observers (Observer pattern)
            request.AddObserver(new PatientSubmissionNotifier(_notificationStrategy));
            request.AddObserver(new DoctorResultsNotifier(_notificationStrategy));
            request.AddObserver(new PatientResultsReleasedNotifier(_notificationStrategy));

            return request;
        }
    }
}
