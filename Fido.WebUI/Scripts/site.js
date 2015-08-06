$(function()
{
    $('#side-menu').metisMenu();
});

// Loads the correct sidebar on window load, collapses the sidebar on window resize. Sets the min-height of
// #page-wrapper to window size
$(function()
{
    $(window).bind("load resize",
        function ()
        {
            topOffset = 50;
            width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;

            if (width < 768)
            {
                $('div.navbar-collapse').addClass('collapse')
                topOffset = 100; // 2-row-menu
            }
            else
            {
                $('div.navbar-collapse').removeClass('collapse')
            }

            height = (this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height;
            height = height - topOffset;

            if (height < 1) height = 1;

            if (height > topOffset)
            {
                $("#page-wrapper").css("min-height", (height) + "px");
            }
        })
});

$(function()
{
    // Attach modal-container bootstrap attributes to links with .modal-link class. When a link is clicked
    // with these attributes, bootstrap will display the href content in a modal dialog, #modal-container.
    $('body').on('click', '.modal-link',
        function (e)
        {
            e.preventDefault();
            $(this).attr('data-target', '#modal-container');
            $(this).attr('data-toggle', 'modal');
        });

    // Attach listener to .modal-close-btn's so that when the button is pressed the modal dialog disappears.
    $('body').on('click', '.modal-close-btn',
        function ()
        {
            alert('.modal-close-btn click');
            $('#modal-container').modal('hide');
        });

    // The hidden.bs.modal event is fired after the modal dialog box closes. I presume this includes if it
    // was closed from the "X" in the upper right-hand corner of the window. When it does close, the below
    // function will clear the model cache so that the next time it is displayed, the fields are clear.
    $('#modal-container').on('hidden.bs.modal',
        function ()
        {
            //alert('#modal-container hidden.bs.model');
            $(this).removeData('bs.modal');
        });
});
