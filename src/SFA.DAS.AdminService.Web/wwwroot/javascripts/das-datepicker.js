(function () {
  var flatpickrFromOptions = {
    allowInput: true,
    maxDate: "today",
    wrap: true,
    disableMobile: "true",
    locale: {
      firstDayOfWeek: 1
    },
    onChange: function (selectedDates, dateStr) {
      toFlatpickr.set("minDate", dateStr);
    },
  };

  var flatpickrToOptions = {
    allowInput: true,
    maxDate: "today",
    wrap: true,
    disableMobile: "true",
    locale: {
      firstDayOfWeek: 1
    }
    // altInput: true,
    // altFormat: "j F Y", // GDS date format: 30 June 2020
  };

  var fromFlatpickr = flatpickr(".js-flatpickr-from", flatpickrFromOptions);
  var toFlatpickr = flatpickr(".js-flatpickr-to", flatpickrToOptions);

  var removeSelectedClassFromNodeList = function (nodeListSelector) {
    var nodeList = document.querySelectorAll("." + nodeListSelector);
    for (var i = 0; i < nodeList.length; i++) {
      nodeList[i].classList.remove(nodeListSelector);
    }
  };

  document
    .querySelector('[data-module="das-clear-form-errors"]')
    .addEventListener("submit", function () {
      var errorSummary = document.querySelector(".govuk-error-summary");
      var errorMessages = document.querySelectorAll(".govuk-error-message");

      if (!errorSummary || !errorMessages) return;

      errorSummary.style.display = "none";
      for (var i = 0; i < errorMessages.length; i++) {
        errorMessages[i].style.display = "none";
      }
      removeSelectedClassFromNodeList("govuk-form-group--error");
      removeSelectedClassFromNodeList("govuk-select--error");
      removeSelectedClassFromNodeList("govuk-input--error");
    });
})();
