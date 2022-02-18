(function ($) {

    if ($.validator != undefined) {

        $.validator.setDefaults({
            ignore: ":hidden"
        });

        $.validator.unobtrusive.adapters.addBool("requiredcb", "required");

        $.validator.addMethod('umbracoforms_regex', function(value, element) {

            var regex = $(element).attr("data-regex");
            var val = $(element).val();
            if (val.length == 0) {
                return true;
            }
            return val.match(regex);
        });

        $.validator.unobtrusive.adapters.addBool("regex", "umbracoforms_regex");

        $('.contour input[type=submit]').not('.cancel').click(function (evt) {
            evt.preventDefault();
            var self = $(this);
            var frm = self.closest('form');
            frm.validate();
            if (frm.valid()) {
                frm.submit();
                self.attr('disabled', 'disabled');

            }
        });
    }
} (jQuery));

