let aboutme = [
    { "Name": "aboutme-name", "Value": "text" },
    { "Name": "aboutme-description", "Value": "text" },
    { "Name": "aboutme-socials", "Value": "json", "HandleWith": writeSocials },
    { "Name": "aboutme-home-title", "Value": "text" },
    { "Name": "aboutme-about-title", "Value": "text" },
    { "Name": "aboutme-about-texts", "Value": "object", "HandleWith": writeAboutTexts},
    { "Name": "aboutme-about-skills-title", "Value": "text" },
    { "Name": "aboutme-about-skills", "Value": "object", "HandleWith": wirteSkills },
    { "Name": "aboutme-about-languages-title", "Value": "text" },
    { "Name": "aboutme-about-languages", "Value": "object", "HandleWith": writeLanguages },

    { "Name": "aboutme-experience-title", "Value": "text" },
    { "Name": "aboutme-experience", "Value": "object", "HandleWith": writeExperience },
];

loadData();

async function loadData() {
    aboutme.forEach(async function (item) {
        let element = document.getElementsByClassName(item.Name)[0];
        let result;

        if (item.Value == "text")
            result = await getDataAsText(item.Name.replace("-", "/"));
        if (item.Value == "object")
            result = JSON.parse(await getDataAsText(item.Name.replace("-", "/")));
        if (item.Value == "json")
            result = await getDataAsJson(item.Name.replace("-", "/"));
        
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
async function writeAboutTexts(element, texts) {
    texts.forEach(item => {
        let html = '<p>{text}</p>';

        html = html.replace("{text}", item);

        element.innerHTML += html;
    });
}
async function wirteSkills(element, skills) {
    let skillCounter = 0;
    skills.forEach(item => {
        let html = '<span class="skill">{skill}</span>';

        html = html.replace("{skill}", item);

        element.innerHTML += html;
        
        skillCounter++;
        if (skillCounter == 4) {
            element.innerHTML += "<br>";
            skillCounter = 0;
        }
    });
}
async function writeLanguages(element, languages) {
    languages.forEach(item => {
        let html = '<div class="language"><p>{Language}</p><span class="bar"><span style="width: {SkillLevelProcent}%;"></span></span></div>';

        Object.getOwnPropertyNames(item).forEach(itemChild => {
            html = html.replace("{" + itemChild + "}", item[itemChild]);
        });

        element.innerHTML += html;
    });
}
async function writeExperience(element, experiences) {
    experiences.forEach(item => {
        let html = '<div class="timeline-item"><div class="timeline-date">{Timeframe}</div><div class="timeline-content"><h3>{Title}</h3><p>{Text}</p></div></div>';

        Object.getOwnPropertyNames(item).forEach(itemChild => {
            html = html.replace("{" + itemChild + "}", item[itemChild]);
        });

        element.innerHTML += html;
    });
}