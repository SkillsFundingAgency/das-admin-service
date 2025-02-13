(function ($) {

    $(document).ready(function () {

        $("#standardButton").click(function (event) {
            $("#searchMode").val(window.searchmodes.standards);
            $("#FirstName").val("");
            $("#LastName").val("");
            $("#Day").val("");
            $("#Month").val("");
            $("#Year").val("");
        });

        $("#frameworkButton").click(function (event) {
            $("#searchMode").val(window.searchmodes.frameworks);
            $("#SearchString").val("");
        });
    });

})(jQuery);
