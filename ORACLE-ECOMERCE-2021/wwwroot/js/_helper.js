/*  
  #Bootstrap bugs
  - dropdown click đâu cũng tự collapse
  - offcanvas nút toggle bị lỗi tự đóng( khi có children )
*/

$('[data-bs-toggle="offcanvas"]').on('click', (e) => e.stopPropagation());
$('[drop-propagation]').on('click', (e) => e.stopPropagation());




/*  
  #Navigation bar & Mobile menu
*/


//
// Show nav bar on hover into header info
// $('#header-info').mouseenter((e) =>
//   document.getElementById("navbar").style.top = "0");
//
// Hide nav bar when scrolling down
// var headerInfoHeight = document.getElementById('header-info').offsetHeight;
// window.onscroll = function () {
//   if (window.pageYOffset >= headerInfoHeight) {
//     document.getElementById("header").style.top = -headerInfoHeight + "px";
//   } else if (window.pageYOffset < headerInfoHeight - 10){
//     document.getElementById("header").style.top = "0";
//   }
// }

//
// #Search bar

const SEARCH = {
  delay: 1000,
  container: '#page-search',
  firstSearch: true,
  showed: false,
  clearSearchResult: function () {
    $('#search-result').html("");
  },
  showLoading: function () {
    $('#search-loading').css('display', 'block');
    $('#search-result').css('display', 'none');
  },
  hideLoading: function () {
    $('#search-loading').css('display', 'none');
    $('#search-result').css('display', 'block');
  },
  search(key) {
    var result = ['lColor Picker — HTML Color Codes', 'lColor Picker — HTML Color Codes'];

    if (result.length) {
      for (var i = 0; i < result.length; i++) {
        var item = $('<li></li>');
        item.append(`<a href="#">${result[i]}</a>`);
        $('#search-result').append(item);
      }

      if (result.length == 10) {
        $('#search-result').append('<li class="_fs-24"> ... </li>');
      }
    }

    this.hideLoading();
  },
  onSearch: function (key) {
    var that = this;
    // key press
    // waiting 500ms ... has timeOut
    // key press again
    // has timeOut, cancel timeOut

    that.showLoading();
    if (that.timeOut)
      clearTimeout(that.timeOut);

    that.timeOut = setTimeout(function () {
      that.clearSearchResult();
      that.search(key);
    }, that.delay);
  },
  searchShow: function () {
    var that = this;
    $('body').toggleClass('_on-search');
    $("._search").toggleClass('show');
    $('._search ._search-bar input').focus();

    showed = true;

    if (that.firstSearch) {
      $('body._on-search').keydown((e) => {
        if (showed && e.keyCode == 27)
          that.searchClose();
      });

      that.firstSearch = false;
    }
  },
  searchClose: function () {
    $('body').toggleClass('_on-search');
    $("._search").toggleClass('show');
    showed = false;
  },

  init: function () {
    var that = this;
    //   
    // Show / Hide Search Menu
    $('#search-btn').click(() => that.searchShow());
    $('#search-close').click(() => that.searchClose());
    $('._search-backdrop').click(() => that.searchClose());
    //
    // Search Events
    $(that.container + ' ._search-bar input').keyup((e) => {
      var notEscape = e.keyCode != 27,
        charOnly = e.keyCode == 8 || (e.keyCode >= 48 && e.keyCode <= 90) || (e.keyCode >= 96 && e.keyCode <= 105);
      if (notEscape && charOnly) {
        var searchString = $(e.target).val();
        that.onSearch(searchString);
      }
    })

  }
}

SEARCH.init();

/*  
  FontAwesome Helper
  - Scale size with font-size base
  - Change fill color dynamically
*/

function fasIconHelper_updateSVGFill(target) {
  var parent = target.closest("[_used-icon]");
  var textColor = window.getComputedStyle(parent, null).getPropertyValue('color'),
    iconScale = parent.getAttribute("_used-icon") || 1,
    fontSize = window.getComputedStyle(parent, null).getPropertyValue('font-size');
  
  var iconSize = fontSize = +fontSize.substring(0, fontSize.length - 2) * iconScale;
  textColor = textColor || 'black';

  // console.log(iconSize);
  // console.log(textColor);

  target.setAttribute("fill", textColor);
  target.setAttribute("width", iconSize);

}

