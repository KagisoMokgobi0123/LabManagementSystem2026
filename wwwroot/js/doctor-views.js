(function () {
    "use strict";
    function classify(text) {
        var value = (text || "").replace(/\s+/g, " ").trim().toLowerCase();
        if (!value) return "";
        if (/critical|abnormal|cancelled|canceled|rejected|overdue|failed|high risk/.test(value)) return "doctor-status-danger";
        if (/urgent|high|pending|awaiting|submitted|in progress|samples received|review required|unverified/.test(value)) return "doctor-status-warning";
        if (/completed|released|approved|verified|normal|active|received|success/.test(value)) return "doctor-status-success";
        if (/routine|medium|scheduled|requested|processing/.test(value)) return "doctor-status-info";
        if (/inactive|unknown|not started|draft|n\/a/.test(value)) return "doctor-status-neutral";
        return "";
    }
    function decorate() {
        var root = document.querySelector(".doctor-main");
        if (!root) return;
        root.querySelectorAll(".badge, .status-badge, td, .request-status, .result-status, .urgency-badge").forEach(function (el) {
            if (el.dataset.doctorColourApplied === "true") return;
            var text = (el.textContent || "").replace(/\s+/g, " ").trim();
            if (text.length > 42) return;
            var cls = classify(text);
            if (!cls) return;
            el.classList.add("doctor-data-badge", cls);
            el.dataset.doctorColourApplied = "true";
        });
    }
    if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", decorate, { once: true });
    else decorate();
})();
