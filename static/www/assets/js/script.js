let isString = value => typeof value === 'string' || value instanceof String;

let getData = async url => await fetch("/api/" + url);
let getMyElementByName = name => document.getElementsByClassName("robinmuff-" + name)[0];
let getTemplate = async name => await getDataAsText("template-" + name);

let getDataAsJson = async url => await (await getData(url)).json();
let getDataAsText = async url => await (await getData(url)).text();

loadData();

async function loadData() {
    let aboutme = await getDataAsJson("structure");

    aboutme.forEach(async function (item) {
        let result;

        if (item.Value == "text") result = await getDataAsText(item.Name);
        if (item.Value == "object") result = JSON.parse(await getDataAsText(item.Name));
        if (item.Value == "json") result = await getDataAsJson(item.Name);

        console.log(result)

        writeData(item.Name, result);
    });
}

function replaceOwnPropertyNames(template, item) {
    Object.getOwnPropertyNames(item).forEach(itemChild => {
        let replacementMethods = [
            {"Text": "{[" + itemChild + "]}", "Replace": "item[itemChild]"},
            {"Text": "{[" + itemChild + "|lower]}", "Replace": "item[itemChild].toLowerCase()"},
            {"Text": "{[_item_]}", "Replace": "item"}
        ];

        replacementMethods.forEach(async method => {
            if (template.includes(method.Text)) template = template.replaceAll(method.Text, eval(method.Replace));
        });
    });
    return template;
}

async function writeDocumentData(name, data) {
    // Extract last part for element "document-title" => "title"
    let element = name.split("-").slice(-1);

    document[element] = data;
}

async function writeData(name, data) {
    if (name.startsWith("document")) {
        writeDocumentData(name, data);
        return;
    }

    if (isString(data)) {
        getMyElementByName(name).innerText = data;
        return;
    }

    let html = await getTemplate(name);

    data.forEach(async item => {        
        let currentHtml = replaceOwnPropertyNames(html, item);

        getMyElementByName(name).innerHTML += currentHtml;
    });
}