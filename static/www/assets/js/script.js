let aboutme = [
    { "Name": "aboutme-name", "Value": "text" },
    { "Name": "aboutme-description", "Value": "text" },
    { "Name": "aboutme-socials", "Value": "json", "HandleWith": writeSocials },
    { "Name": "aboutme-home-title", "Value": "text" },
];

loadData();

async function loadData() {
    aboutme.forEach(async function (item) {
        let element = document.getElementsByClassName(item.Name)[0];
        let result;

        if (item.Value == "text")
            result = await getDataAsText(item.Name.replace("-", "/"));
        if (item.Value == "json")
            result = await getDataAsJson(item.Name.replace("-", "/"));
        
        console.log(item.Name.replace("-", "/"))
        console.log(result)
        if (item.HandleWith)
            item.HandleWith(element, result);
        else
            element.innerText = result;
    });
}
async function getDataAsJson(url) {
    return await (await fetch(url)).json();
}
async function getDataAsText(url) {
    return await (await fetch(url)).text();
}

async function writeSocials(element, socials) {
    socials.forEach(item => {
        let html = '<a href="{url}" target="_blank" class="{classes}" id="{id}"></a>';

        Object.getOwnPropertyNames(item).forEach(itemChild => {
            html = html.replace("{" + itemChild + "}", item[itemChild]);
        });

        element.innerHTML += html;
    });
}