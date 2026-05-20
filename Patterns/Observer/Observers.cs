using NMB_HLabSys.Patterns.Observer;
using NMB_HLabSys.Patterns.Strategy;

namespace NMB_HLabSys.Patterns.Observer
{
    public class PatientSubmissionNotifier : ITestRequestObserver
    {
        private readonly INotificationStrategy _strategy;

        public PatientSubmissionNotifier(INotificationStrategy strategy)
            => _strategy = strategy;

        public void OnStatusChanged(TestRequestStatusChangedEvent e)
        {
            if (e.NewStatus != "Submitted") return;

            _strategy.Send(
                to: e.PatientEmail,
                subject: "Your lab test request has been submitted",
                body: $"Hi {e.PatientName},\n\n" +
                         $"Your test request #{e.TestRequestId} has been submitted " +
                         $"and is being processed by the laboratory.\n\n" +
                         $"You will be notified when your results are available."
            );
        }
    }

    // ----------------------------------------------------------
    // Notifies the DOCTOR when all tests have been verified,
    // and attaches the PDF results
    // ----------------------------------------------------------
    public class DoctorResultsNotifier : ITestRequestObserver
    {
        private readonly INotificationStrategy _strategy;

        public DoctorResultsNotifier(INotificationStrategy strategy)
            => _strategy = strategy;

        public void OnStatusChanged(TestRequestStatusChangedEvent e)
        {
            if (e.NewStatus != "Verified") return;

            _strategy.SendWithAttachment(
                to: e.DoctorEmail,
                subject: $"Results ready — Request #{e.TestRequestId}",
                body: $"Dear Dr {e.DoctorName},\n\n" +
                            $"All tests for request #{e.TestRequestId} have been " +
                            $"verified. Please find the results attached.\n\n" +
                            $"Please log in to review and release results to the patient.",
                attachment: e.ResultPdfBytes,
                filename: $"Results_{e.TestRequestId}.pdf"
            );
        }
    }

    // ----------------------------------------------------------
    // Notifies the PATIENT when the doctor releases results
    // ----------------------------------------------------------
    public class PatientResultsReleasedNotifier : ITestRequestObserver
    {
        private readonly INotificationStrategy _strategy;

        public PatientResultsReleasedNotifier(INotificationStrategy strategy)
            => _strategy = strategy;

        public void OnStatusChanged(TestRequestStatusChangedEvent e)
        {
            if (e.NewStatus != "ReleasedByDoctor") return;

            _strategy.Send(
                to: e.PatientEmail,
                subject: "Your lab results are available",
                body: $"Hi {e.PatientName},\n\n" +
                         $"Your results for request #{e.TestRequestId} are now " +
                         $"available. Please log in to view them."
            );
        }
    }
}
