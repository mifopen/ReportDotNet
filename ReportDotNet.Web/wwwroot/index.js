window.onload = function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/reportHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    connection.on("ReportUpdated", refresh);
    connection.start().catch(err => console.error(err.toString()));
    const pagePool = new PagePool();
    const $loader = $("#loader");
    const $pages = $("#pages");

    $("#downloadButton").click(() => {
        window.location.href = `/Home/GetDocx?templateName=${getTemplateName()}`;
    });
    
    initTemplateName();

    refresh();

    function handleError(er) {
        $loader.hide();
        resetTemplateName(); 
        const iframe = $("#error_iframe");
        iframe.show();
        iframe[0].src = `data:text/html;charset=utf-8,${escape(er.responseText)}`;
    }

    function refresh() {
        $("#error_iframe").hide();
        $.getJSON(`/Home/Render?templateName=${getTemplateName()}`)
            .done(res => {
                $("#log").html(res.Log || "none");
                const pages = pagePool.acquire(res.PagesCount);
                for (let i = 0; i < res.PagesCount; i++)
                    pages[i].setImageSrc(`/Home/GetPage?pageNumber=${i}&hash=${Date.now()}`);
                $pages.show();
                $loader.hide();
            })
            .fail(handleError)
            .always(() => $("#time").html(new Date().toLocaleTimeString()));
    }

    function initTemplateName() {
        $("#templateName").val(localStorage.getItem("templateName") || "Simple");
        $("#templateNameButton").click(() => {
            localStorage.setItem("templateName", getTemplateName());
            $loader.show();
            refresh();
        });
    }
    
    function resetTemplateName(){
       localStorage.removeItem("templateName");
        $("#templateName").val("Simple");
    }

    function getTemplateName() {
        return $("#templateName").val();
    }
};

class Page {
    constructor($html) {
        this.normalPageWidth = 500;
        this.zoomedPageWidth = 1100;
        this.$html = $html;
        this.$img = $html.find("img");
        this.$img[0].width = this.normalPageWidth;
        this.$html.find(".page").click(() => this.toggleZoom());
        this.zoomed = false;
    }

    setImageSrc(src) {
        this.$img.attr("src", src);
    }

    show() {
        this.$html.show();
    }

    hide() {
        this.$html.hide();
    }

    toggleZoom() {
        if (Page.zoomedPage && Page.zoomedPage !== this)
            Page.zoomedPage.toggleZoom();

        if (this.zoomed) {
            this.$img[0].width = this.normalPageWidth;
            Page.zoomedPage = null;
            this.zoomed = false;
        } else {
            this.$img[0].width = this.zoomedPageWidth;
            Page.zoomedPage = this;
            this.zoomed = true;
        }
    }
}

class PagePool {
    constructor() {
        this.pages = [];
    }

    acquire(count) {
        this.maxPageCount = (this.maxPageCount || 0) < count
            ? count
            : this.maxPageCount;
        const result = [];

        for (let i = 0; i < count; i++) {
            let page = this.pages[i];
            if (!page) {
                const $html = this.__createPageHtml(i);
                page = this.pages[i] = new Page($html);
            }
            result.push(page);
            page.show();
        }

        for (let i = count; i < this.maxPageCount; i++)
            if (this.pages[i])
                this.pages[i].hide();

        return result;
    }

    __createPageHtml(index) {
        const $result = $("#pageWrapTemplate").clone();
        $result.find(".pageNumber").html(index + 1);
        $("#pages").append($result);
        return $result;
    }
}
