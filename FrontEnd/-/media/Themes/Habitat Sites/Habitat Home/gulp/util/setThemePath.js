//Used in post init
let path = require('path');
let fs = require('fs');

var getConf = () => {
    let fileContent = fs.readFileSync('gulp/serverConfig.json')
    return JSON.parse(fileContent)
}
let setConfig = (content) => {
    fs.writeFileSync('gulp/serverConfig.json', JSON.stringify(content));
}
module.exports = {
    "getConf": getConf,
    "updateConfig": function() {
        let currPath = path.join(__dirname, '../..'),
            mediaLibPath = currPath.split('-' + path.sep + 'media'),
            isCreativeExchange = mediaLibPath.length > 1,
            themePath,
            projPath;
        if (isCreativeExchange) {
            try {
                let config = getConf();
                themePath = mediaLibPath[1].match(/\\[\w\d\. ]*$/)[0];
                projPath = mediaLibPath[1].replace(/\\[\d\w\. ]*$/, '');
                config.serverOptions.projectPath = projPath;
                config.serverOptions.themePath = themePath;
                setConfig(config);
            } catch (e) {
                console.log(e);
            }

        }
    }
}