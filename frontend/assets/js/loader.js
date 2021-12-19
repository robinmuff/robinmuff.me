load();
async function load() {
    document.getElementById("name").innerText = await (await fetch("/myname")).text();

    var bulletPlaceHolder = "&nbsp;&bull;&nbsp;";
    var itemsDescription = await (await fetch("/mydescription")).json();
    var innerDescription = "";
    itemsDescription.forEach(element => {
        innerDescription += element + bulletPlaceHolder;
    });
    document.getElementById("description").innerHTML = innerDescription.substring(0, innerDescription.length-bulletPlaceHolder.length);

    var itemsContacts = await (await fetch("/mylinks")).json();
    var innerContacts = "";
    itemsContacts.forEach(element => {
        innerContacts += "<li><a href=\"" + element.link + "\" class=\"" + element.cssClass + "\"><span class=\"label\">Github</span></a></li>";
    });
    document.getElementById("contacts").innerHTML = innerContacts;
}