var svnDirectory = svnDirectory || {};

(function ($, svnDirectory) {
    
    $.extend(svnDirectory, {



    });

})(jQuery, svnDirectory)

$('.svn-directory-button').click(function () {

    var $item = $(this).parents('.item:first');
    var objectData = {
        'Name': $item.data('name')
    }

    $.ajax({
        type: 'GET',
        url: 'Directory/Edit',
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        data: objectData,
        success: function (response) {
            $('#svn-directory-info').html(response);
        }
    });

});