var el = document.getElementsByTagName("td");
for (var i = 0; i < el.length; i++) {
    if(el[i].innerText === "?") {
        el[i].className += " " + "water";
    } else  if(el[i].innerText === "5") {
        el[i].className += " " + "carrier";
    } else  if(el[i].innerText === "4") {
        el[i].className += " " + "battleship";
    }else  if(el[i].innerText === "3") {
        el[i].className += " " + "submarine";
    }else  if(el[i].innerText === "2") {
        el[i].className += " " + "cruiser";
    }else  if(el[i].innerText === "1") {
        el[i].className += " " + "patrol";
    }else  if(el[i].innerText === "x") {
        el[i].className += " " + "hit";
    }else  if(el[i].innerText === "-") {
        el[i].className += " " + "miss";
    }else  if(el[i].innerText === "s") {
        el[i].className += " " + "sunk";
    }
}