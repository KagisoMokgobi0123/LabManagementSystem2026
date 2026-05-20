using System.Text;


namespace NMB_HLabSys.Patterns.Decorator
{
    public interface ILabReport
    {
        string GenerateContent();   // returns the report body text / HTML
        string GetTitle();
    }

    // ----------------------------------------------------------
    // Concrete Component: the plain result report
    // ----------------------------------------------------------
    public class BaseLabReport : ILabReport
    {
        private readonly int _requestId;
        private readonly string _patientName;
        // In a real system this would be a list of TestResult objects
        private readonly string _resultsHtml;

        public BaseLabReport(int requestId, string patientName, string resultsHtml)
        {
            _requestId = requestId;
            _patientName = patientName;
            _resultsHtml = resultsHtml;
        }

        public string GetTitle()
            => $"Lab Results — Request #{_requestId}";

        public string GenerateContent()
            => $"<h2>Patient: {_patientName}</h2>" +
               $"<h3>Request: #{_requestId}</h3>" +
               $"<table>{_resultsHtml}</table>";
    }

    // ----------------------------------------------------------
    // Base Decorator (inherits interface, wraps another report)
    // ----------------------------------------------------------
    public abstract class LabReportDecorator : ILabReport
    {
        protected readonly ILabReport _inner;

        protected LabReportDecorator(ILabReport inner)
            => _inner = inner;

        public virtual string GetTitle()
            => _inner.GetTitle();

        public virtual string GenerateContent()
            => _inner.GenerateContent();
    }

    // ----------------------------------------------------------
    // Decorator 1: Highlights abnormal results in red
    // ----------------------------------------------------------
    public class AbnormalHighlightDecorator : LabReportDecorator
    {
        // In a real system these would be passed in from the test type's normal range
        private readonly IEnumerable<string> _abnormalTestNames;

        public AbnormalHighlightDecorator(ILabReport inner,
                                          IEnumerable<string> abnormalTestNames)
            : base(inner)
            => _abnormalTestNames = abnormalTestNames;

        public override string GenerateContent()
        {
            var content = _inner.GenerateContent();

            // Wrap each abnormal test name in a red highlight span
            foreach (var name in _abnormalTestNames)
                content = content.Replace(
                    name,
                    $"<span style='color:red; font-weight:bold'>⚠ {name}</span>"
                );

            return content + "<p style='color:red'>⚠ Highlighted values are outside the normal range.</p>";
        }
    }

    // ----------------------------------------------------------
    // Decorator 2: Adds the doctor's clinical notes section
    // ----------------------------------------------------------
    public class ClinicalNotesDecorator : LabReportDecorator
    {
        private readonly string _clinicalNotes;
        private readonly string _doctorName;

        public ClinicalNotesDecorator(ILabReport inner, string clinicalNotes, string doctorName)
            : base(inner)
        {
            _clinicalNotes = clinicalNotes;
            _doctorName = doctorName;
        }

        public override string GenerateContent()
            => _inner.GenerateContent() +
               $"<hr/><h4>Clinical Notes — Dr {_doctorName}</h4>" +
               $"<p>{_clinicalNotes}</p>";
    }

    // ----------------------------------------------------------
    // Decorator 3: Adds the NMB-HLabSys letterhead header
    // ----------------------------------------------------------
    public class LetterheadDecorator : LabReportDecorator
    {
        public LetterheadDecorator(ILabReport inner) : base(inner) { }

        public override string GenerateContent()
            => "<div style='text-align:center'>" +
               "<h1>Nelson Mandela Bay Haematology Diagnostic Laboratories</h1>" +
               "<p>Tel: 041-000-0000 | lab@nmbhdl.co.za</p><hr/>" +
               "</div>" +
               _inner.GenerateContent() +
               "<p style='font-size:10px'>This report is confidential and intended for the named recipient only.</p>";
    }
}
