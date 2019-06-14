;(function(global) {
  "use strict";

  var $ = global.jQuery;
  var GOVUK = global.GOVUK || {};

  GOVUK.expandableTable = {
    init: function init() {
      // loop over table row and add relevant attributes
      $(".js-expand-table-row").each(function() {
        var ariaControlId = "table-content-" + $(this).data("expand-id");
        var $expandable = $(this)
          .closest("tr")
          .nextAll("tr.js-expandble-cell:first");
        var $arrow = $(this).find("i.arrow");
        $(this).attr({
          "aria-expanded": "false",
          "aria-controls": ariaControlId
        });
        $expandable.attr({ "aria-hidden": "true", id: ariaControlId });
        $arrow.attr("aria-hidden", "true");

        // show and hide based on click and update aria tags
        $(this).on("click keypress", function(event) {
          //event.preventDefault();
          if (event.type === "keypress" && event.keyCode !== 13) return;
          if ($expandable.hasClass("js-hidden")) {
            // SHOW CONTENT
            $(this).attr({
              "aria-expanded": "true",
              "aria-controls": ariaControlId
            });
            $expandable.removeClass("js-hidden").attr("aria-hidden", "false");
            $arrow
              .attr({ class: "arrow arrow-open", "aria-hidden": "false" })
              .text("\u25bc");
          } else {
            // HIDE CONTENT
            $(this).attr("aria-expanded", "false");
            $expandable
              .addClass("js-hidden")
              .attr({ "aria-hidden": "true", id: ariaControlId });
            $arrow
              .attr({ class: "arrow arrow-closed", "aria-hidden": "true" })
              .text("\u25ba");
          }
        });
        });

        $(".js-expand-table-row-for-radio").each(function () {
            var ariaControlId = "table-content-" + $(this).data("expand-id");
            var $optionContainer = $(this).parent().parent()[0];
            var $firstOption = $optionContainer.firstElementChild;
            var $lastOption = $optionContainer.lastElementChild;
            var $expandable = $(this)
                .closest("tr")
                .nextAll("tr.js-expandble-cell-for-radio:first");
           
            var $notRejected = $(this).find("aria-labelledby");
            $(this).attr({
                "aria-expanded": "false",
                "aria-controls": ariaControlId
            });
            $expandable.attr({ "aria-hidden": "true", id: ariaControlId });
            // show and hide based on click and update aria tags
            $(this).on("click keypress", function (event) {
                if (event.type === "keypress" && event.keyCode !== 13) return;
                if (event.currentTarget.classList.contains("js-first-option")) {
                    //Clear other option in case it is marked checked
                    if ($lastOption.firstElementChild.classList.contains("checked")) {
                        $lastOption.firstElementChild.checked = false;
                        $lastOption.firstElementChild.classList.remove("checked");
                    }
                    //If current option is checked then clear 
                    if (event.currentTarget.classList.contains("checked")) {
                        $firstOption.firstElementChild.checked = false;
                        event.currentTarget.classList.remove("checked");
                    }
                    else {
                        //Current option was not checked so mark checked
                        $firstOption.firstElementChild.checked = true;
                        event.currentTarget.classList.add("checked");
                    }
                } else {
                    //Clear other option in case it is marked checked
                    if ($firstOption.firstElementChild.classList.contains("checked")) {
                        $firstOption.firstElementChild.checked = false;
                        $firstOption.firstElementChild.classList.remove("checked");
                    }
                    //If current option is checked then clear option just hide reason in case it is showing
                    if (event.currentTarget.classList.contains("checked")) {
                        $lastOption.firstElementChild.checked = false;
                        event.currentTarget.classList.remove("checked");
                        $(this).attr("aria-expanded", "false");
                        $expandable
                            .addClass("js-hidden")
                            .attr({ "aria-hidden": "true", id: ariaControlId });
                        return;
                    }
                    else {
                        //Current option was not checked so mark checked
                        $lastOption.firstElementChild.checked = true;
                        event.currentTarget.classList.add("checked");
                    }
                }
                if ($(this).attr("data-hide")) {
                    //FORCE HIDE CONTENT
                    $(this).attr("aria-expanded", "false");
                    $expandable
                        .addClass("js-hidden")
                        .attr({ "aria-hidden": "true", id: ariaControlId });
                }
                else if ($expandable.hasClass("js-hidden")) {
                    // SHOW CONTENT
                    $(this).attr({
                        "aria-expanded": "true",
                        "aria-controls": ariaControlId
                    });
                    $expandable.removeClass("js-hidden").attr("aria-hidden", "false");
                  
                } 
            });
        });

    }
  };

  global.GOVUK = GOVUK;
})(window);
