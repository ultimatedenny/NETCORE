function Cookies() { }

Cookies.key = '~Mwygvo0%eP2EDJTXSxYkj_qmAUOWKRHnpBdbNzZhcrQ-7t9lC.G6Fa5sVi4L8I13uf( '
var _charset = "@abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789%_-.& ";
 
Cookies.setCookie = function (Param, value) {

    const d = new Date();
    //var key = generateKey();
    var afterEncryption = encrypt(value, Cookies.key);
    var encodeValue = afterEncryption;
    var TotalDaysEspires = 30;// expires on 30 hari
    d.setTime(d.getTime() + (parseInt(TotalDaysEspires) * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = Param + "=" + encodeValue + ";" + expires + ";path=/";
}


Cookies.getCookie = function (cname) {
    var name = cname + "=";
    var decodedCookie = document.cookie;
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}


Cookies.Read = function (Param) {
    var value = Cookies.getCookie(Param);
    //var key = generateKey();
    var afterDescryp = decrypt(value, Cookies.key);
    return afterDescryp
}


Cookies.Delete = function (Param) {
    document.cookie = Param + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}




function generateKey() {
    var str = "";
    var tar;
    tar = new Array();
    for (var c = 0; c < _charset.length; c++) {
        tar.push(_charset.substr(c, 1));
    }
    for (var c = 0; c < _charset.length; c++) {
        str += tar.splice(Math.round(Math.random() * (tar.length - 1)), 1);
    }
    return str;
}

function encrypt(v, _key) {
    var str = "";
    var key = _key;
    if (_key.length > 0 && v.length > 0) {
        //v = Server.URLEncode(v);
        for (var c = 0; c < v.length; c++) {
            var ch = _charset.indexOf(v.substr(c, 1));
            if (ch > -1) {
                str += key.substr(ch, 1);
                //key = rotateKey(key, v.charCodeAt(c));
            }
        }
        return str;
    } else {
        return "";
    }
}

function decrypt(v, _key) {
    var str = "";
    var key = _charset;
    if (_key.length > 0 && v.length > 0) {
        for (var c = 0; c < v.length; c++) {
            var ch = _key.indexOf(v.substr(c, 1));
            if (ch > -1) {
                str += key.substr(ch, 1);
                //key = rotateKey(key, -key.charCodeAt(ch));
            }
        }
        return str;
    } else {
        return "";
    }
}

function rotateKey(s, amt) {
    amt = amt % s.length;
    if (amt < 0) {
        amt = s.length + amt;
    }
    if (amt != 0) {
        return s.substr(s.length - amt, amt) + s.substr(0, s.length - amt);
    } else {
        return unescape(s);
    }
}

