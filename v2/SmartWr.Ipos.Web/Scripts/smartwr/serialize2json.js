(function (n) {
    n.fn.serialize2JSON = function () {

        var t = {};
        var i = this.serializeArray();
        return n.each(i, function () {

            if (t[this.name] && typeof (t[this.name]) === 'string') {
                t[this.name] = [t[this.name]];
            }

            t[this.name] ? (t[this.name].push && t[this.name].push(this.value || "")) : t[this.name] = this.value || ""
        }), t
    }
})(jQuery)