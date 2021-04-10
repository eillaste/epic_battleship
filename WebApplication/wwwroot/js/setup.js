async function toggler(){
    //document.getElementsByClassName("p1").classList.add("p2");

    var elements = await document.getElementsByClassName("p2");
    await  elements[0].classList.remove("p2");
    var element2 = await document.getElementById("p1");
    await element2.classList.add("p3");
    var element3 = await document.getElementById("p4");
    await element3.classList.add("p3");
    var element4 = await document.getElementsByClassName("p2");
    await element4[0].classList.remove("p2");

    var shiptypes = ["Carrier","Battleship","Submarine","Cruiser","Patrol"];
    shiptypes.forEach(shiptype => eraseOldSquares(shiptype));
}

function switcher(ship_type, cell){
    switch (ship_type){
        case "Carrier":
            cell.classList.add("carrier")
            break
        case "Battleship":
            cell.classList.add("battleship")
            break
        case "Submarine":
            cell.classList.add("submarine")
            break
        case "Cruiser":
            cell.classList.add("cruiser")
            break
        case "Patrol":
            cell.classList.add("patrol")
            break
    }
}
var shipsizes = {
    "Carrier":5,
    "Battleship":4,
    "Submarine":3,
    "Cruiser":2,
    "Patrol":1
}

var letters = ["a", "b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"];

function logInput(input){


    var shiptype = input.name.substring(0, input.name.length - 2);
    var shipsize = shipsizes[shiptype];
    var coordinate = input.value;
    var values = coordinate.split("-");
    //console.log(values);

    if (values[0][0] === values[1][0] && values[0].substring(1) === values[1].substring(1)){
        eraseOldSquares(shiptype);
        var td = document.getElementById(values[0][0]+values[0].substring(1))
        if (td.className.length > 0 ){
            return
        }
        switcher(shiptype, td)
    }
    if (values[0][0] === values[1][0]){
        //if letters are same, from up to down
        var myarr = [values[0].substring(1), values[1].substring(1)].sort();
        if (myarr[1] - myarr[0] + 1 === shipsize){
            eraseOldSquares(shiptype);
            for (i=1; i<=parseInt(myarr[1]); i++){
                var td = document.getElementById(values[0][0]+i);
                if (td.className.length > 0 ){
                    break
                }
                switcher(shiptype, td)
            }
        }
    } else if (values[0].substring(1) === values[1].substring(1)){
        // if numbers are same, from left to right a1 c1
        var num = values[1].substring(1);
        var firstletterindex = letters.indexOf(values[0][0]);
        var lastletterindex = letters.indexOf(values[1][0]);
        if (Math.abs(lastletterindex - firstletterindex )+ 1 === shipsize){
            eraseOldSquares(shiptype);
            var lettersordered = [firstletterindex, lastletterindex].sort();
            for (i=lettersordered[0]; i<=lettersordered[1]; i++){
                var td = document.getElementById(letters[i]+num);
                //console.log(td);
                if (td.className.length > 0 ){
                    break
                }
                td.className = '';
                //console.log(td);
                switcher(shiptype, td);
            }
        }

    }
}

function eraseOldSquares(ship_type){
    var oldsquares = document.getElementsByClassName(ship_type.toLowerCase())
    if (oldsquares.length > 0){
        Array.from(oldsquares).forEach((oldsquare) => {
            oldsquare.classList="";
        })
    }
}