(function ($) {
    var controller = 'Register';
    
    refreshClickHandlers('approved');

    function refreshClickHandlers(fragment) {
        onClickChangePage(
            "." + fragment + "-page",
            "ChangePageViewOrganisationApprovedStandards",
            "#" + fragment,
            "#" + fragment,
            function () { return refreshClickHandlers(fragment); }
        );
        onSelectApplicationsPerPage(
            "." + fragment + "-per-page",
            "ChangeStandardsPerPageViewOrganisationApprovedStandards",
            "#" + fragment,
            "#" + fragment,
            function () { return refreshClickHandlers(fragment); }
        );
        onSelectSortableColumnHeader(
            "." + fragment + "-sort",
            "SortViewOrganisationApprovedStandards",
            "#" + fragment,
            "#" + fragment,
            function () { return refreshClickHandlers(fragment); }            
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
                var allRouteData = JSON.parse($(obj).attr("data-all-route-data"));
                var specificRouteData = { pageIndex: $(obj).attr("data-pageIndex") };
                changeStandardsPartial(
                    actionMethod,
                    fragment,
                    combineJson(allRouteData, specificRouteData),
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
            var allRouteData = JSON.parse($(this).attr("data-all-route-data"));
            var specificRouteData = { itemsPerPage: this.value };
            
            changeStandardsPartial(
                actionMethod,
                fragment,
                combineJson(allRouteData, specificRouteData),
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
                var allRouteData = JSON.parse($(obj).attr("data-all-route-data"));
                var specificRouteData = { sortColumn: $(obj).attr("data-sortColumn"), sortDirection: $(obj).attr("data-sortDirection") };
                changeStandardsPartial(
                    actionMethod,
                    fragment,
                    combineJson(allRouteData, specificRouteData),
                    containerId,
                    refreshFunction
                );
                event.preventDefault();
            });
        });
    }

    function changeStandardsPartial(
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

    function combineJson(first, second) {
        var attributes = Object.keys(second);

        for (var i = 0; i < attributes.length; i++) {
            if (!first.hasOwnProperty(attributes[i])) {
                first[attributes[i]] = second[attributes[i]];
            }
        }

        return first;
    }

})(jQuery);
