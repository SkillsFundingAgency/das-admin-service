(function ($) {
    var controller = $("#ControllerName").val();
    
    refreshClickHandlers('New','new');
    refreshClickHandlers('InProgress', 'in-progress');
    refreshClickHandlers('Feedback', 'feedback');
    refreshClickHandlers('Approved', 'approved');

    function refreshClickHandlers(reviewStatus, fragment) {
        onClickChangePage(
            "." + fragment + "-page",
            "ChangePage" + reviewStatus + "Applications",
            "#" + fragment,
            "#" + fragment,
            function () { return refreshClickHandlers(reviewStatus, fragment); }
        );
        onSelectApplicationsPerPage(
            "." + fragment + "-per-page",
            "ChangeApplicationsPerPage" + reviewStatus + "Applications",
            "#" + fragment,
            "#" + fragment,
            function () { return refreshClickHandlers(reviewStatus, fragment); }
        );
        onSelectSortableColumnHeader(
            "." + fragment + "-sort",
            "Sort" + reviewStatus + "Applications",
            "#" + fragment,
            "#" + fragment,
            function () { return refreshClickHandlers(reviewStatus, fragment); }            
        );
    }

    function onClickChangePage(
        linkClass,
        actionMethod,
        fragment,
        containerId,
        refreshFunction
    ) {
        $(linkClass).each(function (i, obj) {
            $(obj).click(function (event) {
                var pageIndex = $(obj).attr("data-pageIndex");
                changeApplicationsPartial(
                    actionMethod,
                    fragment,
                    { pageIndex: pageIndex },
                    containerId,
                    refreshFunction
                );
                event.preventDefault();
            });
        });
    }

    function onSelectApplicationsPerPage(
        selectClass,
        actionMethod,
        fragment,
        containerId,
        refreshFunction
    ) {
        $(selectClass).change(function () {
            var selectedVal = $(this)
                .find(":selected")
                .val();
            changeApplicationsPartial(
                actionMethod,
                fragment,
                { itemsPerPage: selectedVal },
                containerId,
                refreshFunction
            );
            event.preventDefault();
        });
    }

    function onSelectSortableColumnHeader(
        linkClass,
        actionMethod,
        fragment,
        containerId,
        refreshFunction
    ) {
        $(linkClass).each(function (i, obj) {
            $(obj).click(function (event) {
                var sortColumn = $(obj).attr("data-sortColumn");
                var sortDirection = $(obj).attr("data-sortDirection");
                changeApplicationsPartial(
                    actionMethod,
                    fragment,
                    { sortColumn: sortColumn, sortDirection: sortDirection },
                    containerId,
                    refreshFunction
                );
                event.preventDefault();
            });
        });
    }

    function changeApplicationsPartial(
        actionMethod,
        fragment,
        data,
        containerId,
        refreshFunction
    ) {
        $.ajax({
            url: "/" + controller + "/" + actionMethod + "Partial",
            type: "get",
            data: data,
            success: function (response) {
                var jqContainer = $(containerId);
                jqContainer.html(response);

                if (fragment !== null) {
                    var nonPartialActionMethod = this.url.replace(actionMethod + "Partial", actionMethod) + fragment;
                    history.replaceState(null, null, nonPartialActionMethod);
                }

                refreshFunction();
            },
            error: function () {
                window.location = "/" + controller;
            }
        });
    }    
})(jQuery);
