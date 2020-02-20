window.onload = function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/reportHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    connection.on("ReportUpdated", updateRenderingStatus);
    connection.start().catch(err => console.error(err.toString()));

    const $loader = $("#loader");
    const $iframe = $("#google_doc_iframe");

    $("#downloadButton").click(() => {
        window.location.href = `/Home/GetDocx?templateName=${getTemplateName()}`;
    });

    initTemplateName();

    updateRenderingStatus();

    function updateRenderingStatus() {
        $("#error_iframe").hide();
        $.getJSON(`/Home/Render?templateName=${getTemplateName()}`)
            .done(res => {
                $("#log").html(res.log || "none");
                if ($iframe.attr("src") !== res.googleDocUrl)
                    $iframe.attr("src", res.googleDocUrl);
                $loader.hide();
                $iframe.show();
            })
            .fail(handleError)
            .always(() => $("#time").html(new Date().toLocaleTimeString()));
    }

    function initTemplateName() {
        $("#templateName").val(localStorage.getItem("templateName") || "Simple");
        $("#templateNameButton").click(() => {
            localStorage.setItem("templateName", getTemplateName());
            $loader.show();
            updateRenderingStatus();
        });
    }

    function resetTemplateName() {
        localStorage.removeItem("templateName");
        $("#templateName").val("Simple");
    }

    function getTemplateName() {
        return $("#templateName").val();
    }

    function handleError(er) {
        $loader.hide();
        resetTemplateName();
        const iframe = $("#error_iframe");
        iframe.show();
        iframe[0].src = `data:text/html;charset=utf-8,${escape(er.responseText)}`;
    }
};
