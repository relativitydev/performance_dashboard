$.fn.moreless = function () {

	// Configure/customize these variables.
	var showChar = 64;  // How many characters are shown by default
	var ellipsestext = "...";
	var moretext = "Show more >";
	var lesstext = "Show less";


	this.each(function () {
		var content = $(this).html();

		if (content.length > showChar) {

			var c = content.substr(0, showChar);
			var h = content.substr(showChar, content.length - showChar);

			var html = c + '<span class="moreellipses">' + ellipsestext + '&nbsp;</span><span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';

			$(this).html(html);
		}

		$('.morecontent span').css('display', 'none');
		$('.morelink').css('display', 'block');



	});

	$(".morelink").click(function () {

		var classes = $(this).parent().parent().attr("class").split(' ');
		var group = classes[1];

		$('.' + group + ' .morelink').each(function () {
			if ($(this).hasClass("less")) {
				$(this).removeClass("less");
				$(this).html(moretext);
			} else {
				$(this).addClass("less");
				$(this).html(lesstext);
			}

			$(this).parent().prev().toggle();
			$(this).prev().toggle();
		});

		return false;
	});

};