function fasIconHelper_assignEvent(target) {

  var parent = target.closest("[_used-icon]")
  parent.addEventListener('click', function() {
    fasIconHelper_updateSVGFill(target)
  });
  parent.addEventListener('mouseenter', function() {
    fasIconHelper_updateSVGFill(target)
  })
  parent.addEventListener('mouseleave', function() {
    fasIconHelper_updateSVGFill(target)
  })
}

function fasIconHelper_useIcons() {
  
  // Update on loaded
  //
  var allIcons = Array.from(document.getElementsByClassName("_icon"));
  for(var i = 0; i < allIcons.length; ++i) {
    fasIconHelper_updateSVGFill(allIcons[i]);
    fasIconHelper_assignEvent(allIcons[i]);
  }
}

fasIconHelper_useIcons();



// Color Scale

function rgba2hex(orig) {
  var a, isPercent,
    rgb = orig.replace(/\s/g, '').match(/^rgba?\((\d+),(\d+),(\d+),?([^,\s)]+)?/i),
    alpha = (rgb && rgb[4] || "").trim(),
    hex = rgb ? 
    (rgb[1] | 1 << 8).toString(16).slice(1) +
    (rgb[2] | 1 << 8).toString(16).slice(1) +
    (rgb[3] | 1 << 8).toString(16).slice(1) : orig;
      if (alpha !== "") {
        a = alpha;
      } else {
        a = 01;
      }

      a = Math.round(a * 100) / 100;
        var alpha = Math.round(a * 255);
        var hexAlpha = (alpha + 0x10000).toString(16).substr(-2).toUpperCase();
        hex = hex + hexAlpha;

  return hex;
}

function colorScale(R,G,B, name, start, end , total) {
  var variables = "", classes = '', 
  step = (start - end)/total, alpha = start - step ;
  var i = 1;
  while (i <= total) {
    var r = Math.round(R*alpha + 255*(1-alpha)),
    g = Math.round(G*alpha + 255*(1-alpha)),
    b = Math.round(B*alpha + 255*(1-alpha));
    
    variables +=`
    --txt-${name}-${i}: rgb(${r}, ${g}, ${b});
    `;
    classes += `
    ._txt-${name}-${i} { color: var(--txt-${name}-${i})!important;}  
    ._bg-${name}-${i} { background-color: var(--txt-${name}-${i})!important;}  
    `;
    
    console.log(alpha)
    
    alpha -= step;
    i++;
  }

  return `
  ._txt-white { color: white!important;}
  ._txt-black { color: black!important;}
  ._bg-white { background-color: white!important;}
  ._bg-black { background-color: black!important;}

    :root {
      ${variables}
    }

    ${classes}
  `;
}

function replaceChar(str,toReplace, replaceWith) {
    var result = "";
    if (!str || str.length == 0)
        return "";

    for (var i = 0; i < str.length; i++) {
        if (str[i] == toReplace)
            result += replaceWith;
        else
            result += str[i];
    }
    return result;
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split('; ');
    for (var i = 0; i < ca.length; i++) {
        var cookie = ca[i].split('=');
        if (cookie[0] == cname)
            return cookie[1];
    }
    return null;
}


//  Toggle class "active" of an element in a container
//  with options to de-active the activated element
function toggleActive(el, container, deActive = true, className = 'active') {
    $(`${container} .${className}`).removeClass( className );
    $(el).addClass('active');
}


// set or replace URL query parameter value
//
function extendQuery(_name, _value, _exclude = []) {
    if (!_value)
        return;

    var search = window.location.search;
    var params = [];
    if (search.length > 0) {
        params = search.substr(1)   // remove '?'
                       .split('&');
    }
    // init value
    //
    if (Array.isArray(_value)) {
        queryValue = _value.reduce(function (r, c) { r.push(_name + '=' + c); return r; }, [])
                        .join('&');
    }
    else {
        queryValue = `${_name}=${_value}`;
    }
    // combine & set query search string
    //
    if (_exclude && Array.isArray(_exclude)) {
        params.filter(p => _exclude.find(e => p.split('=')[0] == e) != undefined);
    }

    var index = params.findIndex((e) => e.split('=')[0] == _name);
    if (index != -1) {
        params[index] = queryValue;
    }
    else {
        params.push(queryValue);
    }
    window.location.search = "?" + params.join('&');

}